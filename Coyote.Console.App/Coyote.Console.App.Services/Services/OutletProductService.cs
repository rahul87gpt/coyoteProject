using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository;
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

namespace Coyote.Console.App.Services
{
    public class OutletProductService : IOutletProductService
    {
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly IUnitOfWork _unitOfWork;

        public OutletProductService(IAutoMappingServices iAutoMapper, IUnitOfWork unitOfWork)
        {
            _iAutoMapper = iAutoMapper;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all active Outlet Products
        /// </summary>
        /// <returns>List<OutletProductViewModel></returns>
        public async Task<PagedOutputModel<List<OutletProductResponseViewModel>>> GetAllActiveOutletProducts(OutletProductFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var outletProducts = _unitOfWork?.GetRepository<OutletProduct>().GetAll(x => !x.IsDeleted);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        outletProducts = outletProducts.Where(x => x.ProductId.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.StoreId.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.SupplierId.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.ProductId)))
                        outletProducts = outletProducts.Where(x => x.ProductId.ToString().ToLower().Equals(inputModel.ProductId.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        outletProducts = outletProducts.Where(x => x.Status);

                    if (!string.IsNullOrEmpty((inputModel?.Id)))
                        outletProducts = outletProducts.Where(x => x.Id.ToString().ToLower().Equals(inputModel.Id.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        outletProducts = outletProducts.Where(x => x.Status);

                    if (!string.IsNullOrEmpty((inputModel?.StoreId)))
                        outletProducts = outletProducts.Where(x => x.StoreId.ToString().ToLower().Contains(inputModel.StoreId.ToLower()));


                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        outletProducts = outletProducts.Where(x => securityViewModel.StoreIds.Contains(x.StoreId));

                    outletProducts = outletProducts.OrderByDescending(x => x.UpdatedAt);
                    count = outletProducts.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        outletProducts = outletProducts.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "storeid":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    outletProducts = outletProducts.OrderBy(x => x.StoreId);
                                else
                                    outletProducts = outletProducts.OrderByDescending(x => x.StoreId);
                                break;
                            case "productid":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    outletProducts = outletProducts.OrderBy(x => x.ProductId);
                                else
                                    outletProducts = outletProducts.OrderByDescending(x => x.ProductId);
                                break;
                            case "supplierid":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    outletProducts = outletProducts.OrderBy(x => x.SupplierId);
                                else
                                    outletProducts = outletProducts.OrderByDescending(x => x.SupplierId);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    outletProducts = outletProducts.OrderBy(x => x.UpdatedAt);
                                else
                                    outletProducts = outletProducts.OrderByDescending(x => x.UpdatedAt);
                                break;
                        }
                    }
                }

                var outletProductList = outletProducts.Include(c => c.Store).Include(c => c.Supplier)
                    .Include(c => c.PromotionSell)
                    .Include(c => c.PromotionBuy)
                    .Include(c => c.PromotionMixMatch1)
                    .Include(c => c.PromotionMixMatch2)
                    .Include(c => c.PromotionMember)
                    .Include(c => c.PromotionOffer1)
                    .Include(c => c.PromotionOffer2)
                    .Include(c => c.PromotionOffer3)
                    .Include(c => c.PromotionOffer4)
                    .Include(c => c.PromotionComp)
                    .Include(x => x.Product).ThenInclude(c => c.GroupMasterListItem)
                    .Include(x => x.Product).ThenInclude(c => c.CategoryMasterListItem)
                    .Include(x => x.Product).ThenInclude(c => c.TypeMasterListItem)
                    .Include(x => x.Product).ThenInclude(c => c.Tax)
                    .Include(x => x.Product).ThenInclude(c => c.Commodity)
                    .Include(x => x.Product).ThenInclude(c => c.Department);


                List<OutletProductResponseViewModel> outletViewModel;

                outletViewModel = (await outletProductList.ToListAsync().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                //outletViewModel = (await outletProductList.ToListAsync().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<OutletProduct, OutletProductResponseViewModel>).ToList();

                return new PagedOutputModel<List<OutletProductResponseViewModel>>(outletViewModel, count);
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), nfe);
            }
            catch (NullReferenceException nre)
            {
                throw new NullReferenceException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound, ex);
            }
        }

        /// <summary>
        /// Get all active Outlet Products
        /// </summary>
        /// <returns>List<OutletProductViewModel></returns>
        public async Task<PagedOutputModel<List<OutletProductResponseViewModel>>> GetActiveOutletProducts(OutletProductFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                bool StoreIds = false;
                var AccessStores = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                {
                    foreach (var storeId in securityViewModel.StoreIds)
                        AccessStores += storeId + ",";
                    StoreIds = true;
                }

                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                var repository = _unitOfWork?.GetRepository<OutletProduct>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@ProductId", inputModel?.ProductId),
                        new SqlParameter("@Id", inputModel?.Id),
                        new SqlParameter("@StoreId", inputModel?.StoreId),
                        new SqlParameter("@StoreIds", (StoreIds == true)?AccessStores:null),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        //new SqlParameter("@Module","OutletProduct"),
                        new SqlParameter("@RoleId",RoleId)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveOutletProducts, dbParams.ToArray()).ConfigureAwait(false);
                List<OutletProductResponseViewModel> outletViewModel = MappingHelpers.ConvertDataTable<OutletProductResponseViewModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
                return new PagedOutputModel<List<OutletProductResponseViewModel>>(outletViewModel, count);
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), nfe);
            }
            catch (NullReferenceException nre)
            {
                throw new NullReferenceException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound, ex);
            }
        }
        /// <summary>
        /// Get all active Outlet Products
        /// </summary>
        /// <returns>List<OutletProductViewModel></returns>
        public async Task<PagedOutputModel<List<OutletProductResponseViewModel>>> GetActiveOutletProductsByProductId(OutletProductFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                bool StoreIds = false;
                var AccessStores = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                {
                    foreach (var storeId in securityViewModel.StoreIds)
                        AccessStores += storeId + ",";
                    StoreIds = true;
                }

                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                var repository = _unitOfWork?.GetRepository<OutletProduct>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@ProductId", inputModel?.ProductId),
                        new SqlParameter("@Id", inputModel?.Id),
                        new SqlParameter("@StoreId", inputModel?.StoreId),
                        new SqlParameter("@StoreIds", (StoreIds == true)?AccessStores:null),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        //new SqlParameter("@Module","OutletProduct"),
                        new SqlParameter("@RoleId",RoleId)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveOutletProductsByProdId, dbParams.ToArray()).ConfigureAwait(false);
                List<OutletProductResponseViewModel> outletViewModel = MappingHelpers.ConvertDataTable<OutletProductResponseViewModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
                return new PagedOutputModel<List<OutletProductResponseViewModel>>(outletViewModel, count);
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), nfe);
            }
            catch (NullReferenceException nre)
            {
                throw new NullReferenceException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound, ex);
            }
        }
        public async Task<List<OutletProductResponseViewModel>> Insert(OutletProductRequestModel viewModel, int userId)
        {
            List<OutletProductResponseViewModel> response = new List<OutletProductResponseViewModel>();
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                if (viewModel != null)
                {
                    if (viewModel.StoreId > 0)
                    {
                        var storeRepo = _unitOfWork?.GetRepository<Store>();
                        if (!await storeRepo.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));

                        }

                        if (viewModel.SupplierId > 0)
                        {

                            var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
                            if (!await supplierRepo.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                            {
                                throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));

                            }
                        }

                        if (viewModel.ProductId > 0)
                        {
                            if ((await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.ProductId == viewModel.ProductId && x.SupplierId == viewModel.SupplierId).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new AlreadyExistsException(ErrorMessages.OutletProductDuplicate.ToString(CultureInfo.CurrentCulture));
                            }
                            var outletProduct = _iAutoMapper.Mapping<OutletProductRequestModel, OutletProduct>(viewModel);
                            outletProduct.IsDeleted = false;
                            outletProduct.CreatedAt = DateTime.UtcNow;
                            outletProduct.UpdatedAt = DateTime.UtcNow;
                            outletProduct.CreatedById = userId;
                            outletProduct.UpdatedById = userId;
                            var result = await repository.InsertAsync(outletProduct).ConfigureAwait(false);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            if (result != null)
                            {
                                response = await GetOutletProductsByStoreId(result.Id).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            throw new NullReferenceCustomException(ErrorMessages.OutletProductId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new NullReferenceCustomException(ErrorMessages.StoreIdRequired.ToString(CultureInfo.CurrentCulture));
                    }
                }
            }
            catch (AlreadyExistsException aee)
            {
                throw new AlreadyExistsException(aee.Message, aee);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), ex);
            }
            return response;
        }

        public async Task<bool> DeleteOutletProduct(long storeId, long productId, long? supplierId, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                if (storeId > 0)
                {
                    if (productId > 0)
                    {
                        var outletProductExists = await repository.GetAll(x => x.StoreId == storeId && x.ProductId == productId && x.SupplierId == supplierId).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (outletProductExists != null)
                        {
                            if (!outletProductExists.IsDeleted)
                            {
                                outletProductExists.UpdatedById = userId;
                                outletProductExists.IsDeleted = true;
                                repository?.Update(outletProductExists);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                return true;
                            }
                            throw new NullReferenceCustomException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    throw new NullReferenceCustomException(ErrorMessages.OutletProductId.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceCustomException(ErrorMessages.OutletProductId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<List<OutletProductResponseViewModel>> GetOutletProductsByStoreId(long storeId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                if (storeId > 0)
                {
                    var outletProducts = await repository.GetAll(x => x.StoreId == storeId && !x.IsDeleted)
                        .Include(c => c.Store).Include(c => c.Supplier)
                        .Include(c => c.Product).ThenInclude(c => c.PromotionOfferProduct).ThenInclude(c => c.PromotionOffer).ThenInclude(c => c.Promotion)
                    .Include(x => x.Product).ThenInclude(c => c.PromotionMemberOffer).ThenInclude(c => c.Promotion)
                    .Include(x => x.Product).ThenInclude(x => x.PromotionMixmatchProduct).ThenInclude(x => x.PromotionMixmatch).ThenInclude(x => x.Promotion)
                       .Include(x => x.Product).ThenInclude(c => c.GroupMasterListItem)
                    .Include(x => x.Product).ThenInclude(c => c.CategoryMasterListItem)
                    .Include(x => x.Product).ThenInclude(c => c.TypeMasterListItem)
                    .Include(x => x.Product).ThenInclude(c => c.Tax)
                    .Include(x => x.Product).ThenInclude(c => c.Commodity)
                    .Include(x => x.Product).ThenInclude(c => c.Department)
                    .ToListAsync().ConfigureAwait(false);


                    var outletViewModel = outletProducts.Select(MappingHelpers.CreateMap).ToList();
                    if (outletViewModel == null)
                    {
                        throw new NotFoundException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    return outletViewModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.OutletProductId.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<bool> Update(OutletProductRequestModel viewModel, long id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                if (viewModel != null)
                {
                    if (id == 0)
                    {
                        throw new NullReferenceCustomException(ErrorMessages.OutletProductId.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.StoreId > 0)
                    {
                        var storeRepo = _unitOfWork?.GetRepository<Store>();
                        if (!await storeRepo.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));

                        }

                        if (viewModel.SupplierId > 0)
                        {

                            var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
                            if (!await supplierRepo.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                            {
                                throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));

                            }
                        }

                        var exists = await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.ProductId == viewModel.ProductId).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (exists != null)
                        {
                            if (exists.IsDeleted == true)
                            {
                                throw new NotFoundException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            var outletProductViewModel = _iAutoMapper.Mapping<OutletProductRequestModel, OutletProduct>(viewModel);
                            outletProductViewModel.IsDeleted = false;
                            outletProductViewModel.CreatedAt = exists.CreatedAt;
                            outletProductViewModel.CreatedById = exists.CreatedById;
                            outletProductViewModel.UpdatedAt = DateTime.UtcNow;
                            outletProductViewModel.UpdatedById = userId;
                            outletProductViewModel.Id = id;
                            repository.DetachLocal(_ => _.Id == outletProductViewModel.Id);
                            repository.Update(outletProductViewModel);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            throw new NotFoundException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new NotFoundException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.GenericViewModelMandatoryMessage.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (NotFoundException nfe)
            {
                throw new NullReferenceCustomException(nfe.Message, nfe);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<bool> SetMinOnHandInOutletProduct(MinStockHandRequestModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.OutletId == 0)
                        throw new NullReferenceException(ErrorMessages.StoreIdRequired.ToString(CultureInfo.CurrentCulture));

                    if (viewModel.DepartmentId == 0)
                        throw new NullReferenceException(ErrorMessages.DeptId.ToString(CultureInfo.CurrentCulture));

                    if (viewModel.DaysHist > 360) //add discussion with manoj sir
                        throw new NullReferenceException(ErrorMessages.MaximumDays.ToString(CultureInfo.CurrentCulture));

                    var outletRepo = _unitOfWork?.GetRepository<OutletProduct>();
                    var productAll = _unitOfWork?.GetRepository<Product>().GetAll();
                    var outletProductAll = _unitOfWork?.GetRepository<OutletProduct>().GetAll();
                    var transactionAll = _unitOfWork?.GetRepository<Transaction>().GetAll();
                    var repoDepartment = _unitOfWork?.GetRepository<Department>();
                    DateTime fromDate = DateTime.UtcNow.AddDays(-viewModel.DaysHist).Date;
                    DateTime toDate = DateTime.UtcNow.Date;

                    var deptCode = new string[] { "2", "24", "25" };
                    var department = repoDepartment.GetAll().Where(x => !x.IsDeleted && deptCode.Contains(x.Code)).Select(x => x.Id);

                    var query = (from outlet in outletProductAll
                                 join product in productAll on outlet.ProductId equals product.Id
                                 join tran in transactionAll on outlet.ProductId equals tran.ProductId
                                 where outlet.StoreId == tran.OutletId &&
                                 outlet.StoreId == viewModel.OutletId &&
                                 outlet.Status == true &&
                                // product.DepartmentId == viewModel.DepartmentId &&
                                 tran.Type.ToUpper().Equals("ITEMSALE")
                                 && (tran.Date >= fromDate && tran.Date <= toDate)
                                 select new
                                 {
                                     ProductId = outlet.ProductId,
                                     PickingBinNo = outlet.PickingBinNo,
                                     DepartmentId = product.DepartmentId,
                                     UnitQty = product.UnitQty,
                                     Qty = tran.Qty,
                                     PromoSales = tran.PromoSales
                                 });

                    if (viewModel.DepartmentId != null)
                        query = query.Where(x => x.DepartmentId == viewModel.DepartmentId);

                    if (viewModel.ExcludePromo == true)
                        query = query.Where(x => x.PromoSales == 0);

                    var list = await query.GroupBy(x => new { x.ProductId, x.PickingBinNo, x.DepartmentId, x.UnitQty })
                                  .Where(x => x.Sum(y => y.Qty) > 0)
                                  .Select(x => new
                                  {
                                      ProductId = x.Key.ProductId,
                                      PickingBinNo = x.Key.PickingBinNo,
                                      DepartmentId = x.Key.DepartmentId,
                                      UnitQty = x.Key.UnitQty,
                                      Qty = x.Sum(x => x.Qty),
                                  }).OrderBy(x => x.ProductId).ThenBy(x => x.DepartmentId).ToListAsyncSafe().ConfigureAwait(false);

                    if (list.Count == 0)
                        throw new NotFoundException(ErrorMessages.SetMinOnHandNotFound.ToString(CultureInfo.CurrentCulture));

                    foreach (var obj in list)
                    {
                        float daysStock = 3, minNumber = 2, calcMinOnHand = 0, unitsSold = 0, avgDailyUnits = 0, realPickingBinNo = 0;

                        if (obj.UnitQty == 0)
                            unitsSold = obj.Qty * 1;
                        unitsSold = obj.Qty * obj.UnitQty;

                        if (unitsSold > 0)
                            avgDailyUnits = Convert.ToSingle(Math.Round(unitsSold / viewModel.DaysHist, 1));

                        if (department.Any(x => x == obj.DepartmentId))
                        {
                            daysStock = 1;
                            minNumber = 2;
                        }

                        calcMinOnHand = avgDailyUnits * daysStock;
                        if (calcMinOnHand < minNumber)
                        {
                            calcMinOnHand = minNumber;
                        }
                        else
                        {
                            calcMinOnHand = Convert.ToInt32(calcMinOnHand);
                        }

                        if (obj.PickingBinNo.ToString().Length > 0)
                        {
                            realPickingBinNo = Convert.ToSingle(obj?.PickingBinNo ?? 0);
                            if (realPickingBinNo > 0)
                            {
                                realPickingBinNo = realPickingBinNo * 2;
                            }
                            if (realPickingBinNo > calcMinOnHand)
                            {
                                calcMinOnHand = realPickingBinNo;
                            }
                        }
                        List<OutletProduct> outletlist;
                        if (viewModel.LeaveExisting == true)
                            outletlist = outletProductAll.Where(x => x.ProductId == obj.ProductId && x.StoreId == viewModel.OutletId && x.MinOnHand == 0).ToList();
                        else
                            outletlist = outletProductAll.Where(x => x.ProductId == obj.ProductId && x.StoreId == viewModel.OutletId).ToList();

                        foreach (var outlet in outletlist)
                        {
                            outlet.MinOnHand = calcMinOnHand;
                            outletRepo.Update(outlet);
                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                    return true;
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.GenericViewModelMandatoryMessage.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<bool> DeativateProducts(DeactivateProductListRequestModel filter, int id)
        {
            try
            {
                if (filter != null && filter.StoreId > 0)
                {
                    var repo = _unitOfWork.GetRepository<OutletProduct>();
                    var trxRepo = _unitOfWork.GetRepository<Transaction>();
                    var prodRepo = _unitOfWork?.GetRepository<Product>();
                    var trxProds = trxRepo.GetAll(x => x.OutletId == filter.StoreId && x.Date > filter.Date && x.Type == "ITEMSALE").Select(x => x.ProductId);
                    var list = repo.GetAll(x => x.StoreId == filter.StoreId && !x.IsDeleted && x.Status).Include(p => p.Product).ThenInclude(d => d.Department).
                        Include(p => p.Product).ThenInclude(c => c.Commodity).Include(x => x.Supplier).AsQueryable();
                    //get product id for produts that are parents and to be excluded
                    var parentProds = prodRepo.GetAll().Select(x => x.Id);
                    if (filter.QtyOnHandZero)
                    {
                        list = list.Where(x => x.QtyOnHand == 0);
                    }
                    else
                    {
                        list = list.Where(x => x.QtyOnHand != 0);
                    }

                    if (!string.IsNullOrEmpty(filter.DepartmentIdsInc))
                    {
                        list = list.Where(x => filter.DepartmentIdsInc.Contains(x.Product.DepartmentId.ToString()));
                    }
                    if (!string.IsNullOrEmpty(filter.DepartmentIdsExc))
                    {
                        list = list.Where(x => !filter.DepartmentIdsExc.Contains(x.Product.DepartmentId.ToString()));
                    }

                    if (!string.IsNullOrEmpty(filter.CommodityIdsInc))
                    {
                        list = list.Where(x => filter.CommodityIdsInc.Contains(x.Product.CommodityId.ToString()));
                    }
                    if (!string.IsNullOrEmpty(filter.CommodityIdsExc))
                    {
                        list = list.Where(x => !filter.CommodityIdsExc.Contains(x.Product.CommodityId.ToString()));
                    }

                    list = list.Where(x => (x.Product.Parent == null || !parentProds.Contains(x.ProductId)) && (x.Product.Department.Code != "7" || x.Product.Department.Code != "14"));
                    list = list.Where(x => x.Product.Commodity.Code != "933");
                    list = list.Where(x => x.Supplier.Code != "EPAY" || x.Supplier.Code != "EPOTH");
                    list = list.Where(x => !trxProds.Contains(x.ProductId));
                    list = list.OrderBy(x => x.Product.DepartmentId).ThenBy(x => x.Product.Desc);

                    //Update all with status 0
                    foreach (var item in list)
                    {
                        item.Status = false;
                        item.UpdatedAt = DateTime.Now;
                        item.UpdatedById = id;
                    }

                    if (await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                    {
                        return true;
                    }
                    throw new BadRequestException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));

                }
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
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

        /// <summary>
        /// Get List of Products to Deactivate
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<DeactivateProductListResponseModel>>> GetDeactivateProductList(PagedInputModel input, DeactivateProductListRequestModel filter, SecurityViewModel securityViewModel)
        {
            try
            {
                List<DeactivateProductListResponseModel> responseModel = new List<DeactivateProductListResponseModel>();
                int count = 0;
                if (filter != null && filter.StoreId > 0)
                {
                    if (string.IsNullOrEmpty(filter.UserPassword))
                    {
                        throw new BadRequestException(ErrorMessages.InValidPassword.ToString(CultureInfo.CurrentCulture));
                    }

                    var repository = _unitOfWork?.GetRepository<OutletProduct>();
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Date",filter?.Date),
                        new SqlParameter("@StoreId",filter?.StoreId),
                        new SqlParameter("@DepartmentIdsInc",filter?.DepartmentIdsInc),
                        new SqlParameter("@DepartmentIdsExc",filter?.DepartmentIdsExc),
                        new SqlParameter("@CommodityIdsInc" ,filter?.CommodityIdsInc),
                        new SqlParameter("@CommodityIdsExc" ,filter?.CommodityIdsExc),
                        new SqlParameter("@QtyOnHandZero",filter?.QtyOnHandZero),
                        new SqlParameter("@UserId",securityViewModel != null?securityViewModel.UserId:0),
                        new SqlParameter("@UserPassword",filter?.UserPassword),
                };

                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.DeactivateProductList, dbParams.ToArray()).ConfigureAwait(false);

                    if (dset?.Tables?.Count > 0)
                    {
                        if (dset?.Tables?.Count == 1 && Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                        {
                            throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                        }
                        else
                        {
                            responseModel = MappingHelpers.ConvertDataTable<DeactivateProductListResponseModel>(dset.Tables[0]);
                            count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                        }
                    }

                    return new PagedOutputModel<List<DeactivateProductListResponseModel>>(responseModel, count);


                }
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}