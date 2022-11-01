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
    public class XeroAccountServices : IXeroAccountServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public XeroAccountServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Add new Xero Account
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<XeroAccountReponseModel> Insert(XeroAccountRequestModel viewModel, int userId)
        {
            XeroAccountReponseModel responseModel = new XeroAccountReponseModel();
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<XeroAccount>();
                    if (viewModel.StoreId > 0)
                    {
                        if ((await repository.GetAll(x => x.StoreId == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new AlreadyExistsException(ErrorMessages.XeroAccDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        var storeRepository = _unitOfWork?.GetRepository<Store>();

                        if (!(await storeRepository.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else {
                        throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                    }


                    var xeroAcc = MappingHelpers.Mapping<XeroAccountRequestModel, XeroAccount>(viewModel);

                    xeroAcc.CreatedById = userId;
                    xeroAcc.UpdatedById = userId;

                    var result = await repository.InsertAsync(xeroAcc).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        responseModel = await GetXeroAccountById(result.Id).ConfigureAwait(false);
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
        /// Update existing Xero Account
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="xeroId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<XeroAccountReponseModel> Update(XeroAccountRequestModel viewModel, int xeroId, int userId)
        {
            XeroAccountReponseModel responseModel = new XeroAccountReponseModel();
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<XeroAccount>();
                    if (viewModel.StoreId > 0)
                    {
                        if (!(await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.Id == xeroId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.XeroAccDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        var storeRepository = _unitOfWork?.GetRepository<Store>();

                        if (!(await storeRepository.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                        }



                        var xeroAccExist = await repository.GetAll(x => x.Id == xeroId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (xeroAccExist != null)
                        {
                            var xeroAcc = MappingHelpers.Mapping<XeroAccountRequestModel, XeroAccount>(viewModel);
                            xeroAcc.Id = xeroId;
                            xeroAcc.CreatedById = xeroAccExist.CreatedById;
                            xeroAcc.CreatedAt = xeroAccExist.CreatedAt;
                            xeroAcc.UpdatedById = userId;

                            repository.DetachLocal(x => x.Id == xeroAcc.Id);
                            repository.Update(xeroAcc);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            responseModel = await GetXeroAccountById(xeroAcc.Id).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new NotFoundException(ErrorMessages.XeroAccDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else {
                        throw new BadRequestException(ErrorMessages.StoreIdRequired.ToString(CultureInfo.CurrentCulture));
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
        /// Get all active Xero Accounts.
        /// </summary>
        /// <param name="inputModel">To filter, paginate and sort</param>
        /// <param name="securityViewModel">To return according to outelt access.</param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<XeroAccountReponseModel>>> GetAllActiveXeroAccount(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<XeroAccount>();

                var list = repository.GetAll(x => !x.IsDeleted, null, includes: new Expression<Func<XeroAccount, object>>[] { c => c.Store });

                var test = await list.ToListAsyncSafe().ConfigureAwait(false);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        list = list.Where(x => securityViewModel.StoreIds.Contains(x.StoreId));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            //case "code":
                            //    if (string.IsNullOrEmpty(inputModel.Direction))
                            //        list = list.OrderBy(x => x.Code);
                            //    else
                            //        list = list.OrderByDescending(x => x.Code);
                            //    break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Desc);
                                else
                                    list = list.OrderByDescending(x => x.Desc);
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

                List<XeroAccountReponseModel> xeroAccounts;
                xeroAccounts = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<XeroAccountReponseModel>>(xeroAccounts, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.XeroAccNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get Xero Account by Id
        /// </summary>
        /// <param name="xeroId"></param>
        /// <returns></returns>
        public async Task<XeroAccountReponseModel> GetXeroAccountById(long xeroId)
        {
            try
            {
                if (xeroId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<XeroAccount>();

                    var account = await repository.GetAll(x => x.Id == xeroId && !x.IsDeleted).Include(c => c.Store).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (account == null)
                    {
                        throw new NotFoundException(ErrorMessages.XeroAccNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    XeroAccountReponseModel reponseModel = MappingHelpers.CreateMap(account);

                    return reponseModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.XeroAccId.ToString(CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.XeroAccNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete Existing Xero Account
        /// </summary>
        /// <param name="xeroId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(long xeroId, int userId)
        {
            try
            {
                if (xeroId > 0)
                {
                    var _repository = _unitOfWork?.GetRepository<XeroAccount>();
                    var xeroAccExist = await _repository.GetAll(x => x.Id == xeroId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (xeroAccExist != null)
                    {
                        if (!xeroAccExist.IsDeleted)
                        {
                            xeroAccExist.Desc = xeroAccExist.Desc + "`" + xeroAccExist.Id.ToString();
                            xeroAccExist.UpdatedById = userId;
                            xeroAccExist.IsDeleted = true;
                            _repository?.Update(xeroAccExist);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.XeroAccNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.XeroAccId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.XeroAccNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
