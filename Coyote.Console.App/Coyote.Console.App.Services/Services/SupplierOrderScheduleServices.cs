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
    public class SupplierOrderScheduleServices : ISupplierOrderScheduleServices
    {
        private IUnitOfWork unitOfWork;

        public SupplierOrderScheduleServices(IUnitOfWork iunitofWork)
        {
            unitOfWork = iunitofWork;
        }

        public async Task<PagedOutputModel<List<SupplierOrderScheduleResponseModel>>> GetAllActiveSchedule(PagedInputModel inputModel)
        {

            try
            {
                var repository = unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                int count = 0;
                var list = repository.GetAll(x => !x.IsDeleted, includes: new Expression<Func<SupplierOrderingSchedule, object>>[] { c => c.Supplier, c => c.Store });
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.StoreId.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.LastRun.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.CoverDays.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "coverday":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.CoverDays);
                                else
                                    list = list.OrderByDescending(x => x.CoverDays);
                                break;
                            case "lastrun":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.LastRun);
                                else
                                    list = list.OrderByDescending(x => x.LastRun);
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

                count = list.Count();
                List<SupplierOrderScheduleResponseModel> responseModels;
                responseModels = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<SupplierOrderScheduleResponseModel>>(responseModels, count);
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

        public async Task<SupplierOrderScheduleResponseModel> GetSupplierOrderScheduleById(long soId)
        {
            try
            {
                if (soId > 0)
                {
                    var repository = unitOfWork?.GetRepository<SupplierOrderingSchedule>();

                    var list = await repository.GetAll(x => x.Id == soId && !x.IsDeleted).Include(c => c.Store).Include(c => c.Supplier).Include(c => c.Store).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (list == null)
                    {
                        throw new NotFoundException(ErrorMessages.SupplierOrderingNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    SupplierOrderScheduleResponseModel result = MappingHelpers.CreateMap(list);

                    return result;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.SupplierOrderingId.ToString(CultureInfo.CurrentCulture));
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

        public async Task<SupplierOrderScheduleResponseModel> Insert(SupplierOrderScheduleRequestModel viewModel, int userId)
        {
            SupplierOrderScheduleResponseModel responseModel = new SupplierOrderScheduleResponseModel();
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
                        var storeRepository = unitOfWork?.GetRepository<Store>();
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
                        var supplierRepository = unitOfWork?.GetRepository<Supplier>();
                        if (!await supplierRepository.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                }
                var repository = unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                if ((await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.SupplierId==viewModel.SupplierId &&!x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                {
                    throw new AlreadyExistsException(ErrorMessages.SupplierOrderingDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                var schedule = MappingHelpers.Mapping<SupplierOrderScheduleRequestModel, SupplierOrderingSchedule>(viewModel);


                schedule.CreatedById = userId;
                schedule.UpdatedById = userId;

                var result = await repository.InsertAsync(schedule).ConfigureAwait(false);
                await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (result != null)
                {
                    responseModel = await GetSupplierOrderScheduleById(result.Id).ConfigureAwait(false);
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
        public async Task<SupplierOrderScheduleResponseModel> Update(SupplierOrderScheduleRequestModel viewModel, int glId, int userId)
        {
            SupplierOrderScheduleResponseModel responseModel = new SupplierOrderScheduleResponseModel();
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
                        var storeRepository = unitOfWork?.GetRepository<Store>();
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
                        var supplierRepository = unitOfWork?.GetRepository<Supplier>();
                        if (!await supplierRepository.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                }
                var repository = unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                if ((await repository.GetAll(x => x.StoreId == viewModel.StoreId && x.SupplierId==viewModel.SupplierId && x.Id != glId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                {
                    throw new AlreadyExistsException(ErrorMessages.SupplierOrderingDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var request = MappingHelpers.Mapping<SupplierOrderScheduleRequestModel, SupplierOrderingSchedule>(viewModel);

                request.Id = glId;
                request.CreatedById = userId;
                request.UpdatedById = userId;
                repository.DetachLocal(_ => _.Id == request.Id);
                repository.Update(request);
                await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                responseModel = await GetSupplierOrderScheduleById(request.Id).ConfigureAwait(false);
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
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete GL Account
        /// </summary>
        /// <param name="glId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(long soId, int userId)
        {
            try
            {
                if (soId > 0)
                {
                    var _repository = unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                    var scheduleExist = await _repository.GetAll(x => x.Id == soId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (scheduleExist != null)
                    {
                        if (!scheduleExist.IsDeleted)
                        {
                            scheduleExist.UpdatedById = userId;
                            scheduleExist.IsDeleted = true;
                            _repository?.Update(scheduleExist);
                            await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.SupplierOrderingNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.SupplierOrderingId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.SupplierOrderingNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
