using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
using Microsoft.EntityFrameworkCore.Internal;

namespace Coyote.Console.App.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly IUserLoggerServices _userLogger;

        public ProductService(IUnitOfWork unitOfWork, IAutoMappingServices iAutoMapper, IUserLoggerServices userLogger)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = iAutoMapper;
            _userLogger = userLogger;
        }

        /// <summary>
        /// Get all active Products
        /// </summary>
        /// <returns>List<ProductViewModel></returns>
        public async Task<PagedOutputModel<List<ProductResponseModel>>> GetAllActiveProducts(ProductFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Product>();
                var products = repository.GetAll(x => !x.IsDeleted, includes: new Expression<Func<Product, object>>[] { c=>c.CategoryMasterListItem,
                    c=>c.Commodity,
                    c=>c.Department,
                    c=>c.GroupMasterListItem,
                    c=>c.ManufacturerMasterListItem,
                    c=>c.NationalRangeMasterListItem,
                    c=>c.UnitMeasureMasterListItem,
                    c=>c.OutletProductProduct,
                    c=>c.Supplier,
                    c=>c.Tax,
                    c=>c.TypeMasterListItem,
                    c=>c.Department,
                    c=>c.SupplierProduct
                });

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.StoreId)))
                    {
                        var outletrepository = _unitOfWork?.GetRepository<OutletProduct>();

                        var outletProductList = outletrepository.GetAll(x => !x.IsDeleted && x.StoreId.ToString().Equals(inputModel.StoreId))
                            .Include(x => x.Product).ThenInclude(c => c.TypeMasterListItem)
                            .Include(x => x.Product).ThenInclude(c => c.Commodity)
                            .Include(x => x.Product).ThenInclude(c => c.Department)
                            .Include(x => x.Product).ThenInclude(c => c.GroupMasterListItem)
                            .Include(x => x.Product).ThenInclude(c => c.ManufacturerMasterListItem)
                            .Include(x => x.Product).ThenInclude(c => c.NationalRangeMasterListItem)
                            .Include(x => x.Product).ThenInclude(c => c.CategoryMasterListItem)
                            .Include(x => x.Product).ThenInclude(c => c.Supplier)
                                        .Include(x => x.Product).ThenInclude(c => c.OutletProductProduct)
                            .Include(x => x.Product).ThenInclude(c => c.Tax)
                                        .Include(x => x.Product).ThenInclude(c => c.SupplierProduct)
                            .ToList();

                        var productList = outletProductList?.Select(x => x.Product).Where(x => !x.IsDeleted).ToList();

                        products = productList.AsQueryable<Product>();
                    }

                    if (!string.IsNullOrEmpty(inputModel?.Dept))
                        products = products.Where(x => x.Department.Code.ToLower().Contains(inputModel.Dept.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        products = products.Where(x => x.Status);

                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        products = products.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Number.ToString().ToLower().Equals(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Id)))
                        products = products.Where(x => x.Id.ToString().ToLower().Contains(inputModel.Id.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Description)))
                        products = products.Where(x => x.Desc.ToLower().Contains(inputModel.Description.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Number)))
                        products = products.Where(x => x.Number.ToString().ToLower().Equals(inputModel.Number.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Replicate)))
                        products = products.Where(x => x.Replicate.ToLower().Equals(inputModel.Replicate.ToLower()));


                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        products = products.Where(x => !string.IsNullOrWhiteSpace(x.AccessOutletIds) || securityViewModel.StoreIds.Contains(Convert.ToInt32(x.AccessOutletIds)));

                    count = products.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        products = products.Skip(inputModel.SkipCount.Value).Take((int)inputModel.MaxResultCount);

                    products = products.OrderByDescending(x => x.UpdatedAt);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "number":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    products = products.OrderBy(x => x.Number);
                                else
                                    products = products.OrderByDescending(x => x.Number);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    products = products.OrderBy(x => x.Desc);
                                else
                                    products = products.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    products = products.OrderByDescending(x => x.UpdatedAt);
                                else
                                    products = products.OrderBy(x => x.UpdatedAt);
                                break;
                        }
                    }
                }
                List<ProductResponseModel> productsViewModel;

                if (string.IsNullOrEmpty(inputModel?.StoreId))
                {
                    productsViewModel = (await products.ToListAsync().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();
                }
                else
                {
                    productsViewModel = (products.ToList()).Select(MappingHelpers.CreateMap).ToList();
                }
                //If Supplier Item is Required
                if (!string.IsNullOrEmpty((inputModel?.SupplierId)))
                {
                    foreach (var item in productsViewModel)
                    {
                        products = products.Include(c => c.SupplierProduct);
                        item.SupplierProductItem = products.Where(x => x.Id == item.Id).FirstOrDefault().SupplierProduct.Where(x => x.SupplierId.ToString().ToLower().Contains(inputModel.SupplierId.ToLower())).Select(x => x.SupplierItem).FirstOrDefault();

                        item.SupplierProductId = products.Where(x => x.Id == item.Id).FirstOrDefault().SupplierProduct.Where(x => x.SupplierId.ToString().ToLower().Contains(inputModel.SupplierId.ToLower())).Select(x => x.Id).FirstOrDefault();
                    }
                }

                foreach (var item in productsViewModel)
                {

                    item.ChildrenProductCount = repository.GetAll(x => x.Parent == item.Number && !x.IsDeleted).Count();
                }
                return new PagedOutputModel<List<ProductResponseModel>>(productsViewModel, count);

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
        /// Get all active Products
        /// </summary>
        /// <returns>List<ProductViewModel></returns>
        public async Task<PagedOutputModel<List<ProductResponseModel>>> GetActiveProducts(ProductFilter inputModel, SecurityViewModel securityViewModel)
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

                var repository = _unitOfWork?.GetRepository<Product>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                new SqlParameter("@Id", inputModel?.Id),
                new SqlParameter("@Status", inputModel?.Status),
                new SqlParameter("@Description", inputModel?.Description),
                new SqlParameter("@Number", inputModel?.Number),
                new SqlParameter("@DeptId", inputModel?.Dept),
                new SqlParameter("@Replicate", inputModel?.Replicate),
                new SqlParameter("@StoreId", inputModel?.StoreId),
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@SupplierId", inputModel?.SupplierId),
                new SqlParameter("@SkipCount", inputModel?.SkipCount),
                new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                new SqlParameter("@AccessOutletIds", (StoreIds == true)?AccessStores:null),
                new SqlParameter("@IsWithoutAPNCall", IsRequired.False),
                new SqlParameter("@IsLogged", inputModel?.IsLogged),
                new SqlParameter("@Module","Product"),
                new SqlParameter("@RoleId",RoleId)
            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveProducts, dbParams.ToArray()).ConfigureAwait(false);
                List<ProductResponseModel> productsViewModel = MappingHelpers.ConvertDataTable<ProductResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<ProductResponseModel>>(productsViewModel, count);
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
        /// Get all active Products
        /// </summary>
        /// <returns>List<ProductViewModel></returns>
        public async Task<PagedOutputModel<List<ProductResponseModel>>> GetActiveReplicateProducts(ProductFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                //bool StoreIds = false;
                //var AccessStores = String.Empty;
                //if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                //{
                //    foreach (var storeId in securityViewModel.StoreIds)
                //        AccessStores += storeId + ",";
                //    StoreIds = true;
                //}

                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                var repository = _unitOfWork?.GetRepository<Product>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@Number", inputModel?.Number),

            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllReplicateProduct, dbParams.ToArray()).ConfigureAwait(false);
                List<ProductResponseModel> productsViewModel = MappingHelpers.ConvertDataTable<ProductResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<ProductResponseModel>>(productsViewModel, count);
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
        /// get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductResponseModel> GetProductById(long id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Product>();

                    var product = await repository.GetAll(x => x.Id == id && !x.IsDeleted
                    ).Include(c => c.GroupMasterListItem)
                    .Include(c => c.APNProduct).Where(c => !c.IsDeleted)
                    .Include(c => c.CategoryMasterListItem)
                              .Include(c => c.OutletProductProduct)
                    .Include(c => c.NationalRangeMasterListItem)
                    .Include(c => c.Department)
                    .Include(c => c.Supplier)
                    .Include(c => c.Commodity)
                    .Include(c => c.Tax)
                    .Include(c => c.TypeMasterListItem)
                    .Include(c => c.ManufacturerMasterListItem)
                    .Include(c => c.UnitMeasureMasterListItem)
                    .Include(c => c.SupplierProduct)
                            .ThenInclude(c => c.Supplier)
                    .Include(c => c.PromotionSellingchProduct)
                            .ThenInclude(c => c.Promotion)
                                .ThenInclude(q => q.PromotionType)
                    .Include(c => c.PromotionBuying)
                            .ThenInclude(c => c.Promotion)
                                .ThenInclude(q => q.PromotionType)
                    .Include(c => c.PromotionMemberOffer)
                            .ThenInclude(c => c.Promotion)
                                .ThenInclude(q => q.PromotionType)
                    .Include(c => c.PromotionMixmatchProduct)
                            .ThenInclude(c => c.PromotionMixmatch)
                                    .ThenInclude(c => c.Promotion)
                                        .ThenInclude(q => q.PromotionType)
                    .Include(c => c.PromotionOfferProduct)
                            .ThenInclude(c => c.PromotionOffer)
                                .ThenInclude(q => q.Promotion).ThenInclude(c => c.PromotionType)
                    .Include(c => c.PromotionCompetitionProduct)
                            .ThenInclude(c => c.CompetitionDetail)
                                .ThenInclude(c => c.Promotion)
                                    .ThenInclude(q => q.PromotionType)

                    .FirstOrDefaultAsync().ConfigureAwait(false);

                    if (product == null)
                    {
                        throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    #region Get Promotion
                    var response = new List<Promotion>();

                    var promotion = product.PromotionMixmatchProduct?.Where(x => !x.IsDeleted).Select(x => x.PromotionMixmatch).Where(x => !x.IsDeleted).Select(x => x.Promotion).Where(x => !x.IsDeleted).ToList();
                    if (promotion != null)
                        response.AddRange(promotion);

                    promotion = product.PromotionOfferProduct?.Where(x => !x.IsDeleted).Select(x => x.PromotionOffer)?.Where(x => !x.IsDeleted).Select(x => x.Promotion).Where(x => !x.IsDeleted).ToList();
                    if (promotion != null)
                        response.AddRange(promotion);

                    promotion = product.PromotionCompetitionProduct?.Where(x => !x.IsDeleted).Select(x => x.CompetitionDetail)?.Where(x => !x.IsDeleted).Select(x => x.Promotion).ToList();
                    if (promotion != null)
                        response.AddRange(promotion);


                    promotion = product.PromotionBuying?.Where(x => !x.IsDeleted).Select(x => x.Promotion).Where(x => !x.IsDeleted).ToList();
                    if (promotion != null)
                        response.AddRange(promotion);


                    promotion = product.PromotionSellingchProduct?.Where(x => !x.IsDeleted).Select(x => x.Promotion).Where(x => !x.IsDeleted).ToList();

                    if (promotion != null)
                        response.AddRange(promotion);


                    promotion = product.PromotionMemberOffer?.Where(x => !x.IsDeleted).Select(x => x.Promotion).Where(x => !x.IsDeleted).ToList();
                    if (promotion != null)
                        response.AddRange(promotion);

                    var responsePromotion = response.Select(MappingHelpers.CreateMap);

                    #endregion

                    ProductResponseModel productVM;
                    productVM = MappingHelpers.CreateMap(product);


                    productVM.Promotions.AddRange(responsePromotion);

                    if (!string.IsNullOrEmpty(product.AccessOutletIds))
                    {
                        var accessOutlets = product.AccessOutletIds.Split(',').ToList();
                        foreach (var outletId in accessOutlets)
                        {
                            productVM.AccessOutlets.Add(Convert.ToInt32(outletId));
                        }
                    }

                    if (product.APNProduct != null)
                    {

                        foreach (var apn in product.APNProduct.Where(x => !x.IsDeleted))
                        {
                            productVM.APNNumbers.Add(apn.Number);
                        }
                    }

                    if (product.Parent != null && product.Parent > 0)
                    {
                        var parent = await repository.GetAll(x => x.Number == product.Parent && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (parent != null)
                        {
                            productVM.Parent = parent.Number;
                            productVM.ParentCartonQty = parent.CartonQty;
                            productVM.ParentDesc = parent.Desc;
                        }
                    }

                    var warehouse = await GetWarehouseCodes().ConfigureAwait(false);
                    productVM.HostCodes.AddRange(warehouse.HostCodes);

                    return productVM;
                }
                throw new NullReferenceException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// <summary>
        /// get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductResponseModel> GetByProductId(long id, PagedInputModel inputModel, bool fetchListItems = true)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Product>();
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@ProductId", id),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@IsListItemsRequired", fetchListItems),
                    };
                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetProductById, dbParams.ToArray()).ConfigureAwait(false);

                    ProductResponseModel productVM = MappingHelpers.ConvertDataTable<ProductResponseModel>(dset.Tables[0]).FirstOrDefault();

                    if (productVM == null)
                    {
                        throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var supplierProductList = MappingHelpers.ConvertDataTable<SupplierProductResponseViewModel>(dset.Tables[1])?.ToList();
                    productVM.SupplierProducts.AddRange(supplierProductList);
                    productVM.SupplierProductsCount = supplierProductList.Count;
                    productVM.Promotions.AddRange(MappingHelpers.ConvertDataTable<PromotionResponseViewModel>(dset.Tables[2])?.ToList().OrderByDescending(x => x.UpdatedAt));
                    foreach (DataRow item in dset.Tables[3].Rows)
                    {
                        productVM.APNNumbers.Add(Convert.ToInt64(item["Number"]));
                    }

                    foreach (DataRow item in dset.Tables[4].Rows)
                    {
                        productVM.HostCodes.Add(Convert.ToString(item["Code"]));
                    }

                    foreach (DataRow item in dset.Tables[5]?.Rows)
                    {
                        productVM.AccessOutlets.Add(Convert.ToInt32(item["AccessOutlets"]));
                    }
                    if (fetchListItems)
                    {
                        productVM.CategoryList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[6])?.ToList());
                        productVM.GroupList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[7])?.ToList());
                        productVM.ManufacturerList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[8])?.ToList());
                        productVM.NationalRangeList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[9])?.ToList());
                        productVM.TypeList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[10])?.ToList());
                        productVM.UnitMeasureList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[11])?.ToList());
                        productVM.CommodityList.AddRange(MappingHelpers.ConvertDataTable<CommodityResponseModel>(dset.Tables[12])?.ToList());
                        productVM.DepartmentList.AddRange(MappingHelpers.ConvertDataTable<DepartmentResponseModel>(dset.Tables[13])?.ToList());
                        productVM.SupplierList.AddRange(MappingHelpers.ConvertDataTable<SupplierResponseViewModel>(dset.Tables[14])?.ToList());
                        productVM.TaxList.AddRange(MappingHelpers.ConvertDataTable<TaxResponseModel>(dset.Tables[15])?.ToList());
                        productVM.ProductChildrenList.AddRange(MappingHelpers.ConvertDataTable<ProductChildModel>(dset.Tables[16])?.ToList());
                        productVM.OutletProducts.AddRange(MappingHelpers.ConvertDataTable<OutletProductResponseViewModel>(dset.Tables[17])?.ToList());
                    }
                    return productVM;
                }
                throw new NullReferenceException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<ProductResponseModel> Insert(ProductRequestModel viewModel, int userId)
        {
            ProductResponseModel responseModel = new ProductResponseModel();
            try
            {
                var repository = _unitOfWork.GetRepository<Product>();
                if (viewModel != null)
                {
                    if (viewModel.DepartmentId > 0)
                    {
                        if (await repository.GetAll(x => x.Number == viewModel.Number).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.ProductDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        //Checking All APNs.
                        #region Check APN
                        var apnRepository = _unitOfWork.GetRepository<APN>();
                        if (viewModel.APNNumber != null)
                        {
                            if (viewModel.APNNumber.Distinct().Count() != viewModel.APNNumber.Count)
                            {
                                //can't add same APN twice
                                throw new AlreadyExistsException(ErrorMessages.APNNumberDuplicate.ToString(CultureInfo.CurrentCulture));
                            }

                            foreach (var apn in viewModel.APNNumber)
                            {
                                if (await apnRepository.GetAll(x => x.Number == apn && !x.IsDeleted && x.ProductId != 0).AnyAsync().ConfigureAwait(false))
                                {
                                    throw new AlreadyExistsException(ErrorMessages.APNNumbDuplicate.ToString(CultureInfo.CurrentCulture) + $" : {apn}");
                                }
                            }
                        }
                        #endregion
                        var supplierRepo = _unitOfWork?.GetRepository<Supplier>();

                        //Checking all Outlet Products
                        #region Check outlet Products
                        if (viewModel.OutletProduct != null && viewModel.OutletProduct.Count > 0)
                        {
                            foreach (var outProd in viewModel.OutletProduct)
                            {
                                //Check if store Id exists
                                if (outProd.StoreId <= 0)
                                {
                                    throw new BadRequestException(ErrorMessages.OutletProductStore.ToString(CultureInfo.CurrentCulture));
                                }

                                //Checkk if storeId is valid
                                var storeRepo = _unitOfWork?.GetRepository<Store>();
                                if (!await storeRepo.GetAll(x => x.Id == outProd.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                                {
                                    throw new BadRequestException(ErrorMessages.OutletProductStore.ToString(CultureInfo.CurrentCulture));

                                }

                                //If supplier is 0, change it to null
                                if (outProd.SupplierId == 0)
                                {
                                    outProd.SupplierId = null;
                                }

                                //If not, check if supplierId is valid or not
                                if (outProd.SupplierId > 0)
                                {
                                    if (!await supplierRepo.GetAll(x => x.Id == outProd.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                                    {
                                        throw new BadRequestException(ErrorMessages.OutletProductSupplier.ToString(CultureInfo.CurrentCulture));

                                    }
                                }
                            }
                        }
                        #endregion

                        //Checking Supplier Products
                        #region Check Supplier Products
                        if (viewModel.SupplierProduct != null && viewModel.SupplierProduct.Count > 0)
                        {
                            if (viewModel.SupplierProduct.Select(x => x.SupplierId).Distinct().Count() != viewModel.SupplierProduct.Count)
                            {
                                //can't add same product twice
                                throw new BadRequestException(ErrorMessages.ProductSupplierDuplicate.ToString(CultureInfo.CurrentCulture));
                            }

                            foreach (var suppProd in viewModel.SupplierProduct)
                            {
                                //Check if supplierId is null or Invalid

                                if (suppProd != null)
                                {
                                    if (suppProd.SupplierId > 0)
                                    {
                                        if (!(await supplierRepo.GetAll(x => x.Id == suppProd.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                                        {
                                            throw new BadRequestException(ErrorMessages.ProductSupplier.ToString(CultureInfo.CurrentCulture));
                                        }
                                    }
                                    else
                                    {
                                        throw new NullReferenceCustomException(ErrorMessages.ProductSupplier.ToString(System.Globalization.CultureInfo.CurrentCulture));
                                    }
                                }
                            }
                        }
                        #endregion

                        var deptRepo = _unitOfWork?.GetRepository<Department>();
                        if (!await deptRepo.GetAll(x => x.Id == viewModel.DepartmentId && x.IsDeleted == false).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.ProductDepartment.ToString(CultureInfo.CurrentCulture));
                        }

                        if (viewModel.Parent != null && viewModel.Parent > 0)
                        {
                            if (!await repository.GetAll(x => x.Number == viewModel.Parent && x.IsDeleted == false).AnyAsync().ConfigureAwait(false))
                            {
                                throw new AlreadyExistsException(ErrorMessages.ProductParent.ToString(CultureInfo.CurrentCulture));
                            }
                            if (viewModel.Number == viewModel.Parent)
                            {
                                throw new AlreadyExistsException(ErrorMessages.ProductParentSame.ToString(CultureInfo.CurrentCulture));
                            }
                        }


                        var product = _iAutoMapper.Mapping<ProductRequestModel, Product>(viewModel);

                        if (viewModel.AccessOutletIds != null)
                        {
                            product.AccessOutletIds = null;
                            foreach (var outletId in viewModel.AccessOutletIds)
                                product.AccessOutletIds += outletId + ",";
                        }



                        if (!string.IsNullOrEmpty(product.AccessOutletIds))
                            product.AccessOutletIds = product.AccessOutletIds.Substring(0, product.AccessOutletIds.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));
                        //product.Number = await GetProductNumber().ConfigureAwait(false);
                        product.IsDeleted = false;
                        product.CreatedAt = DateTime.UtcNow;
                        product.UpdatedAt = DateTime.UtcNow;
                        product.CreatedById = userId;
                        product.UpdatedById = userId;
                        var result = await repository.InsertAsync(product).ConfigureAwait(false);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        if (result != null)
                        {
                            if (result.Id > 0)
                            {
                                if (viewModel.OutletProduct != null)
                                {
                                    await SaveOutletProductsAsync(viewModel.OutletProduct, result.Id, userId).ConfigureAwait(false);
                                }
                                if (viewModel.SupplierProduct != null)
                                {
                                    await SaveSupplierProductsAsync(viewModel.SupplierProduct, result.Id, userId).ConfigureAwait(false);
                                }

                                if (viewModel.APNNumber != null)
                                {
                                    await SaveProductAPN(viewModel.APNNumber, result.Id, userId, result.Desc).ConfigureAwait(false);

                                }

                                responseModel = await GetProductById(result.Id).ConfigureAwait(false);

                                //Adding To USerLog
                                var logModel = new UserLogRequestModel<ProductResponseModel>
                                {
                                    Action = (ActionPerformed.Create).ToString(),
                                    Module = "Product",
                                    Table = "Product",
                                    TableId = result.Id,
                                    NewData = responseModel,
                                    ActionBy = userId
                                };

                                await _userLogger.Insert(logModel).ConfigureAwait(false);


                            }

                        }
                        return responseModel;
                    }
                    else
                    {
                        throw new NullReferenceCustomException(ErrorMessages.DeptId.ToString(CultureInfo.CurrentCulture));
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
            return responseModel;
        }

        public async Task<ProductResponseModel> Update(ProductRequestModel viewModel, long productId, int userId, string imagePath = null)
        {
            try
            {
                ProductResponseModel responseModel = new ProductResponseModel();
                var repository = _unitOfWork?.GetRepository<Product>();
                if (viewModel != null)
                {
                    if (productId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetById(productId).ConfigureAwait(false);
                    var oldData = await GetProductById(productId).ConfigureAwait(false);
                    if (exists != null)
                    {

                        //Check APN

                        #region Check APN
                        var apnRepository = _unitOfWork.GetRepository<APN>();
                        if (viewModel.APNNumber != null)
                        {
                            foreach (var apn in viewModel.APNNumber)
                            {
                                if (viewModel.APNNumber.Distinct().Count() != viewModel.APNNumber.Count)
                                {
                                    //can't add same APN twice
                                    throw new AlreadyExistsException(ErrorMessages.APNNumberDuplicate.ToString(CultureInfo.CurrentCulture));
                                }

                                if (await apnRepository.GetAll(x => x.Number == apn && !x.IsDeleted && x.ProductId != productId).AnyAsync().ConfigureAwait(false))
                                {
                                    throw new AlreadyExistsException(ErrorMessages.APNNumbDuplicate.ToString(CultureInfo.CurrentCulture) + $" : {apn}");
                                }
                            }
                        }

                        #endregion

                        var supplierRepo = _unitOfWork?.GetRepository<Supplier>();

                        //Checking all Outlet Products
                        #region check Outlet Products

                        if (viewModel.OutletProduct != null && viewModel.OutletProduct.Count > 0)
                        {
                            foreach (var outProd in viewModel.OutletProduct)
                            {
                                //Check if store Id exists
                                if (outProd.StoreId <= 0)
                                {
                                    throw new BadRequestException(ErrorMessages.OutletProductStore.ToString(CultureInfo.CurrentCulture));
                                }

                                //Checkk if storeId is valid
                                var storeRepo = _unitOfWork?.GetRepository<Store>();
                                if (!await storeRepo.GetAll(x => x.Id == outProd.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                                {
                                    throw new BadRequestException(ErrorMessages.OutletProductStore.ToString(CultureInfo.CurrentCulture));

                                }

                                //If supplier is 0, change it to null
                                if (outProd.SupplierId == 0)
                                {
                                    outProd.SupplierId = null;
                                }

                                //If not, check if supplierId is valid or not
                                if (outProd.SupplierId > 0)
                                {
                                    if (!await supplierRepo.GetAll(x => x.Id == outProd.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                                    {
                                        throw new BadRequestException(ErrorMessages.OutletProductSupplier.ToString(CultureInfo.CurrentCulture));

                                    }
                                }
                            }
                        }

                        #endregion


                        //Checking Supplier Products
                        #region Check Supplier Products

                        if (viewModel.SupplierProduct != null && viewModel.SupplierProduct.Count > 0)
                        {
                            if (viewModel.SupplierProduct.Select(x => x.SupplierId).Distinct().Count() != viewModel.SupplierProduct.Count)
                            {
                                //can't add same product twice
                                throw new BadRequestException(ErrorMessages.ProductSupplierDuplicate.ToString(CultureInfo.CurrentCulture));
                            }

                            foreach (var suppProd in viewModel.SupplierProduct)
                            {
                                //Check if supplierId is null or Invalid

                                if (suppProd != null)
                                {
                                    if (suppProd.SupplierId > 0)
                                    {
                                        if (!(await supplierRepo.GetAll(x => x.Id == suppProd.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                                        {
                                            throw new BadRequestException(ErrorMessages.ProductSupplier.ToString(CultureInfo.CurrentCulture));
                                        }
                                    }
                                    else
                                    {
                                        throw new NullReferenceCustomException(ErrorMessages.ProductSupplier.ToString(System.Globalization.CultureInfo.CurrentCulture));
                                    }
                                }
                            }
                        }

                        #endregion

                        if (viewModel.Parent != null && viewModel.Parent > 0)
                        {
                            if (!await repository.GetAll(x => x.Number == viewModel.Parent && x.IsDeleted == false).AnyAsync().ConfigureAwait(false))
                            {
                                throw new AlreadyExistsException(ErrorMessages.ProductParent.ToString(CultureInfo.CurrentCulture));
                            }
                            if (viewModel.Number == viewModel.Parent)
                            {
                                throw new AlreadyExistsException(ErrorMessages.ProductParentSame.ToString(CultureInfo.CurrentCulture));
                            }
                        }


                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        var product = _iAutoMapper.Mapping<ProductRequestModel, Product>(viewModel);

                        product.AccessOutletIds = null;
                        if (viewModel?.AccessOutletIds != null)
                        {
                            foreach (var outletId in viewModel.AccessOutletIds)
                                product.AccessOutletIds += outletId + ",";
                        }

                        if (!string.IsNullOrEmpty(product.AccessOutletIds))
                            product.AccessOutletIds = product.AccessOutletIds.Substring(0, product.AccessOutletIds.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));

                        product.Number = exists.Number;
                        product.ImagePath = imagePath;
                        product.Id = productId;
                        product.IsDeleted = false;
                        product.CreatedAt = exists.CreatedAt;
                        product.CreatedById = exists.CreatedById;
                        product.UpdatedById = userId;

                        repository.DetachLocal(_ => _.Id == product.Id);
                        repository.Update(product);
                        var result = await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                        if (result && product.Id > 0)
                        {
                            if (viewModel.OutletProduct != null)
                            {
                                await SaveOutletProductsAsync(viewModel.OutletProduct, product.Id, userId).ConfigureAwait(false);
                            }

                            if (viewModel.SupplierProduct != null)
                            {
                                await SaveSupplierProductsAsync(viewModel.SupplierProduct, product.Id, userId).ConfigureAwait(false);
                            }

                            if (viewModel.APNNumber != null)
                            {
                                await SaveProductAPN(viewModel.APNNumber, product.Id, userId, product.Desc).ConfigureAwait(false);
                            }

                            //if (product.Status == false)
                            //{
                            //Deactivate all modules related to product
                            await DeactivateProduct(productId, userId, product.Status).ConfigureAwait(false);
                            // }

                            responseModel = await GetProductById(product.Id).ConfigureAwait(false);

                            //Adding To USerLog
                            var logModel = new UserLogRequestModel<ProductResponseModel>
                            {
                                Action = (ActionPerformed.Update).ToString(),
                                Module = "Product",
                                Table = "Product",
                                TableId = product.Id,
                                NewData = responseModel,
                                OldData = oldData,
                                ActionBy = userId
                            };

                            await _userLogger.Insert(logModel).ConfigureAwait(false);

                        }

                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));

                }
                throw new NullReferenceException();
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

        public async Task<bool> Delete(long productId, int userId)
        {
            try
            {
                if (productId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Product>();
                    var productExists = await repository.GetAll(x => x.Id == productId)//.Include(c => c.OutletProductProduct)
                        .Include(c => c.APNProduct)
                        .Include(c => c.SupplierProduct)
                        //  .Include(c => c.OutletProductProduct)
                        .Include(c => c.PromotionBuying)
                        .Include(c => c.PromotionCompetitionProduct)
                        .Include(c => c.PromotionMemberOffer)
                        .Include(c => c.PromotionMixmatchProduct)
                        .Include(c => c.PromotionOfferProduct)
                        .Include(c => c.PromotionSellingchProduct)
                        .Include(c => c.OrderDetailProduct)
                        .Include(c => c.StockAdjustDetailProduct)
                        .Include(c => c.StockTakeDetailProduct)
                        .Include(c => c.OrderDetailProduct)

                        .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (productExists != null)
                    {

                        var OldData = MappingHelpers.Mapping<Product, ProductResponseModel>(productExists);
                        //Delete all children as well.

                        var apnList = productExists.APNProduct?.ToList();
                        foreach (var apn in apnList)
                        {
                            apn.IsDeleted = true;
                            apn.UpdatedById = userId;
                        }

                        var supplierList = productExists.SupplierProduct?.ToList();
                        foreach (var supplier in supplierList)
                        {
                            supplier.IsDeleted = true;
                            supplier.UpdatedById = userId;
                        }

                       // var outletProdList = productExists.OutletProductProduct?.ToList();

                        List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@ProductId", productId),
                        new SqlParameter("@UserId", userId)
                    };
                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.DeleteOutletProductByProductId, dbParams.ToArray()).ConfigureAwait(false);
                       
                        var buyPromo = productExists.PromotionBuying?.ToList();
                        foreach (var item in buyPromo)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }

                        var sellPromo = productExists.PromotionSellingchProduct?.ToList();
                        foreach (var item in sellPromo)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }
                        var offerPromo = productExists.PromotionOfferProduct?.ToList();
                        foreach (var item in offerPromo)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }
                        var mixmatchPromo = productExists.PromotionMixmatchProduct?.ToList();
                        foreach (var item in mixmatchPromo)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }

                        var memberPromo = productExists.PromotionMemberOffer?.ToList();
                        foreach (var item in memberPromo)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }

                        var compPromo = productExists.PromotionCompetitionProduct?.ToList();
                        foreach (var item in compPromo)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }

                        var stockTake = productExists.StockTakeDetailProduct?.ToList();
                        foreach (var item in stockTake)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }

                        var stockAdjust = productExists.StockAdjustDetailProduct?.ToList();
                        foreach (var item in stockAdjust)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }


                        var orderDetail = productExists.OrderDetailProduct?.ToList();
                        foreach (var item in orderDetail)
                        {
                            item.IsDeleted = true;
                            item.UpdatedById = userId;
                        }

                        if (!productExists.IsDeleted)
                        {
                            productExists.UpdatedById = userId;
                            productExists.IsDeleted = true;

                            repository?.Update(productExists);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);


                            //Adding To USerLog
                            var logModel = new UserLogRequestModel<ProductResponseModel>
                            {
                                Action = (ActionPerformed.Update).ToString(),
                                Module = "Product",
                                Table = "Product",
                                TableId = productExists.Id,
                                OldData = OldData,
                                ActionBy = userId
                            };
                            await _userLogger.Insert(logModel).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
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

        public async Task<ProductNumberModel> GetNewProductNumber(string Extend = null)
        {

            long newProductNo = 0;
            try
            {
                var repository = _unitOfWork?.GetRepository<Product>();

                var Products = repository.GetAll().Select(x => x.Number);

                var ProdCount = Products.Count();
                long number = 0;
                if (ProdCount > 0)
                {
                    number = await repository.GetAll().Select(x => x.Number).MaxAsync().ConfigureAwait(false);
                }
                else
                {
                    number = 1;
                }
                var Max = await Products.ToListAsync().ConfigureAwait(false);


                newProductNo = number++;
                while (await repository.GetAll(x => x.Number == newProductNo).AnyAsync().ConfigureAwait(false))
                {
                    newProductNo = newProductNo + 1;
                }
            }
            catch (Exception ex)
            {
                throw new ArithmeticException(ex.Message);
            }

            ProductNumberModel prodNumer = await GetWarehouseCodes().ConfigureAwait(false);

            prodNumer.Number = newProductNo;

            if (!string.IsNullOrEmpty(Extend))
            {

                var masterListRepo = _unitOfWork.GetRepository<MasterList>();
                var masterRepo = _unitOfWork.GetRepository<MasterListItems>();

                //Add Unit Measures
                var UnitMeasureCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.UnitMeasure).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                var unitMeasures = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == UnitMeasureCode).ToListAsync().ConfigureAwait(false);

                var unitMeasureList = unitMeasures.Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>);

                prodNumer.UnitMeasureList.AddRange(unitMeasureList);


                //Add Category
                var CategoryCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.Category).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                var categories = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == CategoryCode).ToListAsync().ConfigureAwait(false);

                var categoriesList = categories.Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>);

                prodNumer.CategoryList.AddRange(categoriesList);


                //Add NationalRange
                var NationalRangeCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.NationalRange).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                var nationalRange = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == NationalRangeCode).ToListAsync().ConfigureAwait(false);

                var nationalRangeList = nationalRange.Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>);

                prodNumer.NationalRangeList.AddRange(nationalRangeList);


                //Add Group
                var GroupCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.ProductGroup).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                var groups = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == GroupCode).Take(CommonMessages.MasterListMaxCount).ToListAsync().ConfigureAwait(false);

                var groupsList = groups.Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>);

                prodNumer.GroupList.AddRange(groupsList);


                //Add Type
                var TypeCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.ProductType).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                var types = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == TypeCode).ToListAsync().ConfigureAwait(false);

                var typesList = types.Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>);

                prodNumer.TypeList.AddRange(typesList);

                //Add Manufacturer
                var ManufacturerCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.ProductType).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                var manufacturers = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == ManufacturerCode).Take(CommonMessages.MasterListMaxCount).ToListAsync().ConfigureAwait(false);

                var manufacturersList = manufacturers.Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>);

                prodNumer.ManufacturerList.AddRange(manufacturersList);


                //Add Tax

                var taxRepo = _unitOfWork.GetRepository<Tax>();

                var taxes = await taxRepo.GetAll(x => !x.IsDeleted).Take(CommonMessages.MasterListMaxCount).ToListAsync().ConfigureAwait(false);

                var taxList = taxes.Select(MappingHelpers.Mapping<Tax, TaxResponseModel>);

                prodNumer.TaxList.AddRange(taxList);


                //Add Commodity

                var comRepo = _unitOfWork.GetRepository<Commodity>();

                var commodities = await comRepo.GetAll(x => !x.IsDeleted).Take(CommonMessages.MasterListMaxCount).ToListAsync().ConfigureAwait(false);

                var comList = commodities.Select(MappingHelpers.Mapping<Commodity, CommodityResponseModel>);

                prodNumer.CommodityList.AddRange(comList);

                //Add Department
                var deptRepo = _unitOfWork.GetRepository<Department>();
                var department = await deptRepo.GetAll(x => !x.IsDeleted).Take(CommonMessages.MasterListMaxCount).ToListAsync().ConfigureAwait(false);

                var deptList = department.Select(MappingHelpers.Mapping<Department, DepartmentResponseModel>);

                prodNumer.DepartmentList.AddRange(deptList);

                //Add Supplier
                var supplierRepo = _unitOfWork.GetRepository<Supplier>();
                var suppliers = await supplierRepo.GetAll(x => !x.IsDeleted).Take(CommonMessages.MasterListMaxCount).ToListAsync().ConfigureAwait(false);

                var supplierList = suppliers.Select(MappingHelpers.Mapping<Supplier, SupplierResponseViewModel>);

                prodNumer.SupplierList.AddRange(supplierList);
            }

            return prodNumer;
        }
        public async Task<ProductNumberModel> GetNewProductNo(PagedInputModel inputModel, string Extend = null)
        {

            var repository = _unitOfWork?.GetRepository<Product>();
            List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.MaxResultCount),
                    };
            var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetNewProductNumber, dbParams.ToArray()).ConfigureAwait(false);

            ProductNumberModel productNumberVM = MappingHelpers.ConvertDataTable<ProductNumberModel>(dset.Tables[0]).FirstOrDefault();
            productNumberVM.Number = Convert.ToInt64(dset.Tables[0].Rows[0]["Number"]);
            productNumberVM.CategoryList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[1])?.ToList());
            productNumberVM.GroupList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[2])?.ToList());
            productNumberVM.ManufacturerList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[3])?.ToList());
            productNumberVM.NationalRangeList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[4])?.ToList());
            productNumberVM.TypeList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[5])?.ToList());
            productNumberVM.UnitMeasureList.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[6])?.ToList());
            productNumberVM.CommodityList.AddRange(MappingHelpers.ConvertDataTable<CommodityResponseModel>(dset.Tables[7])?.ToList());
            productNumberVM.DepartmentList.AddRange(MappingHelpers.ConvertDataTable<DepartmentResponseModel>(dset.Tables[8])?.ToList());
            productNumberVM.SupplierList.AddRange(MappingHelpers.ConvertDataTable<SupplierResponseViewModel>(dset.Tables[9])?.ToList());
            productNumberVM.TaxList.AddRange(MappingHelpers.ConvertDataTable<TaxResponseModel>(dset.Tables[10])?.ToList());

            return productNumberVM;
        }
        public async Task<bool> SaveOutletProductsAsync(List<OutletProductRequestModel> viewModels, long productId, int userId)
        {
            var outletProdRepository = _unitOfWork?.GetRepository<OutletProduct>();

            try
            {
                if (viewModels != null && viewModels.Count > 0)
                {
                    foreach (var outProd in viewModels)
                    {
                        if (outProd.StoreId > 0)
                        {
                            var exists = await outletProdRepository.GetAll(x => !x.IsDeleted && x.ProductId == productId && x.StoreId == outProd.StoreId).FirstOrDefaultAsync().ConfigureAwait(false);
                            if (exists != null)
                            {
                                var oldData = MappingHelpers.Mapping<OutletProduct, OutletProductResponseViewModel>(exists);

                                var outletProductViewModel = _iAutoMapper.Mapping<OutletProductRequestModel, OutletProduct>(outProd);
                                outletProductViewModel.IsDeleted = false;
                                outletProductViewModel.CreatedAt = exists.CreatedAt;
                                outletProductViewModel.CreatedById = exists.CreatedById;
                                outletProductViewModel.UpdatedAt = DateTime.UtcNow;
                                outletProductViewModel.UpdatedById = userId;
                                outletProductViewModel.Id = exists.Id;

                                outletProdRepository.DetachLocal(_ => _.Id == exists.Id);
                                outletProdRepository.Update(outletProductViewModel);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                                //Adding To USerLog
                                var logModel = new UserLogRequestModel<OutletProductResponseViewModel>
                                {
                                    Action = (ActionPerformed.Update).ToString(),
                                    Module = "Product",
                                    Table = "OutletProduct",
                                    TableId = outletProductViewModel.ProductId,
                                    NewData = MappingHelpers.Mapping<OutletProduct, OutletProductResponseViewModel>(outletProductViewModel),
                                    OldData = oldData,
                                    ActionBy = userId
                                };
                                await _userLogger.Insert(logModel).ConfigureAwait(false);
                            }
                            else
                            {
                                outProd.ProductId = productId;
                                var outletProduct = _iAutoMapper.Mapping<OutletProductRequestModel, OutletProduct>(outProd);
                                outletProduct.IsDeleted = false;
                                outletProduct.CreatedAt = DateTime.UtcNow;
                                outletProduct.UpdatedAt = DateTime.UtcNow;
                                outletProduct.CreatedById = userId;
                                outletProduct.UpdatedById = userId;
                                var result = await outletProdRepository.InsertAsync(outletProduct).ConfigureAwait(false);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                //Adding To USerLog
                                var logModel = new UserLogRequestModel<OutletProductResponseViewModel>
                                {
                                    Action = (ActionPerformed.Create).ToString(),
                                    Module = "Product",
                                    Table = "OutletProduct",
                                    TableId = result.ProductId,
                                    NewData = MappingHelpers.Mapping<OutletProduct, OutletProductResponseViewModel>(outletProduct),
                                    ActionBy = userId
                                };
                                await _userLogger.Insert(logModel).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                }
                return true;
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

        }

        public async Task<bool> SaveSupplierProductsAsync(List<SupplierProductRequestModel> viewModels, long productId, int userId)
        {
            var supplierProdRepository = _unitOfWork?.GetRepository<SupplierProduct>();

            try
            {

                #region Delete SupplierProducts
                // Delete SupplierProducts not in list
                var existingSupplierProd = await supplierProdRepository.GetAll(x => x.ProductId == productId && !x.IsDeleted).ToListAsync().ConfigureAwait(false);

                if (viewModels?.Count >= 0)
                {
                    foreach (var supplierProd in existingSupplierProd)
                    {
                        var prod = viewModels.FirstOrDefault(c => c.ProductId == productId && c.SupplierId == supplierProd.SupplierId);
                        if (prod == null)
                        {
                            //item is deleted
                            supplierProd.UpdatedAt = DateTime.UtcNow;
                            supplierProd.UpdatedById = userId;
                            supplierProd.IsDeleted = true;
                            supplierProdRepository.Update(supplierProd);
                        }
                    }
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                }
                #endregion

                if (viewModels != null && viewModels.Count > 0 && productId > 0)
                {
                    foreach (var suppProd in viewModels)
                    {
                        //Check if supplierId is null or Invalid

                        if (suppProd != null)
                        {
                            //if (suppProd.SupplierId > 0)
                            //{
                            //    var suppRepo = _unitOfWork.GetRepository<Supplier>();
                            //    if (!(await suppRepo.GetAll(x => x.Id == suppProd.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                            //    {
                            //        throw new BadRequestException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                            //    }
                            //}
                            //else
                            //{
                            //    throw new NullReferenceCustomException(ErrorMessages.SupplierId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                            //}
                            //}

                            var supProdExists = await supplierProdRepository.GetAll(x => x.ProductId == productId && x.SupplierId == suppProd.SupplierId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                            var supplierProduct = MappingHelpers.Mapping<SupplierProductRequestModel, SupplierProduct>(suppProd);

                            if (supProdExists != null)
                            {
                                //Update Supplier product
                                supplierProduct.ProductId = productId;
                                supplierProduct.Id = supProdExists.Id;
                                supplierProduct.CreatedById = supProdExists.CreatedById;
                                supplierProduct.UpdatedById = userId;
                                supplierProduct.IsDeleted = false;
                                supplierProduct.CreatedAt = supProdExists.CreatedAt;
                                supplierProdRepository.DetachLocal(_ => _.Id == supplierProduct.Id);
                                supplierProdRepository.Update(supplierProduct);
                            }
                            else
                            {
                                //Add new Supplier Product
                                supplierProduct.ProductId = productId;
                                supplierProduct.CreatedById = userId;
                                supplierProduct.UpdatedById = userId;
                                supplierProduct.IsDeleted = false;
                                supplierProduct.CreatedAt = DateTime.UtcNow;
                                supplierProduct.UpdatedAt = DateTime.UtcNow;
                                var result = await supplierProdRepository.InsertAsync(supplierProduct).ConfigureAwait(false);
                            }
                        }

                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    }
                }
                return true;
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

        }
        public async Task<bool> SaveProductAPN(List<long> APNList, long productId, int userId, string ProdDesc)
        {
            var apnRepository = _unitOfWork?.GetRepository<APN>();

            #region Delete Products
            // Delete children
            var existingAPN = await apnRepository.GetAll(x => x.ProductId == productId
            && !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            if (APNList?.Count >= 0)
            {
                foreach (var APN in existingAPN.ToList())
                {
                    var apn = APNList.FirstOrDefault(c => c == APN.Number);
                    if (apn == 0)
                    {
                        //item is deleted
                        APN.UpdatedAt = DateTime.UtcNow;
                        APN.UpdatedById = userId;
                        APN.IsDeleted = true;
                        apnRepository.Update(APN);


                        //Adding To USerLog
                        var logModel = new UserLogRequestModel<APNResponseViewModel>
                        {
                            Action = (ActionPerformed.Delete).ToString(),
                            Module = "Product",
                            Table = "APN",
                            TableId = APN.ProductId,
                            OldData = MappingHelpers.Mapping<APN, APNResponseViewModel>(APN),
                            ActionBy = userId
                        };
                        await _userLogger.Insert(logModel).ConfigureAwait(false);
                    }
                }
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
            }
            #endregion


            foreach (var item in APNList)
            {
                //update ////if exists then update else add new APN for product
                var APNExists = await apnRepository.GetAll(x => x.Number == item && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                if (APNExists == null)
                {
                    var APN = new APN();
                    APN.CreatedById = userId;
                    APN.UpdatedById = userId;
                    APN.IsDeleted = false;
                    APN.Status = true;
                    APN.Number = item;
                    APN.ProductId = productId;
                    APN.Desc = ProdDesc;

                    var result = await apnRepository.InsertAsync(APN).ConfigureAwait(false);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                    //Adding To USerLog
                    var logModel = new UserLogRequestModel<APNResponseViewModel>
                    {
                        Action = (ActionPerformed.Create).ToString(),
                        Module = "Product",
                        Table = "APN",
                        TableId = result.ProductId,
                        NewData = MappingHelpers.Mapping<APN, APNResponseViewModel>(result),
                        ActionBy = userId
                    };
                    await _userLogger.Insert(logModel).ConfigureAwait(false);
                }
            }
            return true;
        }

        public async Task<bool> DeactivateProduct(long productId, int userId, bool Status)
        {
            try
            {
                if (productId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Product>();
                    var productExists = await repository.GetAll(x => x.Id == productId).Include(c => c.OutletProductProduct)
                        .Include(c => c.APNProduct)
                        .Include(c => c.SupplierProduct)
                        .Include(c => c.OutletProductProduct)
                        .Include(c => c.PromotionBuying)
                        .Include(c => c.PromotionCompetitionProduct)
                        .Include(c => c.PromotionMemberOffer)
                        .Include(c => c.PromotionMixmatchProduct)
                        .Include(c => c.PromotionOfferProduct)
                        .Include(c => c.PromotionSellingchProduct)
                        .Include(c => c.OrderDetailProduct)
                        .Include(c => c.StockAdjustDetailProduct)
                        .Include(c => c.StockTakeDetailProduct)
                        .Include(c => c.OrderDetailProduct)

                        .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (productExists != null)
                    {
                        //Delete all children as well.

                        var apnList = productExists.APNProduct?.ToList();
                        foreach (var apn in apnList)
                        {
                            apn.Status = Status;
                            apn.UpdatedById = userId;
                        }

                        var supplierList = productExists.SupplierProduct?.ToList();
                        foreach (var supplier in supplierList)
                        {
                            supplier.Status = Status;
                            supplier.UpdatedById = userId;
                        }

                        var outletProdList = productExists.OutletProductProduct?.ToList();
                        foreach (var outletProd in outletProdList)
                        {
                            outletProd.Status = Status;
                            outletProd.UpdatedById = userId;
                        }
                        repository.Update(productExists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task GetSupplierItemAsync(List<long> productId, int supplierId)
        {

            var repository = _unitOfWork.GetRepository<Product>();

            foreach (var product in productId?.ToList())
            {
                var products = await repository.GetAll(x => !x.IsDeleted && x.Id == product).Include(x => x.SupplierProduct).FirstOrDefaultAsync().ConfigureAwait(false);

                var supplierItem = products.SupplierProduct.Where(x => x.SupplierId == supplierId).FirstOrDefault();

            }
        }

        public async Task<ProductNumberModel> GetWarehouseCodes()
        {

            try
            {
                var wareRepo = _unitOfWork.GetRepository<Warehouse>();
                var warehouses = await wareRepo.GetAll(x => !x.IsDeleted).Select(x => x.Code).ToListAsync().ConfigureAwait(false);

                var warehouseList = new ProductNumberModel();

                //Remove Later
                warehouseList.HostCode = warehouses.FirstOrDefault();

                if (warehouses.Count >= 2)
                    warehouseList.HostCode2 = warehouses.ElementAt(1);
                if (warehouses.Count >= 3)
                    warehouseList.HostCode3 = warehouses.ElementAt(2);

                foreach (var item in warehouses)
                {

                    warehouseList.HostCodes.Add(item);
                }

                return warehouseList;
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }


        }

        /// <summary>
        /// Details for tabs in product modules 
        /// Transaction history, Weekely sales, Purchase history
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public async Task<ProductTabsHistoryResponseModel> ProductTabsHistory(SecurityViewModel securityViewModel, ProductHistoryFilter inputFilter)
        {
            ProductTabsHistoryResponseModel responseModel = new ProductTabsHistoryResponseModel();
            try
            {
                if (inputFilter != null)
                {
                    if (!string.IsNullOrEmpty(inputFilter.ModuleName) && inputFilter.ProductId != null)
                    {
                        if (inputFilter.ModuleName.ToLower() == "purchase")
                        {
                            var orderRepo = _unitOfWork?.GetRepository<OrderHeader>();
                            var orderDetailRepo = _unitOfWork?.GetRepository<OrderDetail>();

                            //Purchase History Line No 2140
                            var purchaseList = await orderDetailRepo.GetAll(x => !x.IsDeleted && x.ProductId == inputFilter.ProductId)
                                .Include(c => c.OrderType)
                                .Include(c => c.Supplier)
                                .Include(c => c.SupplierProduct)
                                .Include(c => c.Product)
                                .Include(c => c.OrderHeader)
                                .ThenInclude(c => c.Store)
                                .Include(c => c.OrderHeader).ThenInclude(c => c.Type)
                                .Include(c => c.OrderHeader).ThenInclude(c => c.Status)
                                .ToListAsync().ConfigureAwait(false);

                            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                                purchaseList = purchaseList.Where(x => !string.IsNullOrWhiteSpace(x.Product.AccessOutletIds) || securityViewModel.StoreIds.Contains(Convert.ToInt32(x.Product.AccessOutletIds))).ToList();

                            var purchaseModel = purchaseList.Select(MappingHelpers.CreatePurchaseHistory).ToList();

                            var newPurchaseModel = new PagedOutputModel<List<PurchaseHistoryModel>>(purchaseModel, purchaseModel.Count);

                            responseModel.PurchaseList = (newPurchaseModel);
                        }
                        if (inputFilter.ModuleName.ToLower() == "weeklysales" && inputFilter.FromDate != null && inputFilter.ToDate != null)
                        {

                            //Week Sales History Line 1517             
                            var storeAll = _unitOfWork?.GetRepository<Store>().GetAll();
                            var transactionAll = _unitOfWork?.GetRepository<Transaction>().GetAll();

                            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                                transactionAll = transactionAll.Include(x => x.Product).Where(x => !string.IsNullOrWhiteSpace(x.Product.AccessOutletIds) || securityViewModel.StoreIds.Contains(Convert.ToInt32(x.Product.AccessOutletIds)));

                            var query = (from tran in transactionAll
                                         join store in storeAll on tran.OutletId equals store.Id
                                         where tran.ProductId == inputFilter.ProductId && !tran.IsDeleted &&
                                         (tran.Date >= inputFilter.FromDate && tran.Date <= inputFilter.ToDate) &&
                                         tran.Type.ToUpper().Equals("ITEMSALE") &&
                                         tran.Qty != 0
                                         select new
                                         {
                                             WeekEnd = tran.Weekend,
                                             OutletId = tran.OutletId,
                                             OutletCode = store.Code,
                                             OutletDesc = store.Desc,
                                             Qty = tran.Qty,
                                             Cost = tran.Cost,
                                             Amt = tran.Amt,
                                             Discount = tran.Discount,
                                             PromoSales = tran.PromoSales
                                         });

                            var list = await query.GroupBy(x => new { x.WeekEnd, x.OutletId, x.OutletCode, x.OutletDesc })
                              .Select(x => new WeeklySalesHistoryModel
                              {
                                  WeekEnding = x.Key.WeekEnd,
                                  OutletId = x.Key.OutletId,
                                  OutletCode = x.Key.OutletCode,
                                  OutletDesc = x.Key.OutletDesc,
                                  Quantity = x.Sum(y => y.Qty),
                                  SalesCost = x.Sum(y => y.Cost),
                                  SalesAmt = x.Sum(y => y.Amt),
                                  AvgItemPrice = x.Sum(y => y.Amt) / x.Sum(y => y.Qty),
                                  Margin = x.Sum(y => y.Amt - y.Cost),
                                  GP = (x.Sum(y => y.Amt) - x.Sum(y => y.Cost)) * 100 / x.Sum(y => y.Amt),
                                  Discount = x.Sum(y => y.Discount),
                                  PromoSales = x.Sum(y => y.PromoSales)
                              }).OrderByDescending(x => x.WeekEnding).ThenBy(x => x.OutletCode)
                              .ToListAsync().ConfigureAwait(false);

                            responseModel.WeeklySalesList = new PagedOutputModel<List<WeeklySalesHistoryModel>>(list, list.Count);
                        }
                        if (inputFilter.ModuleName.ToLower() == "transaction" && inputFilter.FromDate != null && inputFilter.ToDate != null)
                        {
                            var transRepo = _unitOfWork?.GetRepository<Transaction>();
                            var transList = await transRepo.GetAll(x => !x.IsDeleted && x.ProductId == inputFilter.ProductId
                                       && (x.Date >= inputFilter.FromDate && x.Date <= inputFilter.ToDate))
                                 .Include(c => c.PromotionSell)
                                 .Include(c => c.PromotionBuy)
                                 .Include(c => c.Store)
                                 .Include(c => c.Till)
                                 .Include(c => c.Department)
                                 .Include(c => c.Supplier)
                                 .Include(c => c.User)
                                 .Include(c => c.Product)
                                 .ToListAsync().ConfigureAwait(false);

                            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                                transList = transList.Where(x => !string.IsNullOrWhiteSpace(x.Product.AccessOutletIds) || securityViewModel.StoreIds.Contains(Convert.ToInt32(x.Product.AccessOutletIds))).ToList();

                            var transactionList = transList.Select(MappingHelpers.CreateTransactionMap).ToList();

                            responseModel.TransactionList = new PagedOutputModel<List<TransactionHistoryModel>>(transactionList, transactionList.Count);
                        }

                        if (inputFilter.ModuleName.ToLower() == "children")
                        {

                            var prodRepo = _unitOfWork?.GetRepository<Product>();
                            var prodParent = await prodRepo.GetAll(x => !x.IsDeleted && x.Id == inputFilter.ProductId).Select(x => x.Number).FirstOrDefaultAsync().ConfigureAwait(false);

                            var ChildProducts = await prodRepo.GetAll(x => !x.IsDeleted && x.Parent == prodParent).Include(c => c.TypeMasterListItem).ToListAsync().ConfigureAwait(false);

                            var chilReponse = ChildProducts.Select(MappingHelpers.CreateChildProduct).ToList();

                            responseModel.ProductChildrenList = new PagedOutputModel<List<ProductChildModel>>(chilReponse, chilReponse.Count);
                        }

                        if (inputFilter.ModuleName.ToLower() == "stockmovement" && inputFilter.FromDate != null && inputFilter.ToDate != null && inputFilter.StoreId != null)
                        {

                            var transRepo = _unitOfWork?.GetRepository<Transaction>();
                            var transactionList = await transRepo.GetAll(x => !x.IsDeleted
                            && x.ProductId == inputFilter.ProductId
                            && (x.Date >= inputFilter.FromDate && x.Date <= inputFilter.ToDate)
                            && x.OutletId == inputFilter.StoreId)
                                .Include(x => x.Store)
                                .Include(c => c.Supplier)
                                 .ToListAsync().ConfigureAwait(false);

                            if (transactionList != null)
                            {
                                var stockMovment = transactionList.Select(MappingHelpers.CreateStockMovement).ToList();
                                responseModel.StockMovementList = new PagedOutputModel<List<ProductStockMovementModel>>(stockMovment, stockMovment.Count);
                            }
                        }

                        if (inputFilter.ModuleName.ToLower() == "zonepricing")
                        {

                            var zonePriceRepo = _unitOfWork?.GetRepository<ZonePricing>();
                            var zonePricingList = await zonePriceRepo.GetAll(x => !x.IsDeleted
                            && x.ProductId == inputFilter.ProductId)
                                .Include(c => c.PriceZoneCostPrice)
                                 .ToListAsync().ConfigureAwait(false);

                            if (zonePricingList != null)
                            {
                                var zonePricing = zonePricingList.Select(MappingHelpers.CreateZonePricing).ToList();
                                responseModel.ZonePricingList = new PagedOutputModel<List<ProductZonePricingModel>>(zonePricing, zonePricing.Count);
                            }
                        }

                        if (inputFilter.ModuleName.ToLower() == "history" && inputFilter.FromDate != null && inputFilter.ToDate != null)
                        {

                            var repository = _unitOfWork.GetRepository<UserLog>();

                            //Add Product History

                            var productHistoryModel = await _userLogger.GetAuditUserLog<ProductResponseModel>(inputFilter.ProductId, "Product", "Product", inputFilter.FromDate, inputFilter.ToDate).ConfigureAwait(false);

                            responseModel.ProductHistory = productHistoryModel;


                            //Add OutletProduct

                            var outletProductHistory = await _userLogger.GetAuditUserLog<OutletProductResponseViewModel>(inputFilter.ProductId, "OutletProduct", "Product", inputFilter.FromDate, inputFilter.ToDate).ConfigureAwait(false);

                            responseModel.OutletProductHistroy = outletProductHistory;

                            //Add APN

                            var APNHistory = await _userLogger.GetAuditUserLog<APNResponseViewModel>(inputFilter.ProductId, "APN", "Product", inputFilter.FromDate, inputFilter.ToDate).ConfigureAwait(false);

                            responseModel.APNProductHistory = APNHistory;
                        }

                    }
                }
                return responseModel;
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
        /// Details for tabs in product modules 
        /// Transaction history, Weekely sales, Purchase history
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public async Task<ProductTabsHistoryResponseModel> GetProductTabsHistory(SecurityViewModel securityViewModel, ProductHistoryFilter inputFilter)
        {
            ProductTabsHistoryResponseModel responseModel = new ProductTabsHistoryResponseModel();
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
                if (inputFilter != null)
                {
                    if (!string.IsNullOrEmpty(inputFilter.ModuleName) && inputFilter.ProductId != null)
                    {
                        if (inputFilter.ModuleName.ToLower() == "purchase")
                        {
                            var orderDetailRepo = _unitOfWork?.GetRepository<OrderDetail>();

                            List<SqlParameter> dbParams = new List<SqlParameter>{
                                new SqlParameter("@ProductId", inputFilter?.ProductId),
                                new SqlParameter("@AccessOutletIds", (StoreIds == true)?AccessStores:null),
                                new SqlParameter("@GlobalFilter", inputFilter?.GlobalFilter),
                                new SqlParameter("@SkipCount", inputFilter?.SkipCount),
                                new SqlParameter("@MaxResultCount", inputFilter?.MaxResultCount),
                                new SqlParameter("@SortColumn", inputFilter?.Sorting),
                                new SqlParameter("@SortDirection", inputFilter?.Direction)
                        };
                            var dset = await orderDetailRepo.ExecuteStoredProcedure(StoredProcedures.GetPurchaseHistory, dbParams.ToArray()).ConfigureAwait(false);
                            var purchaseModel = MappingHelpers.ConvertDataTable<PurchaseHistoryModel>(dset.Tables[0]);
                            var newPurchaseModel = new PagedOutputModel<List<PurchaseHistoryModel>>(purchaseModel, purchaseModel.Count);
                            responseModel.PurchaseList = (newPurchaseModel);
                        }
                        if (inputFilter.ModuleName.ToLower() == "weeklysales" && inputFilter.FromDate != null && inputFilter.ToDate != null)
                        {

                            //Week Sales History Line 1517    
                            var transactionRepo = _unitOfWork?.GetRepository<Transaction>();
                            List<SqlParameter> dbParams = new List<SqlParameter>{
                                new SqlParameter("@ProductId", inputFilter?.ProductId),
                                new SqlParameter("@FromDate", inputFilter?.FromDate),
                                new SqlParameter("@ToDate", inputFilter?.ToDate),
                                new SqlParameter("@AccessOutletIds", (StoreIds == true)?AccessStores:null),
                                new SqlParameter("@GlobalFilter", inputFilter?.GlobalFilter),
                                new SqlParameter("@SkipCount", inputFilter?.SkipCount),
                                new SqlParameter("@MaxResultCount", inputFilter?.MaxResultCount),
                                new SqlParameter("@SortColumn", inputFilter?.Sorting),
                                new SqlParameter("@SortDirection", inputFilter?.Direction)
                    };
                            var dset = await transactionRepo.ExecuteStoredProcedure(StoredProcedures.GetWeeklySales, dbParams.ToArray()).ConfigureAwait(false);
                            var weeklySalesHistoryModel = MappingHelpers.ConvertDataTable<WeeklySalesHistoryModel>(dset.Tables[0]);

                            responseModel.WeeklySalesList = new PagedOutputModel<List<WeeklySalesHistoryModel>>(weeklySalesHistoryModel, weeklySalesHistoryModel.Count);
                        }
                        if (inputFilter.ModuleName.ToLower() == "transaction" && inputFilter.FromDate != null && inputFilter.ToDate != null)
                        {
                            var transRepo = _unitOfWork?.GetRepository<Transaction>();
                            List<SqlParameter> dbParams = new List<SqlParameter>{
                                new SqlParameter("@ProductId", inputFilter?.ProductId),
                                new SqlParameter("@FromDate", inputFilter?.FromDate),
                                new SqlParameter("@ToDate", inputFilter?.ToDate),
                                new SqlParameter("@AccessOutletIds", (StoreIds == true)?AccessStores:null),
                                new SqlParameter("@GlobalFilter", inputFilter?.GlobalFilter),
                                new SqlParameter("@SkipCount", inputFilter?.SkipCount),
                                new SqlParameter("@MaxResultCount", inputFilter?.MaxResultCount),
                                new SqlParameter("@SortColumn", inputFilter?.Sorting),
                                new SqlParameter("@SortDirection", inputFilter?.Direction)
                    };
                            var dset = await transRepo.ExecuteStoredProcedure(StoredProcedures.GetTransactionHistory, dbParams.ToArray()).ConfigureAwait(false);
                            var transactionList = MappingHelpers.ConvertDataTable<TransactionHistoryModel>(dset.Tables[0]);

                            responseModel.TransactionList = new PagedOutputModel<List<TransactionHistoryModel>>(transactionList, transactionList.Count);
                        }

                        if (inputFilter.ModuleName.ToLower() == "children")
                        {

                            var prodRepo = _unitOfWork?.GetRepository<Product>();
                            List<SqlParameter> dbParams = new List<SqlParameter>{
                                new SqlParameter("@ProductId", inputFilter?.ProductId),
                                new SqlParameter("@GlobalFilter", inputFilter?.GlobalFilter),
                                new SqlParameter("@SkipCount", inputFilter?.SkipCount),
                                new SqlParameter("@MaxResultCount", inputFilter?.MaxResultCount),
                                new SqlParameter("@SortColumn", inputFilter?.Sorting),
                                new SqlParameter("@SortDirection", inputFilter?.Direction)
                    };
                            var dset = await prodRepo.ExecuteStoredProcedure(StoredProcedures.GetChilderns, dbParams.ToArray()).ConfigureAwait(false);
                            var chilReponse = MappingHelpers.ConvertDataTable<ProductChildModel>(dset.Tables[0]);
                            responseModel.ProductChildrenList = new PagedOutputModel<List<ProductChildModel>>(chilReponse, chilReponse.Count);
                        }

                        if (inputFilter.ModuleName.ToLower() == "stockmovement" && inputFilter.FromDate != null && inputFilter.ToDate != null && inputFilter.StoreId != null)
                        {

                            var transRepo = _unitOfWork?.GetRepository<Transaction>();
                            List<SqlParameter> dbParams = new List<SqlParameter>{
                                new SqlParameter("@ProductId", inputFilter?.ProductId),
                                new SqlParameter("@OutletId", inputFilter?.StoreId),
                                new SqlParameter("@FromDate", inputFilter?.FromDate),
                                new SqlParameter("@ToDate", inputFilter?.ToDate),
                                new SqlParameter("@GlobalFilter", inputFilter?.GlobalFilter),
                                new SqlParameter("@SkipCount", inputFilter?.SkipCount),
                                new SqlParameter("@MaxResultCount", inputFilter?.MaxResultCount),
                                new SqlParameter("@SortColumn", inputFilter?.Sorting),
                                new SqlParameter("@SortDirection", inputFilter?.Direction)
                    };
                            var dset = await transRepo.ExecuteStoredProcedure(StoredProcedures.GetStockMovement, dbParams.ToArray()).ConfigureAwait(false);
                            var stockMovment = MappingHelpers.ConvertDataTable<ProductStockMovementModel>(dset.Tables[0]);
                            responseModel.StockMovementList = new PagedOutputModel<List<ProductStockMovementModel>>(stockMovment, stockMovment.Count);
                        }

                        if (inputFilter.ModuleName.ToLower() == "zonepricing")
                        {

                            var zonePriceRepo = _unitOfWork?.GetRepository<ZonePricing>();
                            List<SqlParameter> dbParams = new List<SqlParameter>{
                                new SqlParameter("@ProductId", inputFilter?.ProductId),
                                new SqlParameter("@GlobalFilter", inputFilter?.GlobalFilter),
                                new SqlParameter("@SkipCount", inputFilter?.SkipCount),
                                new SqlParameter("@MaxResultCount", inputFilter?.MaxResultCount),
                                new SqlParameter("@SortColumn", inputFilter?.Sorting),
                                new SqlParameter("@SortDirection", inputFilter?.Direction)
                        };
                            var dset = await zonePriceRepo.ExecuteStoredProcedure(StoredProcedures.GetZonePricing, dbParams.ToArray()).ConfigureAwait(false);
                            var zonePricing = MappingHelpers.ConvertDataTable<ProductZonePricingModel>(dset.Tables[0]);
                            responseModel.ZonePricingList = new PagedOutputModel<List<ProductZonePricingModel>>(zonePricing, zonePricing.Count);

                        }

                        if (inputFilter.ModuleName.ToLower() == "history" && inputFilter.FromDate != null && inputFilter.ToDate != null)
                        {

                            var repository = _unitOfWork.GetRepository<UserLog>();

                            //Add Product History

                            var productHistoryModel = await _userLogger.GetAuditUserLog<ProductResponseModel>(inputFilter.ProductId, "Product", "Product", inputFilter.FromDate, inputFilter.ToDate).ConfigureAwait(false);

                            responseModel.ProductHistory = productHistoryModel;


                            //Add OutletProduct

                            var outletProductHistory = await _userLogger.GetAuditUserLog<OutletProductResponseViewModel>(inputFilter.ProductId, "OutletProduct", "Product", inputFilter.FromDate, inputFilter.ToDate).ConfigureAwait(false);

                            responseModel.OutletProductHistroy = outletProductHistory;

                            //Add APN

                            var APNHistory = await _userLogger.GetAuditUserLog<APNResponseViewModel>(inputFilter.ProductId, "APN", "Product", inputFilter.FromDate, inputFilter.ToDate).ConfigureAwait(false);

                            responseModel.APNProductHistory = APNHistory;
                        }

                    }
                }
                return responseModel;
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
        /// Get product details without APN Number
        /// </summary>
        /// <returns>List<ProductViewModel></returns>
        public async Task<PagedOutputModel<List<ProductResponseModel>>> GetAllProductsWithoutAPN(PagedInputModel filter = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Product>();

                var list = repository.GetAll()
                    .Include(c => c.CategoryMasterListItem)
                    .Include(c => c.Commodity)
                    .Include(c => c.Department)
                    .Include(c => c.GroupMasterListItem)
                    .Include(c => c.ManufacturerMasterListItem)
                    .Include(c => c.NationalRangeMasterListItem)
                    .Include(c => c.UnitMeasureMasterListItem)
                    .Include(c => c.OutletProductProduct)
                    .Include(c => c.Supplier)
                    .Include(c => c.Tax)
                    .Include(c => c.TypeMasterListItem)
                    .Include(c => c.APNProduct).Where(x => !x.IsDeleted && x.APNProduct.Count == 0);

                int count = 0;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Number.ToString().ToLower().Contains(filter.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(filter.GlobalFilter.ToLower()));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);

                    switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "code":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.Number);
                            else
                                list = list.OrderByDescending(x => x.Number);
                            break;
                        case "desc":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.Desc);
                            else
                                list = list.OrderByDescending(x => x.Desc);
                            break;
                        default:
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderByDescending(x => x.UpdatedAt);
                            else
                                list = list.OrderBy(x => x.UpdatedAt);
                            break;
                    }
                }

                var productsViewModel = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();
                return new PagedOutputModel<List<ProductResponseModel>>(productsViewModel, count);
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
        /// Get all active Products Without APN
        /// </summary>
        /// <returns>List<ProductViewModel></returns>
        public async Task<PagedOutputModel<List<ProductResponseModel>>> GetProductsWithoutAPN(ProductFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                bool StoreIds = false;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                    StoreIds = true;

                var repository = _unitOfWork?.GetRepository<Product>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                new SqlParameter("@Id", inputModel?.Id),
                new SqlParameter("@Status", inputModel?.Status),
                new SqlParameter("@Description", inputModel?.Description),
                new SqlParameter("@Number", inputModel?.Number),
                new SqlParameter("@DeptId", inputModel?.Dept),
                new SqlParameter("@Replicate", inputModel?.Replicate),
                new SqlParameter("@StoreId", inputModel?.StoreId),
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@SupplierId", inputModel?.SupplierId),
                new SqlParameter("@SkipCount", inputModel?.SkipCount),
                new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                new SqlParameter("@AccessOutletIds", (StoreIds == true)?securityViewModel?.StoreIds:null),
                new SqlParameter("@IsWithoutAPNCall", IsRequired.True),
            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveProducts, dbParams.ToArray()).ConfigureAwait(false);
                List<ProductResponseModel> productsViewModel = MappingHelpers.ConvertDataTable<ProductResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<ProductResponseModel>>(productsViewModel, count);
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

        public async Task<ProductResponseModel> UpdateProduct(ProductRequestModel viewModel, long productId, int userId, SecurityViewModel securityViewModel, string imagePath = null)
        {
            try
            {
                ProductResponseModel responseModel = new ProductResponseModel();
                if (viewModel != null)
                {
                    if (productId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                    }

                    var repository = _unitOfWork?.GetRepository<Product>();

                    var exists = await repository.GetById(productId).ConfigureAwait(false);
                    var oldData = await GetByProductId(productId, null, false).ConfigureAwait(false);
                    if (exists != null)
                    {
                        var productRequestModelDataTable = MappingHelpers.ToDataTable(viewModel);

                        DataTable OutletProductDataTable = null;
                        DataTable SupplierProductDataTable = null;
                        string APNMumbers = null;
                        if (viewModel?.APNNumber != null)
                        {
                            foreach (var apn in viewModel.APNNumber)
                            {
                                if (viewModel.APNNumber.Distinct().Count() != viewModel.APNNumber.Count)
                                {
                                    //can't add same APN twice
                                    throw new AlreadyExistsException(ErrorMessages.APNNumberDuplicate.ToString(CultureInfo.CurrentCulture));
                                }
                                if (APNMumbers == null)
                                {
                                    APNMumbers = "";
                                }
                                else
                                {
                                    APNMumbers += ",";
                                }
                                APNMumbers += apn;
                            }
                            if (productRequestModelDataTable.Rows.Count > 0)
                            {
                                productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                            }
                        }
                        if (securityViewModel != null && !securityViewModel.IsAdminUser && !securityViewModel.AddUnlockProduct && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0 && (viewModel.AccessOutletIds == null || viewModel.AccessOutletIds.Count <= 0))
                        {
                            viewModel.AccessOutletIds = securityViewModel.StoreIds;
                        }


                        string AccessOutletIds = null;
                        if (viewModel?.AccessOutletIds != null)
                        {
                            foreach (var outletId in viewModel.AccessOutletIds)
                            {
                                if (AccessOutletIds == null)
                                    AccessOutletIds = "";
                                else
                                    AccessOutletIds += ",";
                                AccessOutletIds += outletId;
                            }

                            if (productRequestModelDataTable.Rows.Count > 0)
                            {
                                productRequestModelDataTable.Rows[0]["AccessOutletIds"] = AccessOutletIds;
                            }
                        }

                        if (viewModel.OutletProduct != null && viewModel.OutletProduct.Count > 0)
                        {
                            foreach (var outProd in viewModel.OutletProduct)
                            {
                                //Check if store Id exists
                                if (outProd.StoreId <= 0)
                                {
                                    throw new BadRequestException(ErrorMessages.OutletProductStore.ToString(CultureInfo.CurrentCulture));
                                }
                                //If supplier is 0, change it to null
                                if (outProd.SupplierId == 0)
                                {
                                    outProd.SupplierId = null;
                                }
                            }
                            OutletProductDataTable = MappingHelpers.ToDataTable(viewModel.OutletProduct, true);
                        }
                        if (viewModel.SupplierProduct != null && viewModel.SupplierProduct.Count > 0)
                        {
                            if (viewModel.SupplierProduct.Select(x => x.SupplierId).Distinct().Count() != viewModel.SupplierProduct.Count)
                            {
                                //can't add same product twice
                                throw new BadRequestException(ErrorMessages.ProductSupplierDuplicate.ToString(CultureInfo.CurrentCulture));
                            }
                            foreach (var suppProd in viewModel.SupplierProduct)
                            {
                                //Check if supplierId is null or Invalid
                                if (suppProd != null)
                                {
                                    if (suppProd.SupplierId <= 0)
                                    {
                                        throw new NullReferenceCustomException(ErrorMessages.ProductSupplier.ToString(System.Globalization.CultureInfo.CurrentCulture));
                                    }
                                }
                            }
                            SupplierProductDataTable = MappingHelpers.ToDataTable(viewModel.SupplierProduct, true);
                        }
                        List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@UserId", userId),
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@productRequestModelDataTable",
                  TypeName ="dbo.ProductRequestType",
                  Value = productRequestModelDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@OutletProductDataTable",
                  TypeName ="dbo.OutletProductType",
                  Value = OutletProductDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter
                {
                  Direction = ParameterDirection.Input,
                  ParameterName = "@SupplierProductDataTable",
                  TypeName ="dbo.SupplierProductType",
                  Value = SupplierProductDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString())
            };
                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.AddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);

                        if (dset?.Tables?.Count > 0)
                        {
                            if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                            {
                                throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                            }
                            else
                            {
                                responseModel = await GetByProductId(Convert.ToInt64(productId), null, false).ConfigureAwait(false);

                                if (responseModel != oldData)
                                {
                                    //Adding To USerLog if updated
                                    var logModel = new UserLogRequestModel<ProductResponseModel>
                                    {
                                        Action = (ActionPerformed.Update).ToString(),
                                        Module = "Product",
                                        Table = "Product",
                                        TableId = Convert.ToInt64(productId),
                                        NewData = responseModel,
                                        OldData = oldData,
                                        ActionBy = userId
                                    };
                                    await _userLogger.Insert(logModel).ConfigureAwait(false);
                                }


                            }
                        }
                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));

                }
                throw new NullReferenceException();
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
        public async Task<ProductResponseModel> InsertProduct(ProductRequestModel viewModel, SecurityViewModel securityViewModel, int userId)
        {
            ProductResponseModel responseModel = new ProductResponseModel();
            try
            {
                var repository = _unitOfWork?.GetRepository<Product>();
                if (viewModel != null)
                {
                    if (viewModel.DepartmentId > 0)
                    {
                        var productRequestModelDataTable = MappingHelpers.ToDataTable(viewModel);
                        DataTable OutletProductDataTable = null;
                        DataTable SupplierProductDataTable = null;
                        string APNMumbers = null;


                        if (viewModel.APNNumber != null)
                        {
                            if (viewModel.APNNumber.Distinct().Count() != viewModel.APNNumber.Count)
                            {
                                //can't add same APN twice
                                throw new AlreadyExistsException(ErrorMessages.APNNumberDuplicate.ToString(CultureInfo.CurrentCulture));
                            }
                            foreach (var apn in viewModel.APNNumber)
                            {
                                if (APNMumbers == null)
                                {
                                    APNMumbers = "";
                                }
                                else
                                {
                                    APNMumbers += ",";
                                }
                                APNMumbers += apn;
                            }
                            if (productRequestModelDataTable.Rows.Count > 0)
                            {
                                productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                            }
                        }

                        if (securityViewModel != null && !securityViewModel.IsAdminUser && !securityViewModel.AddUnlockProduct && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0 && (viewModel.AccessOutletIds == null || viewModel.AccessOutletIds.Count <= 0))
                        {
                            viewModel.AccessOutletIds = securityViewModel.StoreIds;
                        }


                        string AccessOutletIds = null;
                        if (viewModel?.AccessOutletIds != null)
                        {
                            foreach (var outletId in viewModel.AccessOutletIds)
                            {
                                if (AccessOutletIds == null)
                                    AccessOutletIds = "";
                                else
                                    AccessOutletIds += ",";
                                AccessOutletIds += outletId;
                            }

                            if (productRequestModelDataTable.Rows.Count > 0)
                            {
                                productRequestModelDataTable.Rows[0]["AccessOutletIds"] = AccessOutletIds;
                            }
                        }

                        //Checking all Outlet Products
                        #region Check outlet Products
                        if (viewModel.OutletProduct != null && viewModel.OutletProduct.Count > 0)
                        {
                            foreach (var outProd in viewModel.OutletProduct)
                            {
                                //Check if store Id exists
                                if (outProd.StoreId <= 0)
                                {
                                    throw new BadRequestException(ErrorMessages.OutletProductStore.ToString(CultureInfo.CurrentCulture));
                                }
                                //If supplier is 0, change it to null
                                if (outProd.SupplierId == 0)
                                {
                                    outProd.SupplierId = null;
                                }
                            }
                            OutletProductDataTable = MappingHelpers.ToDataTable(viewModel.OutletProduct, true);
                        }
                        #endregion

                        //Checking Supplier Products
                        #region Check Supplier Products
                        if (viewModel.SupplierProduct != null && viewModel.SupplierProduct.Count > 0)
                        {
                            if (viewModel.SupplierProduct.Select(x => x.SupplierId).Distinct().Count() != viewModel.SupplierProduct.Count)
                            {
                                //can't add same product twice
                                throw new BadRequestException(ErrorMessages.ProductSupplierDuplicate.ToString(CultureInfo.CurrentCulture));
                            }

                            foreach (var suppProd in viewModel.SupplierProduct)
                            {
                                //Check if supplierId is null or Invalid

                                if (suppProd != null)
                                {
                                    if (suppProd.SupplierId <= 0)
                                    {
                                        throw new NullReferenceCustomException(ErrorMessages.ProductSupplier.ToString(System.Globalization.CultureInfo.CurrentCulture));
                                    }
                                }
                            }
                            SupplierProductDataTable = MappingHelpers.ToDataTable(viewModel.SupplierProduct, true);
                        }
                        #endregion

                        if (viewModel.Parent != null && viewModel.Parent > 0)
                        {
                            if (viewModel.Number == viewModel.Parent)
                            {
                                throw new AlreadyExistsException(ErrorMessages.ProductParentSame.ToString(CultureInfo.CurrentCulture));
                            }
                        }




                        List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@ProductId", 0),
                new SqlParameter("@UserId", userId),
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@productRequestModelDataTable",
                  TypeName ="dbo.ProductRequestType",
                  Value = productRequestModelDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@OutletProductDataTable",
                  TypeName ="dbo.OutletProductType",
                  Value = OutletProductDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter
                {
                  Direction = ParameterDirection.Input,
                  ParameterName = "@SupplierProductDataTable",
                  TypeName ="dbo.SupplierProductType",
                  Value = SupplierProductDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@ActionPerformed",ActionPerformed.Create.ToString()),

            };
                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.AddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);

                        if (dset?.Tables?.Count > 0)
                        {
                            if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                            {
                                throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                            }
                            else
                            {
                                responseModel = await GetByProductId(Convert.ToInt64(dset.Tables[0].Rows[0]["ProductId"]), null, false).ConfigureAwait(false);

                                //Adding To USerLog
                                var logModel = new UserLogRequestModel<ProductResponseModel>
                                {
                                    Action = (ActionPerformed.Create).ToString(),
                                    Module = "Product",
                                    Table = "Product",
                                    TableId = Convert.ToInt64(dset.Tables[0].Rows[0]["ProductId"]),
                                    NewData = responseModel,
                                    ActionBy = userId
                                };

                                await _userLogger.Insert(logModel).ConfigureAwait(false);
                            }
                        }
                        // responseModel = await GetByProductId(productId, null).ConfigureAwait(false);
                        return responseModel;
                    }
                    else
                    {
                        throw new NullReferenceCustomException(ErrorMessages.DeptId.ToString(CultureInfo.CurrentCulture));
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
            return responseModel;
        }

    }
}
