using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class POSMessageServices : IPOSMessageServices
    {
        private readonly IUnitOfWork unitOfWork = null;

        public POSMessageServices(IUnitOfWork iunitOfWork)
        {
            unitOfWork = iunitOfWork;
        }

        public async Task<PagedOutputModel<List<POSMessagesResponseModel>>> GetAllActivePOSMessages(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                int count = 0;
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                var _repository = unitOfWork?.GetRepository<POSMessages>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                new SqlParameter("@SkipCount", inputModel?.SkipCount),
                new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@IsLogged", inputModel?.IsLogged),
                new SqlParameter("@Module","POSMessage"),
                new SqlParameter("@RoleId",RoleId)
            };
                var dset = await _repository.ExecuteStoredProcedure(StoredProcedures.GetPosMessages, dbParams.ToArray()).ConfigureAwait(false);
                List<POSMessagesResponseModel> responseModels = MappingHelpers.ConvertDataTable<POSMessagesResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<POSMessagesResponseModel>>(responseModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<PagedOutputModel<List<POSMessagesResponseModel>>> GetActivePOSMessages(PagedInputModel inputModel)
        {
            try
            {
                var _repository = unitOfWork?.GetRepository<POSMessages>();
                var list = _repository.GetAll(x => !x.IsDeleted, null, includes:
                    new Expression<Func<POSMessages, object>>[] { c => c.Zone });

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.POSMessage.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.ReferenceId.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

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
                            case "message":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.POSMessage);
                                else
                                    list = list.OrderByDescending(x => x.POSMessage);
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

                List<POSMessagesResponseModel> responseModels;
                responseModels = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<POSMessagesResponseModel>>(responseModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<POSMessagesResponseModel> GetPOSMessageById(int id)
        {

            try
            {
                var _repository = unitOfWork?.GetRepository<POSMessages>();
                if (id > 0)
                {

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@ID", id),
            };
                    var dset = await _repository.ExecuteStoredProcedure(StoredProcedures.GetPosMessagesByID, dbParams.ToArray()).ConfigureAwait(false);
                    POSMessagesResponseModel reponseModel = MappingHelpers.ConvertDataTable<POSMessagesResponseModel>(dset.Tables[0]).FirstOrDefault();
                    if (reponseModel == null)
                    {
                        throw new NotFoundException(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    reponseModel.DisplayTypeId = (int)Enum.Parse(typeof(MessageDisplayType), reponseModel?.DisplayType);
                    reponseModel.ReferenceTypeId = (int)Enum.Parse(typeof(MessageRefType), reponseModel?.ReferenceType);
                    if (reponseModel.DisplayTypeId == (int)MessageDisplayType.CFD_DEFAULT)
                    {
                        reponseModel.DisplayType = EnumHelper.GetDescription<MessageDisplayType>(MessageDisplayType.CFD_DEFAULT);
                    }

                    if (!string.IsNullOrEmpty(reponseModel?.ReferenceOverrideType))
                        reponseModel.ReferenceOverrideTypeId = (int)Enum.Parse(typeof(MessageRefType), reponseModel?.ReferenceOverrideType);


                    if (!string.IsNullOrEmpty(reponseModel.ImagePath))
                    {
                        Byte[] imageBytes;
                        string imageFolderPath = Directory.GetCurrentDirectory() + reponseModel.ImagePath;
                        if (File.Exists(imageFolderPath))
                        {
                            imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                            reponseModel.Image = imageBytes;
                        }
                    }


                    return reponseModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.POSMsgId.ToString(CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<POSMessagesResponseModel> POSMessageById(int id)
        {

            try
            {
                var _repository = unitOfWork?.GetRepository<POSMessages>();
                if (id > 0)
                {
                    var list = await _repository.GetAll(x => !x.IsDeleted && x.Id == id).Include(c => c.Zone).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (list == null)
                    {
                        throw new NotFoundException(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture));
                    }


                    POSMessagesResponseModel reponseModel = MappingHelpers.CreateMap(list);

                    //if (!string.IsNullOrEmpty(list.ImagePath))
                    //{
                    //    Byte[] imageBytes;
                    //    string imageFolderPath = Directory.GetCurrentDirectory() + list.ImagePath;
                    //    if (File.Exists(imageFolderPath))
                    //    {
                    //        imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                    //        reponseModel.Image = imageBytes;
                    //    }
                    //}


                    return reponseModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.POSMsgId.ToString(CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<POSMessagesResponseModel> Insert(POSMessageRequestModel viewModel, int userId)
        {
            POSMessagesResponseModel responseModel = new POSMessagesResponseModel();

            long refId = 0;
            try
            {
                if (viewModel != null)
                {

                    if (viewModel.Image == null && (string.IsNullOrEmpty(viewModel.POSMessage) || string.IsNullOrWhiteSpace(viewModel.POSMessage)))
                    {
                        throw new BadRequestException(ErrorMessages.MessageRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.ReferenceTypeId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidReftype.ToString(CultureInfo.CurrentCulture));
                    }
                    var storeRepository = unitOfWork?.GetRepository<MasterListItems>();
                    if (viewModel.ZoneId > 0)
                    {
                        if (!(await storeRepository.GetAll(x => x.Id == viewModel.ZoneId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidZone.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.CFD_DEFAULT)
                    {

                        viewModel.ReferenceId = CommonMessages.CFDRefId;
                        viewModel.DisplayTypeId = (int)MessageDisplayType.CFD_DEFAULT;
                        viewModel.Desc = CommonMessages.CFDetailsDesc;
                        viewModel.DayParts = CommonMessages.CFDDayParts;
                    }
                    else if (viewModel.ReferenceTypeId == (int)MessageRefType.RECIEPT)
                    {

                        viewModel.ReferenceId = CommonMessages.RecieptRefId;
                        viewModel.DisplayTypeId = (int)MessageDisplayType.CUSTOMER;
                        viewModel.Desc = CommonMessages.RecieptDesc;
                    }

                    else if (viewModel.ReferenceTypeId == (int)MessageRefType.REMINDER)
                    {

                        viewModel.ReferenceId = CommonMessages.ReminderRefId;
                        viewModel.DisplayTypeId = (int)MessageDisplayType.CASHIER;
                        viewModel.Desc = CommonMessages.ReminderDesc;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(viewModel.ReferenceId))
                        {
                            throw new BadRequestException(ErrorMessages.ReferenceIdRequired.ToString(CultureInfo.CurrentCulture));
                        }
                        if (viewModel.DisplayTypeId <= 0)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidDisplayId.ToString(CultureInfo.CurrentCulture));
                        }

                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.PRODUCT)
                    {
                        try
                        {
                            refId = Convert.ToInt64(viewModel.ReferenceId);
                        }
                        catch (FormatException)
                        {
                            throw new FormatException(ErrorMessages.InvalidRefId.ToString(CultureInfo.CurrentCulture));
                        }

                        var prodRepository = unitOfWork.GetRepository<Product>();

                        if (!await prodRepository.GetAll(x => !x.IsDeleted && x.Number == refId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidProduct.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.COMPETETION)
                    {
                        var compRepository = unitOfWork.GetRepository<CompetitionDetail>();

                        if (!await compRepository.GetAll(x => !x.IsDeleted && x.Code == viewModel.ReferenceId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCompetetion.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.PROMOTION)
                    {
                        var promoRepository = unitOfWork.GetRepository<Promotion>();

                        if (!await promoRepository.GetAll(x => !x.IsDeleted && x.Code == viewModel.ReferenceId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidPrmotion.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.COMMODITY)
                    {
                        var comRepository = unitOfWork.GetRepository<Commodity>();

                        if (!await comRepository.GetAll(x => !x.IsDeleted && x.Code == viewModel.ReferenceId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCommodity.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    //if (viewModel.ReferenceOverrideTypeId == (int)MessageRefType.Commodity)
                    //{ }

                    var repository = unitOfWork?.GetRepository<POSMessages>();

                    var posMsg = MappingHelpers.CreateMap(viewModel);

                    posMsg.CreatedById = userId;
                    posMsg.UpdatedById = userId;

                    if (posMsg.Image != null)
                    {
                        posMsg.ImageType = ".png";
                    }
                    var result = await repository.InsertAsync(posMsg).ConfigureAwait(false);
                    await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        if (viewModel.ReferenceTypeId == (int)MessageRefType.RECIEPT || viewModel.ReferenceTypeId == (int)MessageRefType.REMINDER || viewModel.ReferenceTypeId == (int)MessageRefType.CFD_DEFAULT)
                        {
                            result.ReferenceId = result.ReferenceId + result.Id;
                            repository.DetachLocal(x => x.Id == result.Id);
                            repository?.Update(result);
                            await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        }
                        responseModel = await GetPOSMessageById(result.Id).ConfigureAwait(false);
                    }

                }
                return responseModel;
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    throw new BadRequestException(ex.Message);
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
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<POSMessagesResponseModel> Update(POSMessageRequestModel viewModel, int posId, int userId)
        {
            POSMessagesResponseModel responseModel = new POSMessagesResponseModel();
            try
            {
                long refId = 0;
                if (posId <= 0)
                {
                    throw new BadRequestException(ErrorMessages.POSMsgId.ToString(CultureInfo.CurrentCulture));
                }
                if (viewModel != null)
                {

                    if (viewModel.Image == null && (string.IsNullOrEmpty(viewModel.POSMessage) || string.IsNullOrWhiteSpace(viewModel.POSMessage)))
                    {
                        throw new BadRequestException(ErrorMessages.MessageRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    var storeRepository = unitOfWork?.GetRepository<MasterListItems>();
                    if (viewModel.ZoneId > 0)
                    {
                        if (!(await storeRepository.GetAll(x => x.Id == viewModel.ZoneId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidZone.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.CFD_DEFAULT)
                    {

                        viewModel.ReferenceId = CommonMessages.CFDRefId + posId.ToString();
                        viewModel.DisplayTypeId = (int)MessageDisplayType.CFD_DEFAULT;
                        viewModel.Desc = CommonMessages.CFDetailsDesc;
                        viewModel.DayParts = CommonMessages.CFDDayParts;
                    }

                    else if (viewModel.ReferenceTypeId == (int)MessageRefType.RECIEPT)
                    {

                        viewModel.ReferenceId = CommonMessages.RecieptRefId + posId.ToString(); ;
                        viewModel.DisplayTypeId = (int)MessageDisplayType.CUSTOMER;
                        viewModel.Desc = CommonMessages.RecieptDesc;
                    }

                    else if (viewModel.ReferenceTypeId == (int)MessageRefType.REMINDER)
                    {

                        viewModel.ReferenceId = CommonMessages.ReminderRefId + posId.ToString(); ;
                        viewModel.DisplayTypeId = (int)MessageDisplayType.CASHIER;
                        viewModel.Desc = CommonMessages.ReminderDesc;
                    }

                    else
                    {
                        if (string.IsNullOrEmpty(viewModel.ReferenceId))
                        {
                            throw new BadRequestException(ErrorMessages.ReferenceIdRequired.ToString(CultureInfo.CurrentCulture));
                        }
                        if (viewModel.DisplayTypeId <= 0)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidDisplayId.ToString(CultureInfo.CurrentCulture));
                        }


                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.PRODUCT)
                    {
                        try
                        {
                            refId = Convert.ToInt64(viewModel.ReferenceId);
                        }
                        catch (FormatException)
                        {
                            throw new FormatException(ErrorMessages.InvalidRefId.ToString(CultureInfo.CurrentCulture));
                        }

                        var prodRepository = unitOfWork.GetRepository<Product>();

                        if (!await prodRepository.GetAll(x => !x.IsDeleted && x.Number == refId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidProduct.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.COMPETETION)
                    {
                        var compRepository = unitOfWork.GetRepository<CompetitionDetail>();

                        if (!await compRepository.GetAll(x => !x.IsDeleted && x.Code == viewModel.ReferenceId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCompetetion.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.PROMOTION)
                    {
                        var promoRepository = unitOfWork.GetRepository<Promotion>();

                        if (!await promoRepository.GetAll(x => !x.IsDeleted && x.Code == viewModel.ReferenceId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidPrmotion.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    if (viewModel.ReferenceTypeId == (int)MessageRefType.COMMODITY)
                    {
                        var comRepository = unitOfWork.GetRepository<Commodity>();

                        if (!await comRepository.GetAll(x => !x.IsDeleted && x.Code == viewModel.ReferenceId).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCommodity.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    var repository = unitOfWork?.GetRepository<POSMessages>();

                    var posExists = await repository.GetAll(x => !x.IsDeleted && x.Id == posId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (posExists == null)
                    {
                        throw new NotFoundException(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    // var posMsg = MappingHelpers.Mapping<POSMessageRequestModel, POSMessages>(viewModel);


                    var posMsg = MappingHelpers.CreateMap(viewModel);

                    posMsg.Priority = posMsg.Priority <= 0 ? 1 : posMsg.Priority;
                    posMsg.CreatedById = posExists.CreatedById;
                    posMsg.CreatedAt = posExists.CreatedAt;
                    posMsg.UpdatedById = userId;
                    posMsg.Id = posId;


                    if (posMsg.Image != null)
                    {
                        posMsg.ImageType = ".png";
                    }

                    repository.DetachLocal(x => x.Id == posId);
                    repository?.Update(posMsg);
                    await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                }
                responseModel = await GetPOSMessageById(posId).ConfigureAwait(false);
                return responseModel;
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    throw new BadRequestException(ex.Message);
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
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<bool> Delete(long msgId, int userId)
        {
            try
            {
                if (msgId > 0)
                {
                    var _repository = unitOfWork?.GetRepository<POSMessages>();
                    var msgExist = await _repository.GetAll(x => x.Id == msgId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (msgExist != null)
                    {
                        if (!msgExist.IsDeleted)
                        {
                            msgExist.UpdatedById = userId;
                            msgExist.IsDeleted = true;
                            _repository?.Update(msgExist);
                            await (unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.POSMsgId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<int> POSNumber()
        {

            try
            {
                var number = 0;
                var _repository = unitOfWork?.GetRepository<POSMessages>();

                List<SqlParameter> dbParams = new List<SqlParameter>();
                var dset = await _repository.ExecuteStoredProcedure(StoredProcedures.GetPOSMessageId, dbParams.ToArray()).ConfigureAwait(false);
                number =Convert.ToInt32(dset.Tables[0].Rows[0]["MessageId"]);
                return number;
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
                throw new Exception(ErrorMessages.POSNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
