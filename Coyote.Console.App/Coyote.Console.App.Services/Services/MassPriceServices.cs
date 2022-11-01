using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class MassPriceServices : IMassPriceServices
    {
        private IUnitOfWork _unitOfWork = null;

        public MassPriceServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MassPriceUpdateResultModel> MassPriceUpdateAsync(MassPriceUpdateRequestModel requestModel, int userId)
        {
            try
            {
                var storeRepo = _unitOfWork.GetRepository<Store>();
                var deptRepo = _unitOfWork.GetRepository<Department>();
                var comRepo = _unitOfWork.GetRepository<Commodity>();
                var sysRepo = _unitOfWork.GetRepository<SystemControls>();
                var outletProdRepo = _unitOfWork.GetRepository<OutletProduct>();

                var response = new MassPriceUpdateResultModel();

                var store = await (storeRepo?.GetAll()?.Where(x => x.Id == requestModel.StoreId && !x.IsDeleted && x.Status)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (store == null)
                {
                    throw new BadRequestException(ErrorMessages.StoreInactive.ToString(CultureInfo.CurrentCulture));
                }

                if (!await (deptRepo.GetAll().Where(x => x.Id == requestModel.DepartmentId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new BadRequestException(ErrorMessages.DepartmentIdNotFound.ToString(CultureInfo.CurrentCulture));
                }

                if (requestModel?.CommodityId != null)
                {
                    if(requestModel.CommodityId != 0)
                    {
                        if (!await (comRepo.GetAll().Where(x => x.Id == requestModel.CommodityId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                    }                    
                }

                if (requestModel?.OutletPassword != store.Code)
                {
                    throw new BadRequestException(ErrorMessages.InvalidOutletPass.ToString(CultureInfo.CurrentCulture));
                }

                if (string.IsNullOrEmpty(requestModel.SystemPassword))
                {
                    throw new BadRequestException(ErrorMessages.InvalidSysPass.ToString(CultureInfo.CurrentCulture));
                }
                else
                {
                    var sysPassword = await sysRepo.GetAll(x => x.IsActive == Status.Active && x.MassPriceUpdate == requestModel.SystemPassword).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (sysPassword == null)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidSysPass.ToString(CultureInfo.CurrentCulture));
                    }
                }

                if (requestModel.ChaneSellGP <= 0)
                {
                    throw new BadRequestException(ErrorMessages.InvalidSellGP.ToString(CultureInfo.CurrentCulture));
                }

                var toUpdate = outletProdRepo.GetAll().Include(c => c.Product).ThenInclude(c => c.ProductZonePricing).Where(x => !x.IsDeleted && x.Status && x.StoreId == requestModel.StoreId);

                if (requestModel.DepartmentId > 0)
                {
                    toUpdate = toUpdate.Where(x => x.Product.DepartmentId == requestModel.DepartmentId);
                }
                if (requestModel?.CommodityId != null)
                {
                    if(requestModel.CommodityId != 0)
                    {
                        toUpdate = toUpdate.Where(x => x.Product.CommodityId == requestModel.CommodityId);
                    }              
                }

                //if (requestModel.OnlyHostCodes && !string.IsNullOrEmpty(store.PriceZone))
                //{
                //    toUpdate = toUpdate.Where(x => x.Product.ProductZonePricing.Equals(store.PriceZone));
                //}

                //to use if changed function flow.
                //var response = (await toUpdate.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMassPriceMap);

                var outletProducts = await toUpdate.ToListAsyncSafe().ConfigureAwait(false);

                float costFactor = 1;
                if (requestModel.ChangeCost > 0)
                {
                    costFactor = (requestModel.ChangeCost / 100) + 1;
                }

                foreach (var item in outletProducts)
                {
                    if (item.Product.DepartmentId == requestModel.DepartmentId && item.CartonCost > 0 && item.Product.UnitQty > 0 && item.StoreId == requestModel.StoreId)
                    {
                        float CartonCost = 0;
                        // Calc New Ctn Cost
                        CartonCost = item.CartonCost * costFactor;

                        // Calc Item Cost
                        float itemCost = 0;
                        if (item.CartonCost > 0)
                        {
                            itemCost = (CartonCost / item.Product.CartonQty) * item.Product.UnitQty;
                        }

                        float sellPrice = 0;
                        // Calc New price
                        if (itemCost != 0 && requestModel.ChaneSellGP != 100)
                        {
                            sellPrice = itemCost / ((100 - requestModel.ChaneSellGP) / 100);
                        }
                        if (sellPrice == 0)
                        {
                            sellPrice = item.NormalPrice1;
                        }

                        if (requestModel.RoundSellPrice)
                        {
                            var toDecimal = sellPrice > 0 ? Convert.ToDecimal(sellPrice) : 0;
                            sellPrice = (float)Math.Round(toDecimal, 2);
                        }

                        if (CartonCost == 0)
                        { CartonCost = item.CartonCost; }
                        item.ChangeLabelInd = true;

                        if (sellPrice == item.NormalPrice1)
                        { item.ChangeLabelInd = false; }

                        if (sellPrice != item.NormalPrice1 || CartonCost != item.CartonCost)
                        {
                            item.CartonCost = CartonCost;
                            item.CartonCostHost = CartonCost;
                            item.NormalPrice1 = sellPrice;
                            item.UpdatedById = userId;

                            outletProdRepo.DetachLocal(x => x.Id == item.Id);
                            outletProdRepo.Update(item);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                response.UpdatedRec= $"{outletProducts.Count} { ErrorMessages.ProductsUpdated}";
                return response;
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
