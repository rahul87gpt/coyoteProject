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
    public class TaxServices : ITaxServices
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;


        /// <summary>
        /// TaxServices Constructor, initilize CoyoteAppDBContext,IAutoMappingServices
        /// </summary>
        /// <param name="ITaxRepository"></param>
        /// <param name="iAutoMapper"></param>
        public TaxServices(IUnitOfWork context, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager)
        {
            _unitOfWork = context;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        /// <summary>
        /// Get all Taxes List
        /// </summary>
        /// <returns>List<TaxViewModel></returns>
        public async Task<PagedOutputModel<List<TaxResponseModel>>> GetAllActiveTaxes(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Tax>();
                var taxes = repository.GetAll(x => !x.IsDeleted);


                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))

                        taxes = taxes.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        taxes = taxes.Where(x => x.Status);

                    taxes = taxes.OrderByDescending(x => x.UpdatedAt);
                    count = taxes.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        taxes = taxes.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    taxes = taxes.OrderBy(x => x.Code);
                                else
                                    taxes = taxes.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    taxes = taxes.OrderBy(x => x.Desc);
                                else
                                    taxes = taxes.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    taxes = taxes.OrderBy(x => x.Id);
                                else
                                    taxes = taxes.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                List<TaxResponseModel> taxViewModels;
                taxViewModels = (await taxes.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<Tax, TaxResponseModel>).ToList();
                return new PagedOutputModel<List<TaxResponseModel>>(taxViewModels, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.NoTaxFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get a tex with specific id
        /// </summary>
        /// <param name="taxId">Tax Id</param>
        /// <returns>TaxViewModel</returns>
        public async Task<TaxResponseModel> GetTaxById(long taxId)
        {
            try
            {
                if (taxId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Tax>();
                    var tax = await repository.GetAll(x => x.Id == taxId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (tax == null)
                    {
                        throw new NotFoundException(ErrorMessages.TaxIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    return _iAutoMapper.Mapping<Tax, TaxResponseModel>(tax);
                }
                throw new NullReferenceException(ErrorMessages.TaxIdNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Delete a specific tax.
        /// </summary>
        /// <param name="taxId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTax(long taxId, int userId)
        {
            try
            {
                if (taxId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Tax>();
                    var exists = await repository.GetById(taxId).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;
                        // exists.Code = (exists.Code + "~" + exists.Id);
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.TaxDeleted.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.TaxNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<TaxResponseModel> Update(TaxRequestModel viewModel, long taxId, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    TaxResponseModel responseModel = new TaxResponseModel();
                    if (taxId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.TaxIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<Tax>();
                    if (await repository.GetAll(x => x.Code == viewModel.Code && x.Id != taxId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.TaxCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetById(taxId).ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.TaxNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        var comm = _iAutoMapper.Mapping<TaxRequestModel, Tax>(viewModel);
                        comm.Id = taxId;
                        comm.IsDeleted = false;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;
                        comm.UpdatedById = userId;

                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        responseModel = await GetTaxById(comm.Id).ConfigureAwait(false);
                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.TaxNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<TaxResponseModel> Insert(TaxRequestModel viewModel, int userId)
        {

            TaxResponseModel responseModel = new TaxResponseModel();
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Tax>();
                    if (await repository.GetAll(x => x.Code == viewModel.Code && x.IsDeleted == false).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.TaxCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var comm = _iAutoMapper.Mapping<TaxRequestModel, Tax>(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        responseModel = await GetTaxById(result.Id).ConfigureAwait(false);
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
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }
    }
}



