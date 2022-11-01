using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class ManualSaleService : IManualSaleService
    {
        private IUnitOfWork _unitOfWork = null;
        /// <summary>
        /// ManualSaleService Constructor, initilize IMasterListRepository,IAutoMappingServices
        /// </summary>
        /// <param name="IMasterListRepository"></param>
        /// <param name="iAutoMapperService"></param>
        public ManualSaleService(IUnitOfWork repo)
        {
            _unitOfWork = repo;
        }
        public async Task<int> GetManualSaleRefrenceNo()
        {
            try
            {
                //Random rng = new Random();
                var repoManualSale = _unitOfWork?.GetRepository<ManualSale>();
                var refrenceNo = await repoManualSale.GetAll(x => !x.IsDeleted).MaxAsync(x => x.Code).ConfigureAwait(false);
                if (refrenceNo == null)
                    return 100;
                return Convert.ToInt32(refrenceNo) + 1;
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<PagedOutputModel<List<ManualSaleResponseModel>>> GetManualSale(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repoManualSale = _unitOfWork?.GetRepository<ManualSale>();
                var manualSaleList = repoManualSale.GetAll().Include(x => x.ManualSaleItem).Where(x => !x.IsDeleted);
                if (manualSaleList == null)
                {
                    throw new NotFoundException(ErrorMessages.ManualSaleNotFound.ToString(CultureInfo.CurrentCulture));
                }

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        manualSaleList = manualSaleList.Where(x => x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    manualSaleList = manualSaleList.OrderByDescending(x => x.UpdatedAt);
                    count = manualSaleList.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        manualSaleList = manualSaleList.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    manualSaleList = manualSaleList.OrderBy(x => x.Code);
                                else
                                    manualSaleList = manualSaleList.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    manualSaleList = manualSaleList.OrderBy(x => x.Desc);
                                else
                                    manualSaleList = manualSaleList.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    manualSaleList = manualSaleList.OrderBy(x => x.Id);
                                else
                                    manualSaleList = manualSaleList.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }

                var result = (await manualSaleList.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateManualSaleMap).ToList();
                return new PagedOutputModel<List<ManualSaleResponseModel>>(result, count);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<PagedOutputModel<List<ManualSaleResponseModel>>> GetAllManualSale(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                List<ManualSaleResponseModel> viewModel = new List<ManualSaleResponseModel>();
                var repository = _unitOfWork?.GetRepository<ManualSale>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@PageNumber", inputModel?.SkipCount),
                        new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        new SqlParameter("@Module","ManualSale"),
                        new SqlParameter("@RoleId",RoleId)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveStores, dbParams.ToArray()).ConfigureAwait(false);
                var count = 0;
                if (dset.Tables.Count > 1)
                {
                    viewModel = MappingHelpers.ConvertDataTable<ManualSaleResponseModel>(dset.Tables[0]);

                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.ManualSaleNotFound.ToString(CultureInfo.CurrentCulture));
                }
                return new PagedOutputModel<List<ManualSaleResponseModel>>(viewModel, count);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<ManualSaleResponseModel> GetManualSaleById(long Id)
        {
            try
            {
                var repoManualSale = _unitOfWork?.GetRepository<ManualSale>();
                var manualSale = await repoManualSale.GetAll()
                    .Include(x => x.ManualSaleItem).ThenInclude(x => x.Product)
                    .Include(x => x.ManualSaleItem).ThenInclude(x => x.Store)
                    .Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (manualSale == null)
                {
                    throw new NotFoundException(ErrorMessages.ManualSaleNotFound.ToString(CultureInfo.CurrentCulture));
                }
                var manualSaleResponse = MappingHelpers.CreateManualSaleMap(manualSale);
                var manualSaleItemList = (manualSale.ManualSaleItem.Where(x => !x.IsDeleted).ToList()).Select(MappingHelpers.CreateManualSaleItemMap).ToList();
                if (manualSaleItemList.Count > 0)
                    manualSaleResponse.manualSaleItemResponseModels.AddRange(manualSaleItemList);
                return manualSaleResponse;
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<ManualSaleResponseModel> AddManualSale(ManualSaleRequestModel requestModel, int userId)
        {
            try
            {
                if (requestModel == null)
                    throw new NullReferenceException();

                if (string.IsNullOrEmpty(requestModel.Code.ToString()))
                    throw new BadRequestException(ErrorMessages.ManualSaleCodeIsRequired.ToString(CultureInfo.CurrentCulture));

                if (string.IsNullOrEmpty(requestModel.Desc))
                    throw new BadRequestException(ErrorMessages.ManualSaleDescIsRequired.ToString(CultureInfo.CurrentCulture));

                var manualSale = new ManualSale();
                var repoManualSale = _unitOfWork?.GetRepository<ManualSale>();
                var repoProduct = _unitOfWork?.GetRepository<Product>();
                var repoStore = _unitOfWork?.GetRepository<Store>();
                var repoOutletProduct = _unitOfWork?.GetRepository<OutletProduct>();
                var repoManualSaleItem = _unitOfWork?.GetRepository<ManualSaleItem>();
                var repoMasterListItems = _unitOfWork?.GetRepository<MasterListItems>();

                if (await repoManualSale.GetAll().Where(x => x.Code == requestModel.Code.ToString() && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                    throw new AlreadyExistsException(ErrorMessages.ManualSaleCodeDuplicate.ToString(CultureInfo.CurrentCulture));

                manualSale.Code = requestModel.Code.ToString();
                manualSale.Desc = requestModel.Desc;
                manualSale.TypeId = repoMasterListItems.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Code == "MANUALSALE" && x.MasterList.Code == "MANUALSALE").FirstOrDefault().Id;
                manualSale.CreatedAt = DateTime.UtcNow;
                manualSale.CreatedById = userId;
                manualSale.UpdatedAt = DateTime.UtcNow;
                manualSale.UpdatedById = userId;

                foreach (var obj in requestModel.ManualSaleItemRequestModel)
                {
                    if (obj.ProductId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                    }
                    if (obj.OutletId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.OutletId.ToString(CultureInfo.CurrentCulture));
                    }
                    if (obj.Qty <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ManualSaleInvalidQuantity.ToString(CultureInfo.CurrentCulture));
                    }
                    if (obj.Price <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ManualSaleInvalidPrice.ToString(CultureInfo.CurrentCulture));
                    }
                    if (string.IsNullOrEmpty(obj.PriceLevel))
                    {
                        throw new BadRequestException(ErrorMessages.ManualSalePriceLevelrequired.ToString(CultureInfo.CurrentCulture));
                    }

                    var product = await repoProduct.GetAll().Where(x => x.Id == obj.ProductId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (product == null)
                        throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));

                    var outlet = await repoStore.GetAll().Where(x => x.Id == obj.OutletId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (outlet == null)
                        throw new NotFoundException(ErrorMessages.OutletNotFound.ToString(CultureInfo.CurrentCulture));

                    var outletProduct = await repoOutletProduct.GetAll().Where(x => x.StoreId == obj.OutletId && x.ProductId == obj.ProductId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (outletProduct == null)
                        throw new NotFoundException(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture));

                    if (manualSale.ManualSaleItem == null)
                        manualSale.ManualSaleItem = new List<ManualSaleItem>();

                    var manualSaleItem = MappingHelpers.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(obj);
                    manualSaleItem.TypeId = repoMasterListItems.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Code == "MANUALSALEI" && x.MasterList.Code == "MANUALSALEI").FirstOrDefault().Id;
                    manualSaleItem.Amount = obj.Qty * obj.Price;
                    manualSaleItem.Cost = 0;
                    if (outletProduct.CartonCost > 0 && product.CartonQty > 0 && product.UnitQty > 0)
                    {
                        manualSaleItem.Cost = (outletProduct.CartonCost / product.CartonQty) * product.UnitQty;
                    }
                    manualSaleItem.CreatedAt = DateTime.UtcNow;
                    manualSaleItem.CreatedById = userId;
                    manualSaleItem.UpdatedAt = DateTime.UtcNow;
                    manualSaleItem.UpdatedById = userId;
                    manualSale.ManualSaleItem.Add(manualSaleItem);
                }

                var result = await repoManualSale.InsertAsync(manualSale).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetManualSaleById(result.Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<ManualSaleResponseModel> EditManualSale(ManualSaleRequestModel requestModel, long Id, int userId)
        {
            try
            {
                if (requestModel == null || Id == 0)
                    throw new NullReferenceException();

                if (string.IsNullOrEmpty(requestModel.Code.ToString()))
                    throw new BadRequestException(ErrorMessages.ManualSaleCodeIsRequired.ToString(CultureInfo.CurrentCulture));

                if (string.IsNullOrEmpty(requestModel.Desc))
                    throw new BadRequestException(ErrorMessages.ManualSaleDescIsRequired.ToString(CultureInfo.CurrentCulture));

                var repoManualSale = _unitOfWork?.GetRepository<ManualSale>();
                var repoManualSaleItem = _unitOfWork?.GetRepository<ManualSaleItem>();
                var repoProduct = _unitOfWork?.GetRepository<Product>();
                var repoStore = _unitOfWork?.GetRepository<Store>();
                var repoOutletProduct = _unitOfWork?.GetRepository<OutletProduct>();
                var repoMasterListItems = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);

                if (!await repoManualSale.GetAll().Where(x => x.Id == Id && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                    throw new NotFoundException(ErrorMessages.ManualSaleNotFound.ToString(CultureInfo.CurrentCulture));

                if (await repoManualSale.GetAll().Where(x => x.Id != Id && x.Code == requestModel.Code.ToString() && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                    throw new AlreadyExistsException(ErrorMessages.ManualSaleCodeDuplicate.ToString(CultureInfo.CurrentCulture));

                var manualSale = await repoManualSale.GetAll().Include(x => x.ManualSaleItem).Where(x => x.Id == Id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                manualSale.Code = requestModel?.Code ?? manualSale.Code;
                manualSale.Desc = requestModel?.Desc ?? manualSale.Desc;
                manualSale.TypeId = repoMasterListItems.Where(x => !x.IsDeleted && x.Code == "MANUALSALE" && x.MasterList.Code == "MANUALSALE").FirstOrDefault().Id;
                manualSale.UpdatedAt = DateTime.UtcNow;
                manualSale.UpdatedById = userId;

                #region delete children
                if (manualSale.ManualSaleItem?.Count > 0)
                {
                    foreach (var existingChild in manualSale.ManualSaleItem.ToList())
                    {
                        var child = requestModel.ManualSaleItemRequestModel.FirstOrDefault(c => c.ProductId == existingChild.ProductId && c.OutletId == existingChild.OutletId);
                        if (child == null)
                        {
                            //item is deleted
                            existingChild.UpdatedAt = DateTime.UtcNow;
                            existingChild.UpdatedById = userId;
                            existingChild.IsDeleted = true;
                            repoManualSaleItem.Update(existingChild);                          
                        }
                    }
                }
                #endregion
              
                repoManualSale.Update(manualSale);

                foreach (var obj in requestModel.ManualSaleItemRequestModel)
                {
                    if (obj.ProductId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                    }
                    if (obj.OutletId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.OutletId.ToString(CultureInfo.CurrentCulture));
                    }
                    if (obj.Qty <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ManualSaleInvalidQuantity.ToString(CultureInfo.CurrentCulture));
                    }
                    if (obj.Price <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ManualSaleInvalidPrice.ToString(CultureInfo.CurrentCulture));
                    }
                    if (string.IsNullOrEmpty(obj.PriceLevel))
                    {
                        throw new BadRequestException(ErrorMessages.ManualSalePriceLevelrequired.ToString(CultureInfo.CurrentCulture));
                    }

                    var product = await repoProduct.GetAll().Where(x => x.Id == obj.ProductId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (product == null)
                        throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));

                    var outlet = await repoStore.GetAll().Where(x => x.Id == obj.OutletId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (outlet == null)
                        throw new NotFoundException(ErrorMessages.OutletNotFound.ToString(CultureInfo.CurrentCulture));

                    var outletProduct = await repoOutletProduct.GetAll().Where(x => x.StoreId == obj.OutletId && x.ProductId == obj.ProductId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (outletProduct == null)
                        throw new NotFoundException(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture));

                    var manualSaleItem = manualSale?.ManualSaleItem?.Where(x => x.ProductId == obj.ProductId && x.OutletId == obj.OutletId && !x.IsDeleted).FirstOrDefault();
                    if (manualSaleItem != null)
                    {
                        manualSaleItem.TypeId = repoMasterListItems.Where(x => !x.IsDeleted && x.Code == "MANUALSALEI" && x.MasterList.Code == "MANUALSALEI").FirstOrDefault().Id;
                        manualSaleItem.Qty = obj?.Qty ?? manualSaleItem.Qty;
                        manualSaleItem.Price = obj?.Price ?? manualSaleItem.Price;
                        manualSaleItem.Amount = manualSaleItem.Qty * manualSaleItem.Price;
                        manualSaleItem.Cost = 0;
                        if (outletProduct.CartonCost > 0 && product.CartonQty > 0 && product.UnitQty > 0)
                        {
                            manualSaleItem.Cost = (outletProduct.CartonCost / product.CartonQty) * product.UnitQty;
                        }
                        manualSaleItem.PriceLevel = obj?.PriceLevel ?? manualSaleItem.PriceLevel;
                        manualSaleItem.UpdatedAt = DateTime.UtcNow;
                        manualSaleItem.UpdatedById = userId;
                        repoManualSale.DetachLocal(_ => _.Id == manualSaleItem.Id);
                        repoManualSaleItem.Update(manualSaleItem);
                    }
                    else
                    {
                        manualSaleItem = MappingHelpers.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(obj);
                        manualSaleItem.TypeId = repoMasterListItems.Where(x => !x.IsDeleted && x.Code == "MANUALSALEI" && x.MasterList.Code == "MANUALSALEI").FirstOrDefault().Id;
                        manualSaleItem.Amount = obj.Qty * obj.Price;
                        manualSaleItem.Cost = 0;
                        if (outletProduct.CartonCost > 0 && product.CartonQty > 0 && product.UnitQty > 0)
                        {
                            manualSaleItem.Cost = (outletProduct.CartonCost / product.CartonQty) * product.UnitQty;
                        }
                        manualSaleItem.CreatedAt = DateTime.UtcNow;
                        manualSaleItem.CreatedById = userId;
                        manualSaleItem.UpdatedAt = DateTime.UtcNow;
                        manualSaleItem.UpdatedById = userId;
                        manualSaleItem.ManualSaleId = Id;
                        await repoManualSaleItem.InsertAsync(manualSaleItem).ConfigureAwait(false);
                    }   
                }
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetManualSaleById(Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<bool> DeleteManualSale(long Id, int userId)
        {
            try
            {
                var repoManualSale = _unitOfWork?.GetRepository<ManualSale>();
                var manualSale = await repoManualSale.GetAll().Include(x => x.ManualSaleItem).Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefaultAsync().ConfigureAwait(false);
                if (manualSale == null)
                {
                    throw new NotFoundException(ErrorMessages.ManualSaleNotFound.ToString(CultureInfo.CurrentCulture));
                }
                manualSale.IsDeleted = true;
                foreach (var obj in manualSale.ManualSaleItem)
                {
                    obj.IsDeleted = true;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedById = userId;
                }
                manualSale.UpdatedAt = DateTime.UtcNow;
                manualSale.UpdatedById = userId;
                repoManualSale.Update(manualSale);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<bool> DeleteManualSaleItem(long Id, int userId)
        {
            try
            {
                var repoManualSaleItem = _unitOfWork?.GetRepository<ManualSaleItem>();
                var manualSale = await repoManualSaleItem.GetAll(x => !x.IsDeleted && x.Id == Id).FirstOrDefaultAsync().ConfigureAwait(false);
                if (manualSale == null)
                {
                    throw new NotFoundException(ErrorMessages.ManualSaleItemNotFound.ToString(CultureInfo.CurrentCulture));
                }
                manualSale.IsDeleted = true;
                manualSale.UpdatedById = userId;
                manualSale.UpdatedAt = DateTime.Now;
                repoManualSaleItem.Update(manualSale);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
