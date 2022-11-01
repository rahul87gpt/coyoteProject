using System;
using System.Collections.Generic;
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
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class GLAccountServices : IGLAccountServices
    {
        private IUnitOfWork _unitOfWork = null;

        public GLAccountServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all Active GL Account.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<GLAccountResponseModel>>> GetAllActiveGLAccount(GLAccountFilters inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<GLAccount>();
                var list = repository.GetAll(x => !x.IsDeleted);

                var count = list.Count(); 
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.AccountNumber.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.AccountSystem.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                  
                    if (!string.IsNullOrEmpty((inputModel?.StoreId)))
                        list = list.Where(x => x.StoreId.ToString().ToLower().Equals(inputModel.StoreId.ToLower()));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    count = list.Count();

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Desc);
                                else
                                    list = list.OrderByDescending(x => x.Desc);
                                break;
                            case "accountsystem":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.AccountSystem);
                                else
                                    list = list.OrderByDescending(x => x.AccountSystem);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Id);
                                else
                                    list = list.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }

            
                
                List<GLAccountResponseModel> accountModel;
                accountModel = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();                
                return new PagedOutputModel<List<GLAccountResponseModel>>(accountModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.GLAccountNotExist.ToString(CultureInfo.CurrentCulture));
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
        /// Get GL Account By Id.
        /// </summary>
        /// <param name="glId"></param>
        /// <returns></returns>
        public async Task<GLAccountResponseModel> GetGLAccountById(long glId)
        {
            try
            {
                if (glId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<GLAccount>();

                    var glAccount = await repository.GetAll(x => x.Id == glId && !x.IsDeleted).Include(c => c.Store).Include(c => c.Supplier).Include(c => c.TypeMasterListItem).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (glAccount == null)
                    {
                        throw new NotFoundException(ErrorMessages.GLAccountNotExist.ToString(CultureInfo.CurrentCulture));
                    }

                    GLAccountResponseModel result = MappingHelpers.CreateMap(glAccount);

                    return result;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.GLAccountId.ToString(CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.GLAccountNotExist.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        /// <summary>
        /// Add new GL Account
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GLAccountResponseModel> Insert(GLAccountRequestModel viewModel, int userId)
        {
            GLAccountResponseModel responseModel = new GLAccountResponseModel();
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.StoreId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var storeRepository = _unitOfWork?.GetRepository<Store>();
                        if (!(await storeRepository.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.SupplierId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var supplierRepository = _unitOfWork?.GetRepository<Supplier>();
                        if (!await supplierRepository.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.TypeId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                        var listId = await masterRepository.GetAll(x => x.Id == viewModel.TypeId && !x.IsDeleted).Include(x => x.MasterList).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (listId == null || listId.MasterList.Code != MasterListCode.GLACCOUNTTYPE)
                            throw new BadRequestException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                var repository = _unitOfWork?.GetRepository<GLAccount>();
                if ((await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.SupplierId==viewModel.SupplierId && x.AccountNumber==viewModel.AccountNumber && x.TypeId==viewModel.TypeId   && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                {
                    throw new AlreadyExistsException(ErrorMessages.GLAccountDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                var glAccount = MappingHelpers.Mapping<GLAccountRequestModel, GLAccount>(viewModel);


                glAccount.AccountSystem = Enum.GetName(typeof(GLAccountSystem), viewModel?.AccountSystemId);
                if (String.IsNullOrEmpty(glAccount.AccountSystem))
                {
                    throw new BadRequestException(ErrorMessages.InvalidAccountSystem.ToString(CultureInfo.CurrentCulture));
                }


                glAccount.CreatedById = userId;
                glAccount.UpdatedById = userId;

                var result = await repository.InsertAsync(glAccount).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (result != null)
                {
                    responseModel = await GetGLAccountById(result.Id).ConfigureAwait(false);
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
        /// Update existing GL Accounts
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="glId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GLAccountResponseModel> Update(GLAccountRequestModel viewModel, int glId, int userId)
        {
            GLAccountResponseModel responseModel = new GLAccountResponseModel();
            try
            {
                if (viewModel != null && glId > 0)
                {
                    if (viewModel.StoreId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var storeRepository = _unitOfWork?.GetRepository<Store>();
                        if (!(await storeRepository.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.SupplierId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var supplierRepository = _unitOfWork?.GetRepository<Supplier>();
                        if (!await supplierRepository.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.TypeId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                        var listId = await masterRepository.GetAll(x => x.Id == viewModel.TypeId && !x.IsDeleted).Include(x => x.MasterList).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (listId == null || listId.MasterList.Code != MasterListCode.GLACCOUNTTYPE)
                            throw new BadRequestException(ErrorMessages.InvalidTypeId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                var repository = _unitOfWork?.GetRepository<GLAccount>();
                if ((await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.SupplierId == viewModel.SupplierId && x.AccountNumber == viewModel.AccountNumber && x.TypeId == viewModel.TypeId && x.Id !=glId &&!x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                {
                    throw new AlreadyExistsException(ErrorMessages.GLAccountDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var glAccount = MappingHelpers.Mapping<GLAccountRequestModel, GLAccount>(viewModel);

                try
                {
                    glAccount.AccountSystem = Enum.GetName(typeof(GLAccountSystem), viewModel?.AccountSystemId);
                }
                catch (Exception)
                {
                    throw new BadRequestException(ErrorMessages.InvalidAccountSystem.ToString(CultureInfo.CurrentCulture));
                }
                glAccount.Id = glId;
                glAccount.CreatedById = userId;
                glAccount.UpdatedById = userId;
                repository.DetachLocal(_ => _.Id == glAccount.Id);
                repository.Update(glAccount);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                responseModel = await GetGLAccountById(glAccount.Id).ConfigureAwait(false);
                return responseModel;
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
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete GL Account
        /// </summary>
        /// <param name="glId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(long glId, int userId)
        {
            try
            {
                if (glId > 0)
                {
                    var _repository = _unitOfWork?.GetRepository<GLAccount>();
                    var accountExist = await _repository.GetAll(x => x.Id == glId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (accountExist != null)
                    {
                        if (!accountExist.IsDeleted)
                        {
                            accountExist.UpdatedById = userId;
                            accountExist.IsDeleted = true;
                            _repository?.Update(accountExist);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.GLAccountNotExist.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.GLAccountId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.GLAccountNotExist.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}