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
    public class StoreGroupServices : IStoreGroupServices
    {
        private IUnitOfWork _unitOfWork = null;
        private ILoggerManager _iLoggerManager = null;

        /// <summary>
        /// Setting Values of Private Variables.
        /// </summary>
        /// <param name="context">Database Context ObjectS</param>
        /// <param name="iAutoMapperService">Auto Mapper to Map EntityModel to ViewModel</param>
        public StoreGroupServices(IUnitOfWork context, ILoggerManager iLoggerManager)
        {
            this._unitOfWork = context;
            _iLoggerManager = iLoggerManager;
        }


        /// <summary>
        /// Delete a Store Group with specific Store_Group_Id
        /// </summary>
        /// <param name="Id">Store_Group_Id</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteStoreGroup(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<StoreGroup>();
                    var exists = await repository.GetAll().Include(x => x.Stores).Where(x => x.Id == Id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (exists != null)
                    {
                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;

                        foreach (var store in exists.Stores.ToList())
                        {
                            store.IsDeleted = true;
                            store.UpdatedById = userId;
                            store.UpdatedAt = DateTime.UtcNow;
                        }


                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;

                        throw new NullReferenceException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.StoreGroupId.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of all Active Store Groups
        /// </summary>
        /// <returns>List<StoreGroupViewModel></returns>
        public async Task<PagedOutputModel<List<StoreGroupResponseModel>>> GetAllActiveStoreGroups(PagedInputModel filter)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StoreGroup>();
                var list =await repository.GetAll().Where(x => !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
                int count = 0;
                count = list.Count;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Code.ToLower().Contains(filter.GlobalFilter.ToLower()) || x.Name.ToLower().Contains(filter.GlobalFilter.ToLower())).ToList();

                    if (!string.IsNullOrEmpty((filter?.Status)))
                        list = list.Where(x => x.Status).ToList();

                    list = list.OrderByDescending(x => x.UpdatedAt).ToList();



                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value).ToList();
                    if (!string.IsNullOrEmpty(filter.Sorting))
                    {
                        switch (filter.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => int.Parse(x.Code)).ToList();
                                else
                                    list = list.OrderByDescending(x => int.Parse(x.Code)).ToList();
                                break;
                            case "name":
                            default:
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => int.Parse(x.Code)).ToList();

                                else
                                    list = list.OrderByDescending(x => int.Parse(x.Code)).ToList();
                                break;
                        }
                    }
                }
                
                var listViewModel = list.Select(MappingHelpers.Mapping<StoreGroup, StoreGroupResponseModel>).ToList();

                return new PagedOutputModel<List<StoreGroupResponseModel>>(listViewModel, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get a store group with specific  Store_Group_Id
        /// </summary>
        /// <param name="storeGroupId">Store_Group_Id</param>
        /// <returns>StoreGroupViewModel</returns>
        public async Task<StoreGroupResponseModel> GetStoreGroupsById(int storeGroupId)
        {
            try
            {
                    var repository = _unitOfWork?.GetRepository<StoreGroup>();
                    var storeGroup = await (repository?.GetAll().Where(x => x.Id == storeGroupId && !x.IsDeleted).FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                    if (storeGroup == null)
                    {
                        throw new NotFoundException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    StoreGroupResponseModel storeGroupViewModel;
                    storeGroupViewModel = MappingHelpers.Mapping<StoreGroup, StoreGroupResponseModel>(storeGroup);
                    return storeGroupViewModel;
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

        public async Task<StoreGroupResponseModel> Update(StoreGroupRequestModel viewModel, int groupId, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    StoreGroupResponseModel responseModel = new StoreGroupResponseModel();
                    if (groupId <= 0)
                    {
                        throw new NullReferenceException(ErrorMessages.StoreGroupId.ToString(CultureInfo.CurrentCulture));
                    }

                    var repository = _unitOfWork?.GetRepository<StoreGroup>();
                    if (await repository.GetAll().Where(x => x.Id == groupId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false) == false)
                    {
                        throw new NotFoundException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (await repository.GetAll().Where(x => x.Code == viewModel.Code && x.Id != groupId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.StoreGroupDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetAll().Where(x=>x.Id==groupId).FirstOrDefaultAsyncSafe
                        ().ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
                        }


                        var comm = MappingHelpers.Mapping<StoreGroupRequestModel, StoreGroup>(viewModel);
                        comm.Id = groupId;
                        comm.IsDeleted = false;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;
                        comm.UpdatedById = userId;

                        //Detaching tracked entry - exists
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        responseModel = await GetStoreGroupsById(comm.Id).ConfigureAwait(false);
                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<StoreGroupResponseModel> Insert(StoreGroupRequestModel viewModel, int userId)
        {
            StoreGroupResponseModel responseModel = new StoreGroupResponseModel();
            try
            {
                if (viewModel != null)
                {

                    var repository = _unitOfWork?.GetRepository<StoreGroup>();
                    if (await repository.GetAll().Where(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.StoreGroupDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var comm = MappingHelpers.Mapping<StoreGroupRequestModel, StoreGroup>(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        responseModel = await GetStoreGroupsById(comm.Id).ConfigureAwait(false);
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

        public async Task<PagedOutputModel<List<CorporateTreeResponseModel>>> GetCorporateTree(PagedInputModel filter)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StoreGroup>();
                var list = repository.GetAll(x => !x.IsDeleted);
                int count = 0;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Code.ToLower(CultureInfo.CurrentCulture) == filter.GlobalFilter.ToLower(CultureInfo.CurrentCulture) || x.Name.ToLower(CultureInfo.CurrentCulture) == filter.GlobalFilter.ToLower(CultureInfo.CurrentCulture));

                    list = list.OrderByDescending(x => x.UpdatedAt);

                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
                    if (!string.IsNullOrEmpty(filter.Sorting))
                    {
                        switch (filter.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => int.Parse(x.Code));
                                else
                                    list = list.OrderByDescending(x => int.Parse(x.Code));
                                break;
                            case "name":
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => x.Name);
                                else
                                    list = list.OrderByDescending(x => x.Name);
                                break;
                            default:
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => x.Id);
                                else
                                    list = list.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                var finalList = list.Include(c => c.Stores).ThenInclude(t => t.TillOutlet);

                count = finalList.Count();

                var responseModels = (await finalList.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateCorporateTreeMap).ToList();


                return new PagedOutputModel<List<CorporateTreeResponseModel>>(responseModels, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StoreGroupNotFound.ToString(CultureInfo.CurrentCulture));
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
