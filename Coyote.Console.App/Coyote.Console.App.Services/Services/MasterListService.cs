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
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
    public class MasterListService : IMasterListService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;
        /// <summary>
        /// MasterListService Constructor, initilize IMasterListRepository,IAutoMappingServices
        /// </summary>
        /// <param name="IMasterListRepository"></param>
        /// <param name="iAutoMapperService"></param>
        public MasterListService(IUnitOfWork repo, IAutoMappingServices iAutoMapperService, ILoggerManager iLoggerManager)
        {
            _unitOfWork = repo;
            _iAutoMapper = iAutoMapperService;
            _iLoggerManager = iLoggerManager;
        }


        /// <summary>
        /// returns all MasterList with MasterList_Is_Deleted is false
        /// </summary>
        /// <returns>List of MasterList View model</returns>
        public async Task<PagedOutputModel<List<MasterListResponseViewModel>>> GetAllActiveMasterLists(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<MasterList>();
                var masterList = repository.GetAll().Where(x => !x.IsDeleted && x.AccessId != MasterListAccess.Hidden);

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        masterList = masterList.Where(x => x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Name.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    masterList = masterList.OrderByDescending(x => x.UpdatedAt);
                    count = masterList.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        masterList = masterList.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    masterList = masterList.OrderBy(x => x.Code);
                                else
                                    masterList = masterList.OrderByDescending(x => x.Code);
                                break;
                            case "name":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    masterList = masterList.OrderBy(x => x.Name);
                                else
                                    masterList = masterList.OrderByDescending(x => x.Name);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    masterList = masterList.OrderBy(x => x.Id);
                                else
                                    masterList = masterList.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                    else {
                        if (string.IsNullOrEmpty(inputModel.Direction))
                            masterList = masterList.OrderBy(x => x.Code);
                        else
                            masterList = masterList.OrderByDescending(x => x.Code);
                    }
                }
                List<MasterListResponseViewModel> masterListViewModel;
                masterListViewModel = (await masterList.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<MasterList, MasterListResponseViewModel>).ToList();
                return new PagedOutputModel<List<MasterListResponseViewModel>>(masterListViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get User View Model by MasterList_Id
        /// </summary>
        /// <param name="id">MasterList Id</param>
        /// <returns></returns>
        public async Task<MasterListResponseViewModel> GetMasterListById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<MasterList>();
                    var masterList = await repository.GetAll().Where(x => x.Id == id && !x.IsDeleted && x.AccessId != MasterListAccess.Hidden).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (masterList == null)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (masterList.IsDeleted)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    MasterListResponseViewModel masterListViewModel = MappingHelpers.Mapping<MasterList, MasterListResponseViewModel>(masterList);
                    return masterListViewModel;
                }
                throw new NullReferenceException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get User View Model by MasterList_Code
        /// </summary>
        /// <param name="Code">MasterList Code</param>
        /// <returns></returns>
        public async Task<MasterListResponseViewModel> GetMasterListByCode(string Code)
        {
            try
            {
                if (!string.IsNullOrEmpty(Code))
                {
                    var repository = _unitOfWork?.GetRepository<MasterList>();
                    var masterList = (await repository.GetAll().Where(x => !x.IsDeleted && x.Code == Code && x.AccessId != MasterListAccess.Hidden).FirstOrDefaultAsync().ConfigureAwait(false));
                    if (masterList == null)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    MasterListResponseViewModel masterListViewModel = _iAutoMapper.Mapping<MasterList, MasterListResponseViewModel>(masterList);
                    return masterListViewModel;
                }
                throw new NullReferenceException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> Update(MasterListRequestModel viewModel, int id, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (id > 0)
                    {
                        var repository = _unitOfWork?.GetRepository<MasterList>();
                        var exists = await repository.GetAll().Where(x => x.Id == id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                        if (exists != null)
                        {
                            if (exists.IsDeleted == true)
                            {
                                throw new AlreadyExistsException(ErrorMessages.MasterListIdNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            if (exists.AccessId != MasterListAccess.ReadWrite)
                            {
                                throw new AlreadyExistsException(ErrorMessages.MasterListAccess.ToString(CultureInfo.CurrentCulture));
                            }
                            if (exists.Code != viewModel.Code && (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted && x.Id != id).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new AlreadyExistsException(ErrorMessages.MasterListCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                            }
                            if (exists.Name != viewModel.Name && (await repository.GetAll(x => x.Name == viewModel.Name).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new AlreadyExistsException(ErrorMessages.MasterListNameDuplicate.ToString(CultureInfo.CurrentCulture));
                            }
                            var ml = _iAutoMapper.Mapping<MasterListRequestModel, MasterList>(viewModel);
                            ml.Id = id;
                            ml.IsDeleted = false;
                            ml.CreatedAt = exists.CreatedAt;
                            ml.CreatedById = exists.CreatedById;
                            ml.UpdatedAt = DateTime.UtcNow;
                            ml.UpdatedById = userId;
                            //Detaching tracked entry - exists
                            repository.DetachLocal(_ => _.Id == ml.Id);
                            repository.Update(ml);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NotFoundException(ErrorMessages.MasterListIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceException(ErrorMessages.UpdatedByRequired.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
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

        public async Task<int> Insert(MasterListRequestModel viewModel, int userId)
        {
            int resultId = 0;
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<MasterList>();
                    if ((await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.MasterListCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((await repository.GetAll(x => x.Name == viewModel.Name).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.MasterListNameDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var ml = _iAutoMapper.Mapping<MasterListRequestModel, MasterList>(viewModel);
                    ml.IsDeleted = false;
                    ml.CreatedAt = DateTime.UtcNow;
                    ml.UpdatedAt = DateTime.UtcNow;
                    ml.CreatedById = userId;
                    ml.UpdatedById = userId;
                    var result = await repository.InsertAsync(ml).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        resultId = result.Id;
                    }
                }
                return resultId;
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

        /// <summary>
        /// return true or false when List deletion is success or not
        /// </summary>
        /// <param name="id">MasterList Id</param>
        /// <returns>bool- true or false</returns>
        public async Task<bool> DeleteMasterList(int id, int userId)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<MasterList>();
                    var exists = await repository.GetById(id).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        if (exists.Status)
                        {
                            exists.UpdatedById = userId;
                            exists.IsDeleted = true;
                            //exists.Code = (exists.Code + "~" + exists.Id);

                            repository?.Update(exists);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.MasterListIdNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// GetNationalLevelSalesPeriod
        /// </summary>
        /// <returns>List</returns>
        public async Task<List<string>> GetNationalLevelSalesPeriod()
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<CSCPeriod>();
                return await repository.GetAll().Where(x => x.IsActive == Status.Active).Select(x => x.ID + " : " + x.Heading + " (" + x.From + " - " + x.To + ")").ToListAsyncSafe().ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
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
