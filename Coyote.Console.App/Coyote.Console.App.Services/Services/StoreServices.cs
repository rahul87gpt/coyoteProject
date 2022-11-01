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
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services
{
    public class StoreServices : IStoreServices
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;
        private readonly IUserLoggerServices _userLogger;


        /// <summary>
        /// Setting Values of Private Variables.
        /// </summary>
        /// <param name="repo">Database Context ObjectS</param>
        /// <param name="iAutoMapperService">Auto Mapper to Map EntityModel to ViewModel</param>
        public StoreServices(IUnitOfWork repo, IAutoMappingServices iAutoMapperService, ILoggerManager iLoggerManager, IUserLoggerServices userLogger)
        {
            _unitOfWork = repo;
            _iAutoMapper = iAutoMapperService;
            _iLoggerManager = iLoggerManager;
            _userLogger = userLogger;
        }


        /// <summary>
        /// To delete the store with specific Store_Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteStore(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Store>();
                    var exists = await repository.GetAll(x => x.Id == Id && !x.IsDeleted)
                        .Include(c => c.OutletProducts)
                        .Include(c => c.OutletTradingHours)
                        .Include(c => c.OutletSupplierSettingStroes)
                        .Include(c => c.OrderHeaderOutlet)
                        .Include(c => c.SupplierOrderingScheduleStore)
                        .Include(c => c.KeypadOutlet)
                        .Include(c => c.TillOutlet)
                        .Include(c => c.ZoneOutlets)
                        .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        if (exists.OutletProducts != null)
                        {
                            foreach (var item in exists.OutletProducts.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }

                        if (exists.OutletTradingHours != null)
                        {
                            foreach (var item in exists.OutletTradingHours.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }

                        if (exists.OutletSupplierSettingStroes != null)
                        {
                            foreach (var item in exists.OutletSupplierSettingStroes.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }

                        if (exists.OrderHeaderOutlet != null)
                        {
                            foreach (var item in exists.OrderHeaderOutlet.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }

                        if (exists.ZoneOutlets != null)
                        {
                            foreach (var item in exists.ZoneOutlets.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }

                        if (exists.TillOutlet != null)
                        {
                            foreach (var item in exists.TillOutlet.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }

                        if (exists.KeypadOutlet != null)
                        {
                            foreach (var item in exists.KeypadOutlet.ToList())
                            {
                                item.UpdatedById = userId;
                                item.IsDeleted = true;
                            }
                        }


                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;
                        // exists.Code = (exists.Code + "~" + exists.Id);
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                }
                throw new NullReferenceException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }

        /// <summary>
        /// Getting All Stores that are not deleted.
        /// </summary>
        /// <returns>List<StoreViewModel></returns>
        public async Task<PagedOutputModel<List<StoreResponseModel>>> GetAllActiveStores(SecurityViewModel securityViewModel, PagedInputModel filter = null, int? groupId = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Store>();
                var list = repository.GetAll(x => !x.IsDeleted, includes: new Expression<Func<Store, object>>[] { c => c.StoreGroups, c => c.ZoneOutlets });

                if (groupId != null && groupId != 0) //filter apply by group id
                    list = list.Where(x => x.GroupId == groupId);

                int count = 0;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Code.ToLower().Contains(filter.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(filter.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((filter?.Status)))
                        list = list.Where(x => x.Status);

                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        list = list.Where(x => securityViewModel.StoreIds.Contains(x.Id));

                    list = list.OrderByDescending(x => x.UpdatedAt);

                    count = list.Count();
                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
                    switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "code":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.Code);
                            else
                                list = list.OrderByDescending(x => x.Code);
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
                var listViewModel = (await list.ToListAsync().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                //foreach (var item in listViewModel)
                //{
                //    item.GroupCode = list.Where(x => x.Id == item.Id).Select(x => x.StoreGroups.Code).FirstOrDefault();
                //    item.GroupName = list.Where(x => x.Id == item.Id).Select(x => x.StoreGroups.Name).FirstOrDefault();
                //}

                return new PagedOutputModel<List<StoreResponseModel>>(listViewModel, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Getting All Stores that are not deleted.
        /// </summary>
        /// <returns>List<StoreViewModel></returns>
        public async Task<PagedOutputModel<List<StoreResponseModel>>> GetActiveStores(SecurityViewModel securityViewModel, PagedInputModel filter = null, int? groupId = null)
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

                var repository = _unitOfWork?.GetRepository<Store>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", filter?.GlobalFilter),
                        new SqlParameter("@Id", (StoreIds == true)?AccessStores:null),
                        new SqlParameter("@SortColumn", filter?.Sorting),
                        new SqlParameter("@SortDirection", filter?.Direction),
                        new SqlParameter("@PageNumber", filter?.SkipCount),
                        new SqlParameter("@PageSize", filter?.MaxResultCount),
                        new SqlParameter("@GroupId", (groupId != null && groupId != 0)?groupId:null),
                        new SqlParameter("@IsLogged", filter?.IsLogged),
                        new SqlParameter("@Module","Store"),
                        new SqlParameter("@RoleId",RoleId)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveStores, dbParams.ToArray()).ConfigureAwait(false);
                List<StoreResponseModel> productsViewModel = MappingHelpers.ConvertDataTable<StoreResponseModel>(dset.Tables[0]);
                var count = 0;
                switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
                {
                    case "code":
                        if (string.IsNullOrEmpty(filter.Direction))
                            productsViewModel.OrderBy(x => x.Code);
                        else
                            productsViewModel.OrderByDescending(x => x.Code);
                        break;
                    case "desc":
                        if (string.IsNullOrEmpty(filter.Direction))
                            productsViewModel.OrderBy(x => x.Desc);
                        else
                            productsViewModel.OrderByDescending(x => x.Desc);
                        break;
                    default:
                        if (string.IsNullOrEmpty(filter.Direction))
                            productsViewModel.OrderByDescending(x => x.UpdatedAt);
                        else
                            productsViewModel.OrderBy(x => x.UpdatedAt);
                        break;
                }


                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<StoreResponseModel>>(productsViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get a store with specific Store_Id
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>StoreViewModel</returns>
        public async Task<StoreResponseModel> GetStoreById(int storeId)
        {
            try
            {
                if (storeId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Store>();
                    var store = await repository.GetAll(x => x.Id == storeId && !x.IsDeleted)
                        .Include(c => c.OutletTradingHours)
                        .Include(c => c.TillOutlet)
                        .Include(c => c.SupplierOrderingScheduleStore).ThenInclude(c => c.Supplier)
                        .Include(c => c.ZoneOutlets)
                        .Include(c => c.LabelTypePromo)
                        .Include(c => c.LabelTypeShelf)
                        .Include(c => c.LabelTypeShort)
                        .Include(c => c.Warehouse)
                        .Include(c => c.PriceZone)
                        .Include(c => c.CostZone)
                        .Include(c => c.OutletRoyaltyScalesOutlet)
                        .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (store == null)
                    {
                        throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    StoreResponseModel storeViewModel;
                    storeViewModel = MappingHelpers.CreateMap(store);

                    var storeApp = MappingHelpers.Mapping<Store, AppStoreRequestModel>(store);
                    if (storeApp != null)
                    {
                        storeViewModel.AppStoreDetails = storeApp;
                    }

                    if (store.TillOutlet != null)
                    {
                        var tillsList = store.TillOutlet.Where(x => !x.IsDeleted).Select(MappingHelpers.CreateMap);

                        storeViewModel.Tills.AddRange(tillsList);
                    }
                    if (store.SupplierOrderingScheduleStore != null)
                    {

                        var supplierSchedule = store.SupplierOrderingScheduleStore.Where(x => !x.IsDeleted).Select(MappingHelpers.CreateMap);
                        storeViewModel.OutletSupplierSchedules = new List<SupplierOrderScheduleResponseModel>();
                        storeViewModel.OutletSupplierSchedules.AddRange(supplierSchedule);
                    }
                    if (store.OutletRoyaltyScalesOutlet != null)
                    {
                        var royalty = store.OutletRoyaltyScalesOutlet.Where(x => x.Type == RoyaltyScale.Royalty).Select(MappingHelpers.Mapping<OutletRoyaltyScales, RoyaltyScalesRequestModel>);
                        if (royalty != null)
                        {
                            storeViewModel.RoyaltyScales = new List<RoyaltyScalesRequestModel>();
                            storeViewModel.RoyaltyScales.AddRange(royalty);
                        }

                        var advertising = store.OutletRoyaltyScalesOutlet.Where(x => x.Type == RoyaltyScale.Advertising).Select(MappingHelpers.Mapping<OutletRoyaltyScales, RoyaltyScalesRequestModel>);
                        if (advertising != null)
                        {
                            storeViewModel.AdvertisingRoyaltyScales = new List<RoyaltyScalesRequestModel>();
                            storeViewModel.AdvertisingRoyaltyScales.AddRange(advertising);
                        }
                    }

                    return storeViewModel;
                }
                throw new NullReferenceException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get a store with specific Store_Id
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>StoreViewModel</returns>
        public async Task<StoreResponseModel> GetActiveStoreById(int storeId)
        {
            try
            {
                if (storeId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Store>();
                    StoreResponseModel storeViewModel = new StoreResponseModel();

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Id", storeId)
                    };
                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetStoreById, dbParams.ToArray()).ConfigureAwait(false);
                    storeViewModel = MappingHelpers.ConvertDataTable<StoreResponseModel>(dset.Tables[0]).FirstOrDefault();

                    if (dset == null)
                    {
                        throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var TillOutlet = MappingHelpers.ConvertDataTable<TillResponseModel>(dset.Tables[1]);
                    storeViewModel.Tills.AddRange(TillOutlet);


                    var AppStore = MappingHelpers.ConvertDataTable<AppStoreRequestModel>(dset.Tables[2]);
                    storeViewModel.AppStoreDetails = AppStore.FirstOrDefault();

                    var tradingHours = MappingHelpers.ConvertDataTable<StoreTradingHoursRequest>(dset.Tables[3]);
                    storeViewModel.StoreTradingHours = new StoreTradingHoursRequest();
                    storeViewModel.StoreTradingHours = tradingHours.FirstOrDefault();

                    var supplierOrderingSchedule = MappingHelpers.ConvertDataTable<SupplierOrderScheduleResponseModel>(dset.Tables[4]);
                    storeViewModel.OutletSupplierSchedules = new List<SupplierOrderScheduleResponseModel>();
                    storeViewModel.OutletSupplierSchedules.AddRange(supplierOrderingSchedule);


                    var royalty = MappingHelpers.ConvertDataTable<RoyaltyScalesRequestModel>(dset.Tables[5]);
                    storeViewModel.RoyaltyScales = new List<RoyaltyScalesRequestModel>();
                    storeViewModel.RoyaltyScales.AddRange(royalty);

                    var advertising = MappingHelpers.ConvertDataTable<RoyaltyScalesRequestModel>(dset.Tables[6]);
                    storeViewModel.AdvertisingRoyaltyScales = new List<RoyaltyScalesRequestModel>();
                    storeViewModel.AdvertisingRoyaltyScales.AddRange(advertising);


                    var zoneList = MappingHelpers.ConvertDataTable<int>(dset.Tables[6]);
                    storeViewModel.Zones.AddRange(zoneList);

                    return storeViewModel;
                }
                throw new NullReferenceException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<StoreResponseModel> Update(StoreRequestModel viewModel, int storeId, int userId)
        {
            try
            {
                StoreResponseModel responseModel = new StoreResponseModel();
                if (viewModel != null)
                {
                    if (storeId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
                    }

                    var repository = _unitOfWork?.GetRepository<Store>();
                    if (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted && x.Id != storeId).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.StoreCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.GroupId > 0)
                    {
                        var grpRepo = _unitOfWork?.GetRepository<StoreGroup>();
                        if (!await grpRepo.GetAll(x => x.Id == viewModel.GroupId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.AppStoreDetails != null)
                        if (viewModel.AppStoreDetails.Latitude != null && (viewModel.AppStoreDetails.Latitude < -90 || viewModel.AppStoreDetails.Latitude > 90))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidLatitude.ToString(CultureInfo.CurrentCulture));
                        }

                    if (viewModel.AppStoreDetails.Longitude != null && (viewModel.AppStoreDetails.Longitude < -180 || viewModel.AppStoreDetails.Longitude > 180))
                    {
                        throw new BadRequestException(ErrorMessages.InvalidLogitude.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.LabelTypeShelfId > 0 || viewModel.LabelTypeShortId > 0 || viewModel.LabelTypePromoId > 0)
                    {
                        var labelRepository = _unitOfWork?.GetRepository<PrintLabelType>();

                        if (viewModel.LabelTypeShelfId > 0 && !await labelRepository.GetAll(x => x.Id == viewModel.LabelTypeShelfId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidShelfLabel.ToString(CultureInfo.CurrentCulture));
                        }

                        if (viewModel.LabelTypeShortId > 0 && !await labelRepository.GetAll(x => x.Id == viewModel.LabelTypeShortId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidShortLabel.ToString(CultureInfo.CurrentCulture));
                        }

                        if (viewModel.LabelTypePromoId > 0 && !await labelRepository.GetAll(x => x.Id == viewModel.LabelTypePromoId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidPromoLabel.ToString(CultureInfo.CurrentCulture));
                        }

                    }

                    if (viewModel.WarehouseId != null)
                    {

                        var warehouseRepo = _unitOfWork?.GetRepository<Warehouse>();
                        if (!await warehouseRepo.GetAll(x => x.Id == viewModel.WarehouseId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))

                            throw new BadRequestException(ErrorMessages.InvalidWarehouse.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.PriceZoneId != null)
                    {
                        if (viewModel.PriceZoneId > 0)
                        {
                            var proceZoneList = _unitOfWork?.GetRepository<CostPriceZones>();
                            if (!await proceZoneList.GetAll().Where(x => x.ID == viewModel.PriceZoneId && x.IsActive == Status.Active
                            && x.Type == CostPriceZoneType.Price).AnyAsyncSafe().ConfigureAwait(false))

                                throw new BadRequestException(ErrorMessages.InvalidPriceZoneId.ToString(CultureInfo.CurrentCulture));
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.InvalidPriceZoneId.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.CostZoneId != null)
                    {
                        if (viewModel.CostZoneId > 0)
                        {
                            var masterlist = _unitOfWork?.GetRepository<CostPriceZones>();
                            if (!await masterlist.GetAll().Where(x => x.ID == viewModel.CostZoneId && x.IsActive == Status.Active
                            && x.Type == CostPriceZoneType.Cost).AnyAsyncSafe().ConfigureAwait(false))
                            {
                                throw new BadRequestException(ErrorMessages.InvalidCostZoneId.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCostZoneId.ToString(CultureInfo.CurrentCulture));
                        }
                    }


                    if (viewModel.RoyaltyScales != null)
                    {

                        if (viewModel?.RoyaltyScales.Select(x => x.ScalesFrom).Count() > 5)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidRoyaltyCount.ToString(CultureInfo.CurrentCulture));
                        }

                        for (int i = 0; i < viewModel.RoyaltyScales.Count - 1; i++)
                        {

                            if (viewModel.RoyaltyScales[i].ScalesFrom > viewModel.RoyaltyScales[i].ScalesTo)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyScale.ToString(CultureInfo.CurrentCulture));
                            }

                            if (viewModel.RoyaltyScales[i].Percent <= 0)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyPerct.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                    }

                    if (viewModel.AdvertisingRoyaltyScales != null)
                    {

                        if (viewModel?.AdvertisingRoyaltyScales.Select(x => x.ScalesFrom).Distinct().Count() > 5)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidRoyaltyCount.ToString(CultureInfo.CurrentCulture));
                        }

                        for (int i = 0; i < viewModel.AdvertisingRoyaltyScales.Count - 1; i++)
                        {
                            if (viewModel.AdvertisingRoyaltyScales[i].ScalesFrom > viewModel.AdvertisingRoyaltyScales[i].ScalesTo)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidAdvRoyaltyScale.ToString(CultureInfo.CurrentCulture));
                            }

                            if (viewModel.AdvertisingRoyaltyScales[i].Percent <= 0)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyPerct.ToString(CultureInfo.CurrentCulture));
                            }
                            //for (int j = i + 1; j < viewModel.AdvertisingRoyaltyScales.Count; j++)
                            //{
                            //    if (viewModel.AdvertisingRoyaltyScales[j].ScalesFrom <= viewModel.AdvertisingRoyaltyScales[i].ScalesTo)
                            //    {
                            //        throw new BadRequestException(ErrorMessages.InvalidRoyaltyAdvScalePrev.ToString(CultureInfo.CurrentCulture));
                            //    }
                            //}
                        }
                    }

                    var exists = await repository.GetAll(x => x.Id == storeId).Include(c => c.SupplierOrderingScheduleStore).Include(x => x.OutletRoyaltyScalesOutlet).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        var comm = MappingHelpers.CreateRequestMap(viewModel);
                        comm.Id = storeId;
                        comm.IsDeleted = false;
                        comm.UpdatedById = userId;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;

                        #region Outlet Suplier Schedule
                        if (viewModel.OutletSupplierSchedules != null)
                        {
                            var OutletSupplierScheduleRepo = _unitOfWork.GetRepository<SupplierOrderingSchedule>();

                            //if (viewModel?.OutletSupplierSchedules.Select(x => x.SupplierId).Distinct().Count() != viewModel?.OutletSupplierSchedules.Count)
                            //{
                            //    //can't add same product twice
                            //    throw new BadRequestException(ErrorMessages.OutletSupplierSchedulesDuplicate.ToString(CultureInfo.CurrentCulture));
                            //}

                            #region Delete Children
                            // Delete children
                            if (exists.SupplierOrderingScheduleStore?.Count > 0)
                            {
                                foreach (var existingChild in exists.SupplierOrderingScheduleStore.ToList())
                                {
                                    var child = viewModel.OutletSupplierSchedules.FirstOrDefault(c => c.SupplierId == existingChild.SupplierId && c.StoreId == existingChild.StoreId);
                                    if (child == null)
                                    {
                                        //item is deleted
                                        existingChild.UpdatedAt = DateTime.UtcNow;
                                        existingChild.UpdatedById = userId;
                                        existingChild.IsDeleted = true;
                                        OutletSupplierScheduleRepo.Update(existingChild);
                                    }
                                }
                            }
                            #endregion

                            foreach (var setting in viewModel.OutletSupplierSchedules)
                            {
                                var supplierRepository = _unitOfWork?.GetRepository<Supplier>();
                                if (!await supplierRepository.GetAll(x => x.Id == setting.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                                {
                                    throw new BadRequestException($"{ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture)} :{setting.SupplierId}");
                                }

                                var schedule = MappingHelpers.Mapping<SupplierOrderScheduleRequestModel, SupplierOrderingSchedule>(setting);

                                var existingSetting = await OutletSupplierScheduleRepo.GetAll(x => x.StoreId == storeId && x.SupplierId == setting.SupplierId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);


                                if (comm.SupplierOrderingScheduleStore == null)
                                {
                                    comm.SupplierOrderingScheduleStore = new List<SupplierOrderingSchedule>();

                                }
                                if (existingSetting != null)
                                {
                                    schedule.CreatedById = existingSetting.CreatedById;
                                    schedule.CreatedAt = existingSetting.CreatedAt;
                                    schedule.UpdatedById = userId;
                                    schedule.Id = existingSetting.Id;
                                    OutletSupplierScheduleRepo.DetachLocal(_ => _.Id == existingSetting.Id);
                                    OutletSupplierScheduleRepo.Update(schedule);
                                }
                                else
                                {
                                    schedule.CreatedById = userId;
                                    schedule.UpdatedById = userId;
                                    comm.SupplierOrderingScheduleStore.Add(schedule);
                                }
                            }
                        }
                        else if (exists.SupplierOrderingScheduleStore != null)
                        {
                            var OutletSupplierScheduleRepo = _unitOfWork.GetRepository<SupplierOrderingSchedule>();
                            foreach (var existingChild in exists.SupplierOrderingScheduleStore.ToList())
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = userId;
                                existingChild.IsDeleted = true;
                                OutletSupplierScheduleRepo.Update(existingChild);
                            }
                        }
                        #endregion

                        #region royalty scales

                        if (viewModel.RoyaltyScales != null)
                        {
                            var royaltyRepo = _unitOfWork.GetRepository<OutletRoyaltyScales>();

                            if (viewModel?.RoyaltyScales.Select(x => new { x.ScalesFrom, x.ScalesTo }).Distinct().Count() != viewModel?.RoyaltyScales.Count)
                            {
                                //can't add same scale  twice
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyScaaleRepeat.ToString(CultureInfo.CurrentCulture));
                            }

                            #region Delete Children
                            // Delete children
                            if (exists.OutletRoyaltyScalesOutlet.Where(x => x.Type == RoyaltyScale.Royalty).Any())
                            {
                                foreach (var existingChild in exists.OutletRoyaltyScalesOutlet.Where(x => x.Type == RoyaltyScale.Royalty).ToList())
                                {
                                    var child = viewModel.RoyaltyScales.FirstOrDefault(c => c.ScalesFrom == existingChild.ScalesFrom && c.ScalesTo == existingChild.ScalesTo);
                                    if (child == null)
                                    {
                                        //item is deleted
                                        existingChild.UpdatedAt = DateTime.UtcNow;
                                        existingChild.UpdatedById = userId;
                                        existingChild.IsDeleted = true;
                                        royaltyRepo.Update(existingChild);
                                    }
                                }
                            }
                            #endregion
                            var GstIndRoyalty = viewModel.RoyaltyScales.Select(x => x.IncGST).FirstOrDefault();
                            foreach (var scales in viewModel.RoyaltyScales)
                            {
                                var royalty = MappingHelpers.Mapping<RoyaltyScalesRequestModel, OutletRoyaltyScales>(scales);

                                var existingScale = await royaltyRepo.GetAll(x => x.OutletId == storeId && x.ScalesFrom == scales.ScalesFrom && x.ScalesTo == scales.ScalesTo && !x.IsDeleted && x.Type == RoyaltyScale.Royalty).FirstOrDefaultAsync().ConfigureAwait(false);


                                if (comm.OutletRoyaltyScalesOutlet == null)
                                {
                                    comm.OutletRoyaltyScalesOutlet = new List<OutletRoyaltyScales>();

                                }
                                if (existingScale != null)
                                {
                                    royalty.CreatedById = existingScale.CreatedById;
                                    royalty.CreatedAt = existingScale.CreatedAt;
                                    royalty.UpdatedById = userId;
                                    royalty.Id = existingScale.Id;
                                    royalty.IncGST = GstIndRoyalty;
                                    royalty.OutletId = storeId;
                                    royalty.Type = RoyaltyScale.Royalty;
                                    royaltyRepo.DetachLocal(_ => _.Id == existingScale.Id);
                                    royaltyRepo.Update(royalty);
                                }
                                else
                                {
                                    royalty.CreatedById = userId;
                                    royalty.UpdatedById = userId;
                                    royalty.CreatedAt = DateTime.UtcNow;
                                    royalty.UpdatedAt = DateTime.UtcNow;
                                    royalty.OutletId = storeId;
                                    royalty.IncGST = GstIndRoyalty;
                                    royalty.Type = RoyaltyScale.Royalty;
                                    comm.OutletRoyaltyScalesOutlet.Add(royalty);
                                }
                            }
                        }
                        else if (exists.SupplierOrderingScheduleStore != null)
                        {
                            var OutletSupplierScheduleRepo = _unitOfWork.GetRepository<SupplierOrderingSchedule>();
                            foreach (var existingChild in exists.SupplierOrderingScheduleStore.ToList())
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = userId;
                                existingChild.IsDeleted = true;
                                OutletSupplierScheduleRepo.Update(existingChild);
                            }
                        }

                        #endregion

                        #region advertising royalty scales

                        if (viewModel.AdvertisingRoyaltyScales != null)
                        {
                            var royaltyRepo = _unitOfWork.GetRepository<OutletRoyaltyScales>();

                            if (viewModel?.AdvertisingRoyaltyScales.Select(x => new { x.ScalesFrom, x.ScalesTo }).Distinct().Count() != viewModel?.AdvertisingRoyaltyScales.Count)
                            {
                                //can't add same scale  twice
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyScaaleRepeat.ToString(CultureInfo.CurrentCulture));
                            }

                            #region Delete Children
                            // Delete children
                            if (exists.OutletRoyaltyScalesOutlet.Where(x => x.Type == RoyaltyScale.Advertising).Any())
                            {
                                foreach (var existingChild in exists.OutletRoyaltyScalesOutlet.Where(x => x.Type == RoyaltyScale.Advertising).ToList())
                                {
                                    var child = viewModel.AdvertisingRoyaltyScales.FirstOrDefault(c => c.ScalesFrom == existingChild.ScalesFrom && c.ScalesTo == existingChild.ScalesTo);
                                    if (child == null)
                                    {
                                        //item is deleted
                                        existingChild.UpdatedAt = DateTime.UtcNow;
                                        existingChild.UpdatedById = userId;
                                        existingChild.IsDeleted = true;
                                        royaltyRepo.Update(existingChild);
                                    }
                                }
                            }
                            #endregion
                            var GstIndAdvertisiment = viewModel.AdvertisingRoyaltyScales.Select(x => x.IncGST).FirstOrDefault();
                            foreach (var scales in viewModel.AdvertisingRoyaltyScales)
                            {
                                var royalty = MappingHelpers.Mapping<RoyaltyScalesRequestModel, OutletRoyaltyScales>(scales);

                                var existingScale = await royaltyRepo.GetAll(x => x.OutletId == storeId && x.ScalesFrom == scales.ScalesFrom && x.ScalesTo == scales.ScalesTo && !x.IsDeleted && x.Type == RoyaltyScale.Advertising).FirstOrDefaultAsync().ConfigureAwait(false);


                                if (comm.OutletRoyaltyScalesOutlet == null)
                                {
                                    comm.OutletRoyaltyScalesOutlet = new List<OutletRoyaltyScales>();

                                }
                                if (existingScale != null)
                                {
                                    royalty.CreatedById = existingScale.CreatedById;
                                    royalty.CreatedAt = existingScale.CreatedAt;
                                    royalty.UpdatedById = userId;
                                    royalty.Id = existingScale.Id;
                                    royalty.IncGST = GstIndAdvertisiment;
                                    royalty.OutletId = storeId;
                                    royalty.Type = RoyaltyScale.Advertising;
                                    royaltyRepo.DetachLocal(_ => _.Id == existingScale.Id);
                                    royaltyRepo.Update(royalty);
                                }
                                else
                                {
                                    royalty.CreatedAt = DateTime.UtcNow;
                                    royalty.UpdatedAt = DateTime.UtcNow;
                                    royalty.CreatedById = userId;
                                    royalty.UpdatedById = userId;
                                    royalty.OutletId = storeId;
                                    royalty.Type = RoyaltyScale.Advertising;
                                    royalty.IncGST = GstIndAdvertisiment;
                                    comm.OutletRoyaltyScalesOutlet.Add(royalty);
                                }
                            }
                        }
                        else if (exists.SupplierOrderingScheduleStore != null)
                        {
                            var OutletSupplierScheduleRepo = _unitOfWork.GetRepository<SupplierOrderingSchedule>();
                            foreach (var existingChild in exists.SupplierOrderingScheduleStore.ToList())
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = userId;
                                existingChild.IsDeleted = true;
                                OutletSupplierScheduleRepo.Update(existingChild);
                            }
                        }


                        #endregion

                        //Detaching tracked entry - exists
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);


                        if (viewModel.StoreTradingHours != null)
                        {
                            var tradeRepo = _unitOfWork.GetRepository<OutletTradingHours>();

                            var tradexists = await tradeRepo.GetAll(x => !x.IsDeleted && x.OuteltId == storeId).FirstOrDefaultAsync().ConfigureAwait(false);

                            var tradingHrs = _iAutoMapper.Mapping<StoreTradingHoursRequest, OutletTradingHours>(viewModel.StoreTradingHours);

                            if (tradexists != null)
                            {
                                tradingHrs.Id = tradexists.Id;
                                tradingHrs.OuteltId = storeId;
                                tradingHrs.IsDeleted = false;
                                tradingHrs.CreatedById = tradexists.CreatedById;
                                tradingHrs.CreatedAt = tradexists.CreatedAt;
                                tradingHrs.UpdatedById = userId;

                                tradeRepo.DetachLocal(_ => _.Id == tradexists.Id);
                                tradeRepo.Update(tradingHrs);
                            }
                            else
                            {
                                tradingHrs.OuteltId = storeId;
                                tradingHrs.IsDeleted = false;
                                tradingHrs.CreatedById = userId;
                                tradingHrs.UpdatedById = userId;
                                var trdResult = await tradeRepo.InsertAsync(tradingHrs).ConfigureAwait(false);
                            }

                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        }

                        responseModel = await GetStoreById(comm.Id).ConfigureAwait(false);

                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));

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

        public async Task<StoreResponseModel> Insert(StoreRequestModel viewModel, int userId)
        {
            StoreResponseModel responseModel = new StoreResponseModel();
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Store>();
                    if (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.StoreCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.GroupId == 0 || viewModel.GroupId == null)
                    {
                        throw new BadRequestException(ErrorMessages.StoreGroupId.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.GroupId > 0)
                    {
                        var grpRepo = _unitOfWork?.GetRepository<StoreGroup>();
                        if (!await grpRepo.GetAll(x => x.Id == viewModel.GroupId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.AppStoreDetails != null)
                    {
                        if (viewModel.AppStoreDetails.Latitude != null && (viewModel.AppStoreDetails.Latitude < -90 || viewModel.AppStoreDetails.Latitude > 90))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidLatitude.ToString(CultureInfo.CurrentCulture));
                        }

                        if (viewModel.AppStoreDetails.Longitude != null && (viewModel.AppStoreDetails.Longitude < -180 || viewModel.AppStoreDetails.Longitude > 180))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidLogitude.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.LabelTypeShelfId > 0 || viewModel.LabelTypeShortId > 0 || viewModel.LabelTypePromoId > 0)
                    {
                        var labelRepository = _unitOfWork?.GetRepository<PrintLabelType>();

                        if (viewModel.LabelTypeShelfId > 0 && !await labelRepository.GetAll(x => x.Id == viewModel.LabelTypeShelfId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidShelfLabel.ToString(CultureInfo.CurrentCulture));
                        }

                        if (viewModel.LabelTypeShortId > 0 && !await labelRepository.GetAll(x => x.Id == viewModel.LabelTypeShortId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidShortLabel.ToString(CultureInfo.CurrentCulture));
                        }

                        if (viewModel.LabelTypePromoId > 0 && !await labelRepository.GetAll(x => x.Id == viewModel.LabelTypePromoId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidPromoLabel.ToString(CultureInfo.CurrentCulture));
                        }

                    }

                    if (viewModel.WarehouseId != null)
                    {

                        var warehouseRepo = _unitOfWork?.GetRepository<Warehouse>();
                        if (!await warehouseRepo.GetAll(x => x.Id == viewModel.WarehouseId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))

                            throw new BadRequestException(ErrorMessages.InvalidWarehouse.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.PriceZoneId != null)
                    {
                        if (viewModel.PriceZoneId > 0)
                        {
                            var masterlist = _unitOfWork?.GetRepository<CostPriceZones>();
                            if (!await masterlist.GetAll().Where(x => x.ID == viewModel.PriceZoneId && x.IsActive == Status.Active
                            && x.Type == CostPriceZoneType.Price).AnyAsyncSafe().ConfigureAwait(false))

                                throw new BadRequestException(ErrorMessages.InvalidPriceZoneId.ToString(CultureInfo.CurrentCulture));
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.InvalidPriceZoneId.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.CostZoneId != null)
                    {
                        if (viewModel.CostZoneId > 0)
                        {
                            var masterlist = _unitOfWork?.GetRepository<CostPriceZones>();
                            if (!await masterlist.GetAll().Where(x => x.ID == viewModel.CostZoneId && x.IsActive == Status.Active
                            && x.Type == CostPriceZoneType.Cost).AnyAsyncSafe().ConfigureAwait(false))
                            {
                                throw new BadRequestException(ErrorMessages.InvalidCostZoneId.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCostZoneId.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.RoyaltyScales != null)
                    {

                        if (viewModel?.RoyaltyScales.Select(x => x.ScalesFrom).Distinct().Count() > 5)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidRoyaltyCount.ToString(CultureInfo.CurrentCulture));
                        }

                        for (int i = 0; i < viewModel.RoyaltyScales.Count - 1; i++)
                        {
                            if (viewModel.RoyaltyScales[i].ScalesFrom > viewModel.RoyaltyScales[i].ScalesTo)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyScale.ToString(CultureInfo.CurrentCulture));
                            }

                            if (viewModel.RoyaltyScales[i].Percent <= 0)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyPerct.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                    }


                    if (viewModel.AdvertisingRoyaltyScales != null)
                    {

                        if (viewModel?.AdvertisingRoyaltyScales.Select(x => x.ScalesFrom).Distinct().Count() > 5)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidRoyaltyCount.ToString(CultureInfo.CurrentCulture));
                        }

                        for (int i = 0; i < viewModel.AdvertisingRoyaltyScales.Count - 1; i++)
                        {
                            if (viewModel.AdvertisingRoyaltyScales[i].ScalesFrom > viewModel.AdvertisingRoyaltyScales[i].ScalesTo)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidAdvRoyaltyScale.ToString(CultureInfo.CurrentCulture));
                            }

                            if (viewModel.AdvertisingRoyaltyScales[i].Percent <= 0)
                            {
                                throw new BadRequestException(ErrorMessages.InvalidRoyaltyPerct.ToString(CultureInfo.CurrentCulture));
                            }
                            //for (int j = i + 1; j < viewModel.AdvertisingRoyaltyScales.Count; j++)
                            //{
                            //    if (viewModel.AdvertisingRoyaltyScales[j].ScalesFrom <= viewModel.AdvertisingRoyaltyScales[i].ScalesTo)
                            //    {
                            //        throw new BadRequestException(ErrorMessages.InvalidRoyaltyAdvScalePrev.ToString(CultureInfo.CurrentCulture));
                            //    }
                            //}
                        }
                    }

                    var comm = MappingHelpers.CreateRequestMap(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    if (comm.SupplierOrderingScheduleStore == null)
                        comm.SupplierOrderingScheduleStore = new List<SupplierOrderingSchedule>();

                    if (viewModel.OutletSupplierSchedules != null)
                    {

                        foreach (var setting in viewModel.OutletSupplierSchedules)
                        {
                            var supplierRepository = _unitOfWork?.GetRepository<Supplier>();
                            if (!await supplierRepository.GetAll(x => x.Id == setting.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                            {
                                throw new BadRequestException($"{ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture)} :{setting.SupplierId}");
                            }

                            var schedule = MappingHelpers.Mapping<SupplierOrderScheduleRequestModel, SupplierOrderingSchedule>(setting);
                            //schedule.StoreId = comm.Id;
                            schedule.CreatedById = userId;
                            schedule.UpdatedById = userId;
                            schedule.UpdatedAt = DateTime.UtcNow;
                            schedule.CreatedAt = DateTime.UtcNow;


                            comm.SupplierOrderingScheduleStore.Add(schedule);
                        }

                    }

                    if (comm.OutletRoyaltyScalesOutlet == null)
                        comm.OutletRoyaltyScalesOutlet = new List<OutletRoyaltyScales>();

                    if (viewModel.RoyaltyScales != null)
                    {
                        var GstIndRoyalty = viewModel.RoyaltyScales.Select(x => x.IncGST).FirstOrDefault();
                        foreach (var item in viewModel?.RoyaltyScales)
                        {
                            var scale = MappingHelpers.Mapping<RoyaltyScalesRequestModel, OutletRoyaltyScales>(item);
                            scale.CreatedById = userId;
                            scale.UpdatedById = userId;
                            scale.CreatedAt = DateTime.UtcNow;
                            scale.UpdatedAt = DateTime.UtcNow;
                            scale.IncGST = GstIndRoyalty;
                            scale.Type = RoyaltyScale.Royalty;
                            comm.OutletRoyaltyScalesOutlet.Add(scale);
                        }
                    }

                    if (viewModel.AdvertisingRoyaltyScales != null)
                    {
                        var GstIndAdvertisiment = viewModel.AdvertisingRoyaltyScales.Select(x => x.IncGST).FirstOrDefault();
                        foreach (var item in viewModel?.AdvertisingRoyaltyScales)
                        {
                            var scale = MappingHelpers.Mapping<RoyaltyScalesRequestModel, OutletRoyaltyScales>(item);
                            scale.CreatedById = userId;
                            scale.UpdatedById = userId;
                            scale.CreatedAt = DateTime.UtcNow;
                            scale.UpdatedAt = DateTime.UtcNow;
                            scale.IncGST = GstIndAdvertisiment;
                            scale.Type = RoyaltyScale.Advertising;
                            comm.OutletRoyaltyScalesOutlet.Add(scale);
                        }
                    }

                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        if (result.Id > 0)
                        {
                            if (viewModel.StoreTradingHours != null)
                            {

                                var tradeRepo = _unitOfWork.GetRepository<OutletTradingHours>();

                                var tradingHrs = _iAutoMapper.Mapping<StoreTradingHoursRequest, OutletTradingHours>(viewModel.StoreTradingHours);
                                tradingHrs.OuteltId = result.Id;
                                tradingHrs.IsDeleted = false;
                                tradingHrs.CreatedById = userId;
                                tradingHrs.UpdatedById = userId;
                                var trdResult = await tradeRepo.InsertAsync(tradingHrs).ConfigureAwait(false);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            }

                        }

                        responseModel = await GetStoreById(result.Id).ConfigureAwait(false);

                    }
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
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

        public async Task<KPIReportResponseModel> KPIStorReport(KPIReportInputModel inputModel)
        {

            if (inputModel == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                if (inputModel.StartDate == null || inputModel.EndDate == null)
                {
                    throw new BadRequestException(ErrorMessages.DateRangeNotSelected.ToString(CultureInfo.CurrentCulture));
                }

                var response = new KPIReportResponseModel();
                var repository = _unitOfWork?.GetRepository<Store>();

                //DateTime DateTYFrom = inputModel.StartDate;
                //DateTime DateTYTo = inputModel.EndDate;

                //var vMonth = DateTYTo.Month;
                //var vYear = DateTYTo.Year;
                //if (vMonth < 7)
                //{ vYear = vYear - 1; }


                //DateTYFrom = new DateTime(vYear, 7, 1);
                //if (vYear == 2011)
                //{
                //    DateTYFrom = new DateTime(vYear, 7, 4);
                //}

                //var DateLYFrom = DateTYFrom.AddDays(-364);
                //var DateLYTo = DateTYTo.AddDays(-364);

                List<SqlParameter> dbParams = new List<SqlParameter>{

                new SqlParameter("@StartDate",inputModel?.StartDate),
                new SqlParameter("@EndDate",inputModel?.EndDate),
                new SqlParameter("@OutletIds", inputModel?.StoreIds),
                new SqlParameter("@DepartmentIds", inputModel?.DepartmentIds),
                new SqlParameter("@ZoneIds", inputModel?.ZoneIds),
                new SqlParameter("@CommodityIds", inputModel?.CommodityIds),
                new SqlParameter("@CategoryIds", inputModel?.CategoryIds),
                new SqlParameter("@GroupIds", inputModel?.GroupIds),
                new SqlParameter("@SupplierIds", inputModel?.SupplierIds),
                new SqlParameter("@ManufacturerIds", inputModel?.ManufacturerIds),
                new SqlParameter("@MemberIds", inputModel?.MemberIds)
            };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.KPIStoreReportSalesHistory, dbParams.ToArray()).ConfigureAwait(false);
                if (dset.Tables.Count > 0)
                {
                    DataTable dtStoreKPITable = new DataTable();
                    dtStoreKPITable = dset.Tables[0].Copy();
                    dtStoreKPITable.Clear();
                    DataTable dtDepartmentTable = new DataTable();
                    dtDepartmentTable = dset.Tables[3]?.Copy();
                    int rowCounter = 0;
                    foreach (DataRow item in dset.Tables[0]?.Rows)
                    {
                        dtStoreKPITable.Clear();
                        dtStoreKPITable.Rows.Add(item.ItemArray);
                        var reportResponseModels = MappingHelpers.ConvertDataTable<KPIReportStoreList>(dtStoreKPITable);
                        response.ReportList.AddRange(reportResponseModels);
                        if (dset.Tables.Count > 3)
                        {
                            string expression = "StoreId = " + item["StoreId"];
                            DataRow[] selectedRows = dset?.Tables[3]?.Select(expression);
                            foreach (DataRow itemDept in selectedRows)
                            {
                                dtDepartmentTable.Clear();
                                dtDepartmentTable.Rows.Add(itemDept.ItemArray);
                                var reportDeptResponseModels = MappingHelpers.ConvertDataTable<KPIReportDepartment>(dtDepartmentTable);
                                response.ReportList[rowCounter].DepartmentReport.AddRange(reportDeptResponseModels);
                            }
                            
                        }
                        rowCounter++;
                    }
                }
                //if (dset.Tables.Count > 0)
                //{
                //    var reportResponseModels = MappingHelpers.ConvertDataTable<KPIReportStoreList>(dset.Tables[0]);
                //    response.ReportList.AddRange(reportResponseModels);
                //}
                if (dset.Tables.Count > 1)
                {
                    var reportTotalResponseModels = MappingHelpers.ConvertDataTable<KPIReportTotal>(dset.Tables[1]);
                    response.ReportTotal = reportTotalResponseModels.FirstOrDefault();
                }
                if (dset.Tables.Count > 2)
                {
                    var reportAvgResponseModels = MappingHelpers.ConvertDataTable<KPIReportTotal>(dset.Tables[2]);
                    response.ReportAverage = reportAvgResponseModels.FirstOrDefault();
                }
                //if (dset.Tables.Count > 3)
                //{
                //    var reportDeptResponseModels = MappingHelpers.ConvertDataTable<KPIReportDepartment>(dset.Tables[3]);
                //    response.DepartmentReport.AddRange(reportDeptResponseModels);
                //}
                //var count = 0;
                //count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }

        public async Task<WeeklySalesWorkbookResponseModel> WeeklySalesWorkbook(WeeklySalesWrokBookRequestModel inputModel)
        {
            try
            {
                //bool StoreAccIds = false;
                //var AccessStores = String.Empty;
                //if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                //{
                //    foreach (var storeId in securityViewModel.StoreIds)
                //        AccessStores += storeId + ",";
                //    StoreAccIds = true;
                //}


                var repository = _unitOfWork?.GetRepository<Store>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@StoreIds", string.IsNullOrEmpty(inputModel?.StoreIds) ?null: inputModel?.StoreIds ),
                        new SqlParameter("@TyDateFrom", inputModel?.DateFrom),
                        new SqlParameter("@TyDateTo",inputModel?.DateTo),
                        new SqlParameter("@LyDateFrom",inputModel?.DateFrom.AddDays(-364)),
                        new SqlParameter("@LyDateTo",inputModel?.DateTo.AddDays(-364))
                    };
                WeeklySalesWorkbookResponseModel weeklySalesWorkbookResponse = new WeeklySalesWorkbookResponseModel();
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetWeeklySalesWorkbook, dbParams.ToArray()).ConfigureAwait(false);

                if (dset.Tables.Count > 0)
                {
                    List<WeeklySalesReportResponseModel> weeklySales = MappingHelpers.ConvertDataTable<WeeklySalesReportResponseModel>(dset.Tables[0]);
                    if (weeklySales != null)
                        weeklySalesWorkbookResponse.WeeklySalesReports.AddRange(weeklySales);
                }
                if (dset.Tables.Count > 1)
                {
                    weeklySalesWorkbookResponse.WeeklySalesTotal = MappingHelpers.ConvertDataTable<WeeklySalesTotal>(dset.Tables[1]).FirstOrDefault();

                }
                if (dset.Tables.Count > 2)
                {
                    List<OutletBudgetTargetModel> outletBudgetTargets = MappingHelpers.ConvertDataTable<OutletBudgetTargetModel>(dset.Tables[2]);
                    if (outletBudgetTargets != null)
                        weeklySalesWorkbookResponse.OutletBudgetTargets.AddRange(outletBudgetTargets);
                }
                if (dset.Tables.Count > 3)
                {
                    weeklySalesWorkbookResponse.OutletBudgetTotal = MappingHelpers.ConvertDataTable<OutletBudgetTotal>(dset.Tables[3]).FirstOrDefault();
                }

                return weeklySalesWorkbookResponse;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> ResetLastRun(int outletSupplierId, int userId)
        {
            try
            {
                if (outletSupplierId > 0)
                {
                    var _repository = _unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                    var outletSupplierExist = await _repository.GetAll(x => x.Id == outletSupplierId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (outletSupplierExist != null)
                    {
                        outletSupplierExist.UpdatedById = userId;
                        outletSupplierExist.LastRun = null;

                        _repository?.Update(outletSupplierExist);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceCustomException(ErrorMessages.SupplierOrderingNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceCustomException(ErrorMessages.SupplierOrderingId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Getting All Stores that are not deleted.
        /// </summary>
        /// <returns>List<StoreViewModel></returns>
        public async Task<PagedOutputModel<List<AnonymousStoreResponseModel>>> GetActiveStoresForDigS(PagedInputModel filter = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Store>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", filter?.GlobalFilter),
                        new SqlParameter("@SortColumn", filter?.Sorting),
                        new SqlParameter("@SortDirection", filter?.Direction),
                        new SqlParameter("@PageNumber", filter?.SkipCount),
                        new SqlParameter("@PageSize", filter?.MaxResultCount)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveStoresDig, dbParams.ToArray()).ConfigureAwait(false);
                List<AnonymousStoreResponseModel> storeViewModel = MappingHelpers.ConvertDataTable<AnonymousStoreResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<AnonymousStoreResponseModel>>(storeViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Getting All Stores that are not deleted.
        /// </summary>
        /// <returns>List<StoreViewModel></returns>
        public async Task<AnonymousStoreResponseModel> GetActiveStoreByIdForDigS(int Id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Store>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Id", Id)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveStoresDig, dbParams.ToArray()).ConfigureAwait(false);
                AnonymousStoreResponseModel storeViewModel = MappingHelpers.ConvertDataTable<AnonymousStoreResponseModel>(dset.Tables[0]).FirstOrDefault();

                return storeViewModel;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
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
