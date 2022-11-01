using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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

public class TillServices : ITillServices
{
    private IUnitOfWork _unitOfWork = null;
    private IAutoMappingServices _iAutoMapper;

    public TillServices(IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
    {
        _unitOfWork = unitOfWork;
        _iAutoMapper = autoMappingServices;
    }

    /// <summary>
    /// Get all active tills
    /// along with searching , sorting and pagination
    /// </summary>
    /// <param name="inputModel"></param>
    /// <returns></returns>
    public async Task<PagedOutputModel<List<TillResponseModel>>> GetAllActiveTillAsync(PagedInputModel inputModel, SecurityViewModel securityViewModel)
    {
        try
        {
            var repository = _unitOfWork?.GetRepository<Till>();

            var till = repository.GetAll().Include(c => c.Store).Include(c => c.Keypad).Include(c => c.Type).Where(x => !x.IsDeleted);
            int count = 0;
            if (inputModel != null)
            {
                if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))

                    till = till.Where(x => x.Code.ToLower(CultureInfo.CurrentCulture).Contains(inputModel.GlobalFilter.ToLower(CultureInfo.CurrentCulture)) || x.Desc.ToLower(CultureInfo.CurrentCulture).Contains(inputModel.GlobalFilter.ToLower(CultureInfo.CurrentCulture)) || x.SerialNo.ToString(CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture).Contains(inputModel.GlobalFilter.ToLower(CultureInfo.CurrentCulture)));

                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                    till = till.Where(x => securityViewModel.StoreIds.Contains(x.OutletId));

                if (!string.IsNullOrEmpty((inputModel?.Status)))
                    till = till.Where(x => x.Status);

                till = till.OrderByDescending(x => x.UpdatedAt);
                count = till.Count();

                if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                    till = till.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                if (!string.IsNullOrEmpty(inputModel.Sorting))
                {
                    switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "code":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                till = till.OrderBy(x => x.Code);
                            else
                                till = till.OrderByDescending(x => x.Code);
                            break;
                        case "desc":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                till = till.OrderBy(x => x.Desc);
                            else
                                till = till.OrderByDescending(x => x.Desc);
                            break;
                        default:
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                till = till.OrderBy(x => x.Id);
                            else
                                till = till.OrderByDescending(x => x.Id);
                            break;
                    }
                }

            }

            List<TillResponseModel> tillModels;
            tillModels = (await till.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

            return new PagedOutputModel<List<TillResponseModel>>(tillModels, count);
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


    public async Task<PagedOutputModel<List<TillResponseModel>>> GetAllTills(PagedInputModel filter, SecurityViewModel securityViewModel)
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
            int count = 0;
            var repository = _unitOfWork?.GetRepository<Till>();

            List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", filter?.GlobalFilter),
                        new SqlParameter("@StoreIds", StoreIds?AccessStores:null),
                        new SqlParameter("@SkipCount", filter?.SkipCount),
                        new SqlParameter("@MaxResultCount", filter?.MaxResultCount),
                        new SqlParameter("@SortColumn", filter?.Sorting),
                        new SqlParameter("@SortDirection", filter?.Direction),
                    };

            var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllTill, dbParams.ToArray()).ConfigureAwait(false);
            List<TillResponseModel> tillModels = MappingHelpers.ConvertDataTable<TillResponseModel>(dset.Tables[0]);
            count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);


            return new PagedOutputModel<List<TillResponseModel>>(tillModels, count);
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
    /// Get active till by Id
    /// </summary>
    /// <param name="tillId"></param>
    /// <returns></returns>
    public async Task<TillResponseModel> GetTillById(int tillId)
    {
        try
        {
            if (tillId > 0)
            {
                var repository = _unitOfWork?.GetRepository<Till>();

                var till = await repository.GetAll(x => x.Id == tillId && !x.IsDeleted).Include(c => c.Keypad).Include(c => c.Type).Include(c => c.Store).FirstOrDefaultAsync().ConfigureAwait(false);
                if (till == null)
                {
                    throw new NotFoundException(ErrorMessages.TillNotFound.ToString(CultureInfo.CurrentCulture));
                }

                TillResponseModel tillModel = MappingHelpers.CreateMap(till);

                return tillModel;
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

    /// <summary>
    /// Add new till
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<TillResponseModel> Insert(TillRequestModel viewModel, int userId)
    {

        TillResponseModel responseModel = new TillResponseModel();
        try
        {
            if (viewModel != null)
            {

                if (viewModel.KeypadId > 0)
                {
                    var keypadRepository = _unitOfWork?.GetRepository<Keypad>();

                    if (!(await keypadRepository.GetAll(x => x.Id == viewModel.KeypadId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture));
                }
                if (viewModel.OutletId > 0)
                {
                    var outletRepository = _unitOfWork?.GetRepository<Store>();

                    if (!(await outletRepository.GetAll(x => x.Id == viewModel.OutletId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.InvalidOutletId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.InvalidOutletId.ToString(CultureInfo.CurrentCulture));
                }
                if (viewModel.TypeId > 0)
                {

                    var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();

                    var listId = await masterListRepostiory.GetAll(x => x.Code == MasterListCode.TillType && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                    var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                    if (!(await masterRepository.GetAll(x => x.Id == viewModel.TypeId && x.ListId == listId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                    }

                }
                else
                {
                    throw new BadRequestException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                }
                var repository = _unitOfWork?.GetRepository<Till>();
                if (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.TillDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var till = _iAutoMapper.Mapping<TillRequestModel, Till>(viewModel);

                till.IsDeleted = false;
                till.CreatedById = userId;
                till.UpdatedById = userId;

                if (string.IsNullOrEmpty(till.SerialNo))
                { till.SerialNo = till.Code; }

                var result = await repository.InsertAsync(till).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (result != null)
                {
                    responseModel = await GetTillById(result.Id).ConfigureAwait(false);
                }
                return responseModel;
            }
            throw new BadRequestException(ErrorMessages.TillInvalidReq.ToString(CultureInfo.CurrentCulture));
        }
        catch (Exception ex)
        {
            if (ex is AlreadyExistsException)
            {
                throw new AlreadyExistsException(ex.Message);
            }
            if (ex is BadRequestException)
            {
                throw new BadRequestException(ex.Message);
            }
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
    /// update existing till
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="tillId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<TillResponseModel> Update(TillRequestModel viewModel, int tillId, int userId)
    {
        TillResponseModel responseModel = new TillResponseModel();
        try
        {
            if (viewModel != null && tillId > 0)
            {

                if (viewModel.KeypadId > 0)
                {
                    var keypadRepository = _unitOfWork?.GetRepository<Keypad>();

                    if (!(await keypadRepository.GetAll(x => x.Id == viewModel.KeypadId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture));
                }
                if (viewModel.OutletId > 0)
                {
                    var outletRepository = _unitOfWork?.GetRepository<Store>();

                    if (!(await outletRepository.GetAll(x => x.Id == viewModel.OutletId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.InvalidOutletId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.InvalidOutletId.ToString(CultureInfo.CurrentCulture));
                }
                if (viewModel.TypeId > 0)
                {

                    var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();

                    var listId = await masterListRepostiory.GetAll(x => x.Code == MasterListCode.TillType && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                    var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                    if (!(await masterRepository.GetAll(x => x.Id == viewModel.TypeId && x.ListId == listId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                    }

                }
                else
                {
                    throw new NotFoundException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                }
                var repository = _unitOfWork?.GetRepository<Till>();
                if (await repository.GetAll(x => x.Code == viewModel.Code && x.Id != tillId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                {
                    throw new NotFoundException(ErrorMessages.TillDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var tillExist = await repository.GetAll(x => x.Id == tillId && !x.IsDeleted).FirstAsync().ConfigureAwait(false);
                if (tillExist != null)
                {
                    var till = _iAutoMapper.Mapping<TillRequestModel, Till>(viewModel);


                    till.Code = tillExist.Code;

                    if (string.IsNullOrEmpty(till.SerialNo))
                    { till.SerialNo = till.Code; }


                    till.Id = tillId;
                    till.CreatedAt = tillExist.CreatedAt;
                    till.IsDeleted = false;
                    till.CreatedById = tillExist.CreatedById;
                    till.UpdatedById = userId;
                    //Detaching tracked entry - exists
                    repository.DetachLocal(_ => _.Id == till.Id);
                    repository.Update(till);

                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                    responseModel = await GetTillById(till.Id).ConfigureAwait(false);
                    return responseModel;

                }
            }
            throw new BadRequestException(ErrorMessages.TillInvalidReq.ToString(CultureInfo.CurrentCulture));
        }
        catch (Exception ex)
        {
            if (ex is AlreadyExistsException)
            {
                throw new AlreadyExistsException(ex.Message);
            }
            if (ex is BadRequestException)
            {
                throw new BadRequestException(ex.Message);
            }
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
    /// Delete an active till
    /// </summary>
    /// <param name="tillId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> Delete(int tillId, int userId)
    {
        try
        {
            if (tillId > 0)
            {
                var _repository = _unitOfWork?.GetRepository<Till>();
                var tillExists = await _repository.GetAll(x => x.Id == tillId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                if (tillExists != null)
                {
                    // tillExists.Code = tillExists.Code + "~" + tillExists.Id;
                    tillExists.UpdatedById = userId;
                    tillExists.IsDeleted = true;
                    _repository?.Update(tillExists);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    return true;
                }
                throw new NullReferenceCustomException(ErrorMessages.TillNotFound.ToString(CultureInfo.CurrentCulture));
            }
            throw new NullReferenceCustomException(ErrorMessages.InvalidTillId.ToString(CultureInfo.CurrentCulture));
        }
        catch (NullReferenceCustomException nre)
        {
            throw new NullReferenceCustomException(nre.Message, nre);
        }
        catch (Exception ex)
        {
            throw new Exception(ErrorMessages.TillNotFound.ToString(CultureInfo.CurrentCulture), ex);
        }
    }

    /// <summary>
    /// Delete multiple tills.
    /// </summary>
    /// <param name="tills">String as array of multiple tillIds.</param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteMultiple(string tills, int userId)
    {
        long tillId = 0;
        try
        {
            if (!string.IsNullOrEmpty(tills))
            {
                var IdsList = tills.Split(',');
                foreach (var Id in IdsList)
                {
                    var _repository = _unitOfWork?.GetRepository<Till>();
                    tillId = Convert.ToInt64(Id);
                    if (tillId > 0)
                    {
                        var tillExist = await _repository.GetAll(x => x.Id == tillId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (tillExist != null)
                        {
                            //  tillExist.Code = (tillExist.Code + "~" + tillExist.Id);
                            tillExist.UpdatedAt = DateTime.UtcNow;
                            tillExist.UpdatedById = userId;
                            tillExist.IsDeleted = true;
                            _repository?.Update(tillExist);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        }
                        throw new NullReferenceCustomException(ErrorMessages.TillNotFound.ToString(CultureInfo.CurrentCulture) + "   " + tillId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                return true;
            }

            throw new NullReferenceCustomException(ErrorMessages.InvalidTillId.ToString(CultureInfo.CurrentCulture) + "   " + tillId.ToString(CultureInfo.CurrentCulture));
        }
        catch (FormatException ex)
        {
            throw new FormatException(ErrorMessages.TillNotFound.ToString(CultureInfo.CurrentCulture), ex);
        }
        catch (NullReferenceCustomException nre)
        {
            throw new NullReferenceCustomException(nre.Message, nre);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorMessages.TillNotFound.ToString(CultureInfo.CurrentCulture), ex);
        }
    }

    public async Task<PagedOutputModel<List<TillSyncResponseModel>>> GetAllTillSync(PagedInputModel inputModel = null, SecurityViewModel securityViewModel = null)
    {
        try
        {
            var repository = _unitOfWork?.GetRepository<TillSync>();

            var till = await repository.GetAll().Include(c => c.Store).Include(c => c.Till).Where(x => !x.IsDeleted && !x.Store.IsDeleted && !x.Till.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
            int count = 0;
            if (inputModel != null)
            {
                if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))

                    till = till.Where(x => x.Store.Code.ToString(CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture).Contains(inputModel.GlobalFilter.ToLower(CultureInfo.CurrentCulture))
                    || x.Till.Code.ToString(CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture).Contains(inputModel.GlobalFilter.ToLower(CultureInfo.CurrentCulture))
                     || x.Till.Desc.ToLower(CultureInfo.CurrentCulture).Contains(inputModel.GlobalFilter.ToLower(CultureInfo.CurrentCulture))).ToList();

                till = till.OrderByDescending(x => x.UpdatedAt).ToList();
                count = till.Count;

                if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                    till = till.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value).ToList();

                if (!string.IsNullOrEmpty(inputModel.Sorting))
                {
                    switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "till":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                till = till.OrderBy(x => x.Till.Code).ToList();
                            else
                                till = till.OrderByDescending(x => x.Till.Code).ToList(); ;
                            break;
                        case "store":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                till = till.OrderBy(x => x.Store.Code).ToList();
                            else
                                till = till.OrderByDescending(x => x.Store.Code).ToList();
                            break;
                        default:
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                till = till.OrderBy(x => x.UpdatedAt).ToList();
                            else
                                till = till.OrderByDescending(x => x.UpdatedAt).ToList();
                            break;
                    }
                }
            }
            List<TillSyncResponseModel> tillModels;
            tillModels = till.Select(MappingHelpers.CreateMap).ToList();

            return new PagedOutputModel<List<TillSyncResponseModel>>(tillModels, count);
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

    public async Task<PagedOutputModel<List<TillSyncResponseModel>>> AddSyncTills(TillSyncRequestModel requestModel, int userId)
    {
        try
        {
            if (requestModel != null)
            {
                if (!requestModel.Cashier && !requestModel.Product && !requestModel.Keypad && !requestModel.Account && !requestModel.RemoveSync)
                {
                    throw new BadRequestException(ErrorMessages.TillInvalidSyncSelect.ToString(CultureInfo.CurrentCulture));
                }

                if (requestModel.RemoveSync && (requestModel.Cashier || requestModel.Product || requestModel.Keypad || requestModel.Account))
                {
                    throw new BadRequestException(ErrorMessages.TillInvalidRemoveSelect.ToString(CultureInfo.CurrentCulture));
                }

                if (requestModel.StoreIds == null)
                {
                    throw new BadRequestException(ErrorMessages.TillInvalidStoreSelect.ToString(CultureInfo.CurrentCulture));
                }

                var tillRepo = _unitOfWork.GetRepository<Till>();
                var tillSyncRepo = _unitOfWork.GetRepository<TillSync>();

                foreach (var storeId in requestModel.StoreIds)
                {
                    var storeRepo = _unitOfWork.GetRepository<Store>();

                    if (!await storeRepo.GetAll(x => !x.IsDeleted && x.Id == storeId).AnyAsync().ConfigureAwait(false))
                    {
                        throw new BadRequestException(ErrorMessages.TillInvalidStore.ToString(CultureInfo.CurrentCulture));
                    }

                    var tillList = await tillRepo.GetAll(x => !x.IsDeleted && x.OutletId == storeId).ToListAsyncSafe().ConfigureAwait(false);

                    foreach (var till in tillList)
                    {
                        var tillExist = await tillSyncRepo.GetAll(x => !x.IsDeleted && x.TillId == till.Id && x.StoreId == storeId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                        if (tillExist != null)
                        {
                            if (requestModel.RemoveSync)
                            {
                                tillExist.Product = null;
                                tillExist.Cashier = null;
                                tillExist.Account = null;
                                tillExist.Keypad = null;
                            }

                            else
                            {
                                if (requestModel.Product)
                                {
                                    tillExist.Product = TillSyncType.SYNC;
                                    tillExist.ClientVersion = Enum.GetName(typeof(TillSyncType), TillSyncType.SYNC).ToString();
                                }
                                else
                                {
                                    tillExist.Product = null;
                                }

                                if (requestModel.Cashier)
                                    tillExist.Cashier = TillSyncType.SYNC;
                                else
                                    tillExist.Cashier = null;

                                if (requestModel.Account)
                                    tillExist.Account = TillSyncType.SYNC;
                                else
                                    tillExist.Account = null;

                                if (requestModel.Keypad)
                                    tillExist.Keypad = TillSyncType.SYNC;
                                else
                                    tillExist.Keypad = null;
                            }

                            tillSyncRepo.DetachLocal(z => z.Id == tillExist.Id);
                            tillSyncRepo.Update(tillExist);
                        }
                        else
                        {
                            var syncTill = new TillSync();

                            if (requestModel.Product)
                            {
                                syncTill.Product = TillSyncType.SYNC;
                                syncTill.ClientVersion = Enum.GetName(typeof(TillSyncType), TillSyncType.SYNC);
                            }

                            if (requestModel.Cashier)
                                syncTill.Cashier = TillSyncType.SYNC;

                            if (requestModel.Account)
                                syncTill.Account = TillSyncType.SYNC;

                            if (requestModel.Keypad)
                                syncTill.Keypad = TillSyncType.SYNC;

                            syncTill.StoreId = storeId;
                            syncTill.TillId = till.Id;
                            syncTill.TillActivity = null;
                            syncTill.CreatedById = userId;
                            syncTill.UpdatedById = userId;

                            await tillSyncRepo.InsertAsync(syncTill).ConfigureAwait(false);
                        }
                    }
                }

                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

            }

            return await GetAllTillSync(null, null).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            if (ex is BadRequestException)
            {
                throw new BadRequestException(ex.Message);
            }

            throw new BadRequestException(ex.Message);
        }


    }

    public async Task<PagedOutputModel<List<TillJournalResponseModel>>> GetTillJournal(TillJournalInputModel inputModel, TillJournalRequestModel requestModel = null, SecurityViewModel securityViewModel = null)
    {
        try
        {
            var AccessStores = String.Empty;
            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
            {
                foreach (var storeId in securityViewModel.StoreIds)
                    AccessStores += storeId + ",";
            }

            if (string.IsNullOrEmpty(inputModel?.JournalType))
            {
                inputModel.JournalType = "TODAY";
            }

            if (requestModel == null)
            {
                requestModel = new TillJournalRequestModel();
            }

            if (inputModel.JournalType.ToUpper(CultureInfo.CurrentCulture) == "TODAY")
            {
                requestModel.StartDate = DateTime.Today;
                requestModel.EndDate = DateTime.Today;
            }
            else if (inputModel.JournalType.ToUpper(CultureInfo.CurrentCulture) != "TODAY" && (requestModel.StartDate == null || requestModel.EndDate == null))
            {
                throw new BadRequestException(ErrorMessages.DateRangeNotSelected.ToString(CultureInfo.CurrentCulture));
            }
            List<TillJournalResponseModel> tillJournalResponses = new List<TillJournalResponseModel>();

            var count = 0;
            var repository = _unitOfWork?.GetRepository<Till>();
            List<SqlParameter> dbParams = new List<SqlParameter>{
                 new SqlParameter("@DateFrom",requestModel?.StartDate),
                 new SqlParameter("@DateTo",requestModel?.EndDate),
                 new SqlParameter("@HourFrom",requestModel?.StartHour),
                 new SqlParameter("@HourTo",requestModel?.EndHour),
                 new SqlParameter("@TransactionType",requestModel?.TransactionType),
                 new SqlParameter("@ExceptionsOnly",inputModel?.ShowException),
                 new SqlParameter("@TillId",requestModel?.TillId),
                 new SqlParameter("@CashierId",requestModel?.CashierId),
                 new SqlParameter("@OutletIds",requestModel?.StoreIds),
                 new SqlParameter("@PromoSales",requestModel?.IsPromoSale),
                 new SqlParameter("@PromoId",requestModel?.PromoId),
                 new SqlParameter("@CommodityIds",requestModel?.CommodityIds),
                 new SqlParameter("@DepartmentIds",requestModel?.DepartmentIds),
                 new SqlParameter("@CategoryIds",requestModel?.CategoryIds),
                 new SqlParameter("@ManufacturerIds",requestModel?.ManufacturerIds),
                 new SqlParameter("@GroupIds",requestModel?.GroupIds),
                 new SqlParameter("@SupplierIds",requestModel?.SupplierIds),
                 new SqlParameter("@AccessOutletIds",AccessStores),
                 new SqlParameter("@JournalRange",inputModel?.JournalType.ToUpper(CultureInfo.CurrentCulture)),
                 new SqlParameter("@SkipCount",inputModel?.SkipCount),
                 new SqlParameter("@MaxResultCount",inputModel?.MaxResultCount),
                 new SqlParameter("@MemberIds",requestModel?.MemberIds)
            };
            var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetTillJournal, dbParams.ToArray()).ConfigureAwait(false);
            tillJournalResponses = MappingHelpers.ConvertDataTable<TillJournalResponseModel>(dset.Tables[0]);
            count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

            return new PagedOutputModel<List<TillJournalResponseModel>>(tillJournalResponses, count);
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

}


