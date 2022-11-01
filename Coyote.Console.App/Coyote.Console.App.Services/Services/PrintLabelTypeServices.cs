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
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class PrintLabelTypeServices: IPrintLabelTypeServices
    {
        private readonly IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLoggerManager = null;



        public PrintLabelTypeServices(IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager, IUnitOfWork commodityRepository)
        {
            _unitOfWork = commodityRepository;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        /// <summary>
        /// Get all active Print Label Types.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<PrintLabelTypeResponseModel>>> GetAllActivePrintLabelTypes(PagedInputModel inputModel = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<PrintLabelType>();
                var labelTypes = repository.GetAll(x => !x.IsDeleted);

                
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        labelTypes = labelTypes.Where(x => x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    labelTypes=labelTypes.OrderByDescending(x => x.UpdatedAt);
                    count = labelTypes.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        labelTypes = labelTypes.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    { 
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    labelTypes = labelTypes.OrderBy(x => x.Code);
                                else
                                    labelTypes = labelTypes.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    labelTypes = labelTypes.OrderBy(x => x.Desc);
                                else
                                    labelTypes = labelTypes.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    labelTypes = labelTypes.OrderBy(x => x.Id);
                                else
                                    labelTypes = labelTypes.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                List<PrintLabelTypeResponseModel> labelViewModel;
                labelViewModel = (await labelTypes.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.Mapping<PrintLabelType, PrintLabelTypeResponseModel>).ToList();


                return new PagedOutputModel<List<PrintLabelTypeResponseModel>>(labelViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get print label type using Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PrintLabelTypeResponseModel> GetPrintLabelTypeById(long id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<PrintLabelType>();

                    var labelType = await repository.GetAll(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (labelType == null)
                    {
                        throw new NotFoundException(ErrorMessages.LabelNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    PrintLabelTypeResponseModel printLabelModel;
                    printLabelModel = MappingHelpers.Mapping<PrintLabelType,PrintLabelTypeResponseModel>(labelType);
                    return printLabelModel;
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
        /// Add new Print label type.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PrintLabelTypeResponseModel> Insert(PrintLabelTypeRequestModel viewModel, int userId)
        {
            PrintLabelTypeResponseModel responseModel = new PrintLabelTypeResponseModel();
            try
            {
                if (viewModel != null)
                {
                    var _repository = _unitOfWork.GetRepository<PrintLabelType>();
                    if (!string.IsNullOrEmpty(viewModel.PrintBarCodeType))
                    {

                        if (await _repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.LabelDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        var printLabelType = _iAutoMapper.Mapping<PrintLabelTypeRequestModel, PrintLabelType>(viewModel);
                        printLabelType.IsDeleted = false;
                        printLabelType.CreatedById = userId;
                        printLabelType.UpdatedById = userId;
                        var result = await _repository.InsertAsync(printLabelType).ConfigureAwait(false);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        if (result != null)
                        {
                            responseModel = await GetPrintLabelTypeById(result.Id).ConfigureAwait(false);
                        }
                        return responseModel;
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.LabelCodeTypeInvalid.ToString(CultureInfo.CurrentCulture));
                    }
                }
            }
            catch (AlreadyExistsException aee)
            {
                throw new AlreadyExistsException(aee.Message, aee);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.LabelDuplicate.ToString(CultureInfo.CurrentCulture), ex);
            }
            return responseModel;
        }

        /// <summary>
        /// Update a print label type.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="Id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PrintLabelTypeResponseModel> Update(PrintLabelTypeRequestModel viewModel, int Id, int userId)
        {
            PrintLabelTypeResponseModel responseModel = new PrintLabelTypeResponseModel();
            try
            {
                if (viewModel != null)
                {
                    var _repository = _unitOfWork.GetRepository<PrintLabelType>();
                    if (Id <= 0) {
                        throw new NullReferenceCustomException(ErrorMessages.LabelIdReq.ToString(CultureInfo.CurrentCulture));
                    }
                    if (!string.IsNullOrEmpty(viewModel.PrintBarCodeType))
                    {

                        if (await _repository.GetAll(x => x.Code == viewModel.Code && x.Id!=Id && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.LabelDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        var labelExists = await _repository.GetAll(x => x.Id == Id && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (labelExists != null)
                        {
                            var printLabelType = _iAutoMapper.Mapping<PrintLabelTypeRequestModel, PrintLabelType>(viewModel);
                            printLabelType.Id = Id;
                            printLabelType.IsDeleted = false;
                            printLabelType.CreatedById = labelExists.CreatedById;
                            printLabelType.CreatedAt = labelExists.CreatedAt;
                            printLabelType.UpdatedById = userId;
                            
                            //Detaching tracked entry - exists
                            _repository.DetachLocal(_ => _.Id == printLabelType.Id);
                            _repository.Update(printLabelType);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                           
                                responseModel = await GetPrintLabelTypeById(printLabelType.Id).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new NotFoundException(ErrorMessages.LabelNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        return responseModel;
                    }
                    else
                    {
                        throw new NullReferenceCustomException(ErrorMessages.LabelCodeTypeInvalid.ToString(CultureInfo.CurrentCulture));
                    }
                }
            }
            catch (AlreadyExistsException aee)
            {
                throw new AlreadyExistsException(aee.Message, aee);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (NotFoundException nf)
            {
                throw new NotFoundException(nf.Message, nf);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.LabelNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
            return responseModel;

        }

        /// <summary>
        /// Delete a print label type.
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int labelId, int userId)
        {
            try
            {
                if (labelId > 0)
                {
                    var _repository = _unitOfWork?.GetRepository<PrintLabelType>();
                    var labelExists = await _repository.GetAll(x => x.Id == labelId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (labelExists != null)
                    {
                       // labelExists.Code = labelExists.Code + "~" + labelExists.Id;
                        labelExists.UpdatedById = userId;
                        labelExists.UpdatedAt = DateTime.UtcNow;
                        labelExists.IsDeleted = true;
                        _repository?.Update(labelExists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                        throw new NotFoundException(ErrorMessages.LabelNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new BadRequestException(ErrorMessages.LabelIdReq.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (NotFoundException ne)
            {
                throw new NotFoundException(ne.Message, ne);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.LabelNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
