using System;
using System.Collections.Generic;
using System.Data;
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

namespace Coyote.Console.App.Services
{
    public class MasterListItemServices : IMasterListItemService
    {
        private IUnitOfWork _unitOfWork = null;
        // private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;
        /// <summary>
        /// MasterListItemServices Constructor, initilize CoyoteAppDBContext,IAutoMappingServices
        /// </summary>
        /// <param name="IMasterListItemRepository"></param>
        /// <param name="iAutoMapperService"></param>
        public MasterListItemServices(IUnitOfWork repo, ILoggerManager iLoggerManager)
        {
            _unitOfWork = repo;
            // _iAutoMapper = iAutoMapperService;
            _iLoggerManager = iLoggerManager;
        }


        /// <summary>
        /// return true or false when List deletion is success or not
        /// </summary>
        /// <param name="id">MasterListItem Id</param>
        /// <returns>bool- true or false</returns>
        public async Task<bool> DeleteMasterListItem(int id, string code, int userId)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<MasterListItems>(); var masterRepo = _unitOfWork?.GetRepository<MasterList>();


                    var masterList = (await masterRepo.GetAll(x => x.Code == code)
                      .FirstOrDefaultAsync().ConfigureAwait(false)); ;
                    if (masterList == null)
                    {
                        throw new NullReferenceException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetAll(x => x.Id == id && x.ListId == masterList.Id && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (!exists.IsDeleted)
                        {
                            exists.UpdatedById = userId;
                            exists.IsDeleted = true;
                            repository?.Update(exists);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceException(ErrorMessages.ListItemIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceException(ErrorMessages.ListItemIdNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.ListItemIdNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// returns all MasterListItem with ListItem_Is_Deleted is false
        /// </summary>
        /// <returns>List of MasterListItem View model</returns>
        public async Task<PagedOutputModel<List<MasterListItemResponseViewModel>>> GetAllActiveMasterListItems(string code, SecurityViewModel securityViewModel, PagedInputModel filter)
        {
            int count = 0;
            try
            {
                var repository = _unitOfWork?.GetRepository<MasterListItems>();
                var masterListItems = repository?.GetAll()?.Include(x => x.MasterList)?.Where(x => !x.IsDeleted && x.MasterList.Code.ToLower() == code.ToLower());
                if (masterListItems == null && masterListItems?.ToList()?.Count == 0)
                {
                    throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                }

                masterListItems = masterListItems.OrderByDescending(x => x.UpdatedAt);
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        masterListItems = masterListItems.Where(x => x.ListId.ToString().ToLower().Contains(filter.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(filter.GlobalFilter.ToLower()));

                    if (code?.ToLower() == "zone" && securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel != null && securityViewModel.ZoneIds != null && securityViewModel.ZoneIds.Count > 0)
                        masterListItems = masterListItems.Where(x => securityViewModel.ZoneIds.Contains(x.ListId));

                    if (!string.IsNullOrEmpty((filter?.Status)))
                        masterListItems = masterListItems.Where(x => x.Status);

                    count = masterListItems.Count();
                    if (filter.MaxResultCount.HasValue)
                    {
                        masterListItems = masterListItems.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
                    }

                    switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "code":
                            if (string.IsNullOrEmpty(filter.Direction))
                                masterListItems = masterListItems.OrderBy(x => x.Code);
                            else
                                masterListItems = masterListItems.OrderByDescending(x => x.Code);
                            break;
                        case "name":
                            if (string.IsNullOrEmpty(filter.Direction))
                                masterListItems = masterListItems.OrderBy(x => x.Name);
                            else
                                masterListItems = masterListItems.OrderByDescending(x => x.Name);
                            break;
                        default:
                            if (string.IsNullOrEmpty(filter.Direction))
                                masterListItems = masterListItems.OrderBy(x => x.ListId);
                            else
                                masterListItems = masterListItems.OrderByDescending(x => x.ListId);
                            break;
                    }
                }

                List<MasterListItemResponseViewModel> masterListItemViewModel;
                masterListItemViewModel = (await masterListItems.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>).ToList();
                return new PagedOutputModel<List<MasterListItemResponseViewModel>>(masterListItemViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ListItemNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// returns all MasterListItem with ListItem_Is_Deleted is false
        /// </summary>
        /// <returns>List of MasterListItem View model</returns>
        public async Task<PagedOutputModel<List<MasterListItemResponseViewModel>>> GetActiveMasterListItems(string code, SecurityViewModel securityViewModel, MasterListFilterModel filter)
        {
            int count = 0;
            int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
            try
            {
                var repository = _unitOfWork?.GetRepository<MasterListItems>();
                var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                var masterList = (await masterRepo.GetAll(x => x.Code.ToLower() == code.ToLower() && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false));
                if (masterList == null)
                {
                    throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                }


                var dset = new DataSet();

                if (filter !=null && filter.ZoneOutlet)
                {
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@SkipCount", filter?.SkipCount),
                new SqlParameter("@MaxResultCount", filter?.MaxResultCount),
                new SqlParameter("@GlobalFilter", filter?.GlobalFilter),
                new SqlParameter("@Status", filter?.Status),
                new SqlParameter("@SortColumn", filter?.Sorting),
                new SqlParameter("@SortDirection", filter?.Direction),
               
            };
                    dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveMasterListZoneOutlet, dbParams.ToArray()).ConfigureAwait(false);
                }
                else
                {
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@ListCode", code),
                new SqlParameter("@SkipCount", filter?.SkipCount),
                new SqlParameter("@MaxResultCount", filter?.MaxResultCount),
                new SqlParameter("@GlobalFilter", filter?.GlobalFilter),
                new SqlParameter("@ListId", masterList.Id),
                new SqlParameter("@Status", filter?.Status),
                new SqlParameter("@SortColumn", filter?.Sorting),
                new SqlParameter("@SortDirection", filter?.Direction),
                new SqlParameter("@IsLogged", filter?.IsLogged),
                new SqlParameter("@RoleId",RoleId),
                 new SqlParameter("@Dashboard", filter?.Dashboard),

            };
                    dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveMasterListItems, dbParams.ToArray()).ConfigureAwait(false);
                }
                List<MasterListItemResponseViewModel> masterListItemViewModel = MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<MasterListItemResponseViewModel>>(masterListItemViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ListItemNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }        /// <summary>
                 /// Get User View Model by MasterList_Id
                 /// </summary>
                 /// <param name="id">MasterListItem Id</param>
                 /// <returns></returns>
        public async Task<MasterListItemResponseViewModel> GetMasterListItemsById(int id, string masterCode)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<MasterListItems>();

                    var test = await (repository?.GetAll().ToListAsyncSafe()).ConfigureAwait(false);


                    var masterlistItems = await repository.GetAll().Include(c => c.MasterList).Where(x => x.Id == id && !x.IsDeleted && x.MasterList.Code.ToLower() == masterCode.ToLower() && !x.MasterList.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (masterlistItems == null)
                    {
                        throw new NotFoundException(ErrorMessages.ListItemNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var MasterListItemViewModel = MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>(masterlistItems);
                    return MasterListItemViewModel;
                }
                throw new NullReferenceException(ErrorMessages.ListItemIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> Update(MasterListItemRequestModel viewModel, string masterCode, int id, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (id == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.ListItemIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (string.IsNullOrEmpty(viewModel.Name))
                    {
                        throw new BadRequestException(ErrorMessages.DescRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    var repository = _unitOfWork?.GetRepository<MasterListItems>();
                    var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                    var masterList = (await masterRepo.GetAll().Where(x => x.Code == masterCode).FirstOrDefaultAsyncSafe().ConfigureAwait(false));
                    if (masterList == null)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (masterList.AccessId != MasterListAccess.ReadWrite)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListAccess.ToString(CultureInfo.CurrentCulture));
                    }

                    MasterList masterListObject = await masterRepo.GetById(viewModel.ListId).ConfigureAwait(false);
                    if (masterListObject == null || masterListObject.IsDeleted == true || masterListObject.Id != masterList.Id)
                    {
                        throw new BadRequestException(ErrorMessages.MasterListIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetAll(x => x.Id == id && x.ListId == masterList.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted)
                        {
                            throw new NotFoundException(ErrorMessages.ListItemIdNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (exists.AccessId != MasterListAccess.ReadWrite)
                        {
                            throw new NotFoundException(ErrorMessages.MasterListAccess.ToString(CultureInfo.CurrentCulture));
                        }
                        if (exists.Code != viewModel.Code && (await repository.GetAll(x => x.Code == viewModel.Code && x.Id != id).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new AlreadyExistsException(ErrorMessages.ListItemCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        if (exists.Name != viewModel.Name && (await repository.GetAll(x => x.Name == viewModel.Name && !x.IsDeleted && x.Id != id).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new AlreadyExistsException(ErrorMessages.ListItemNameDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        var comm = MappingHelpers.Mapping<MasterListItemRequestModel, MasterListItems>(viewModel);
                        comm.Id = id;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;
                        comm.UpdatedAt = DateTime.UtcNow;
                        comm.IsDeleted = false;
                        comm.UpdatedById = userId;
                        //Detaching tracked entry - exists
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NotFoundException(ErrorMessages.ListItemNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<MasterListItemResponseViewModel> AddMasterListItem(MasterListItemRequestModel viewModel, string masterCode, int userId)
        {
            try
            {
                MasterListItemResponseViewModel response = new MasterListItemResponseViewModel();
                if (viewModel != null)
                {
                    if (string.IsNullOrEmpty(viewModel.Name))
                    {
                        throw new BadRequestException(ErrorMessages.DescRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    var repository = _unitOfWork?.GetRepository<MasterListItems>();
                    var masterRepo = _unitOfWork?.GetRepository<MasterList>();


                    if ((await repository.GetAll().Where(x => x.Code == viewModel.Code && x.ListId == viewModel.ListId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.ListItemCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((await repository.GetAll().Where(x => x.Name == viewModel.Name && x.ListId == viewModel.ListId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.ListItemNameDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var masterList = (await masterRepo.GetAll().Where(x => x.Code == masterCode && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false));
                    if (masterList == null)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (masterList.AccessId != MasterListAccess.ReadWrite)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListAccess.ToString(CultureInfo.CurrentCulture));
                    }

                    var masterListObject = await masterRepo.GetAll().Where(x => x.Id == viewModel.ListId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (masterListObject == null)
                    {
                        throw new BadRequestException(ErrorMessages.MasterListIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (masterListObject.Id != masterList.Id)
                    {
                        throw new BadRequestException(ErrorMessages.MasterListInvalid.ToString(CultureInfo.CurrentCulture));

                    }
                    var comm = MappingHelpers.Mapping<MasterListItemRequestModel, MasterListItems>(viewModel);
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    comm.IsDeleted = false;
                    comm.CreatedAt = DateTime.UtcNow;
                    comm.UpdatedAt = DateTime.UtcNow;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        response = await GetMasterListItemsById(result.Id, masterCode).ConfigureAwait(false);
                    }
                }
                return response;
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

        //To Check

        //  DeptMapType
        //ZoneOutlet
        //  WarehousMasterListItem
        //  ProductGroupMasterListItem
        //  ProductCategoryMasterListItem
        //  ProductManufacturerMasterListItem
        //  ProductTypeMasterListItem
        //  ProductNationalRangeMasterListItem
        //  ProductUnitMeasureMasterListItem
        //  PromotionTypeMasterListItem
        //  PromotionSourceMasterListItem
        //  PromotionZoneMasterListItem
        //  PromotionFrequencyMasterListItem
        //  KeypadButtonsMasterListItem
        //  ModuleNameMasterListItem
        //  ActionTypeMasterListItem
        //  CashierTypeMasterListItem
        //  CashierZoneMasterListItem
        //  CashierAccessLevelMasterListItem
        //  TillTypeMasterListItem
        //  OrderTypeMasterListItem
        //  OrderStatusMasterListItem
        //  OrderCreationTypeMasterListItem
        //  OrderDetailTypeMasterListItem
        //  PrintLabelTypeMasterListItem
        //  StockAdjustDetailListItem
        //  CompetitionDetailsZoneMasterListItem
        //  CompetitionDetailsTypeMasterLisItem
        //  CompetitionDetailsResetMasterListItem
        //  CompetitionDetailTriggerTypeMasterList
        //   CompetitionTriggerGroupMasterList
        //  CompetitionRewardTypeMasterList
        //   OutletSupplierSettingStateListItem
        //   OutletSupplierSettingDivisionListItem
        //   AccountTypeMasterListItem
        //   TransactionManufacturers
        //   TransactionCategory
        //   POSMessagesZone
    }
}
