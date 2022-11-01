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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class UserLoggerServices : IUserLoggerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILoggerManager _iLoggerManager = null;

        public UserLoggerServices(IUnitOfWork unitOfWork, ILoggerManager iLoggerManager)
        {
            _unitOfWork = unitOfWork;
            _iLoggerManager = iLoggerManager;
        }

        public async Task Insert<T>(UserLogRequestModel<T> viewModel) where T : class
        {
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<UserLog>();

                    var type = typeof(T);

                    var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(new { newData = viewModel.NewData, oldData = viewModel.OldData });

                    var log = MappingHelpers.Mapping<UserLogRequestModel<T>, UserLog>(viewModel);

                    log.ActionAt = DateTime.UtcNow;
                    log.DataLog = jsonData;

                    var result = await repository.InsertAsync(log).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        public async Task<PagedOutputModel<List<UserLogResponseModel<T>>>> GetUserLog<T>(int? Id, string TableName, string Module, DateTime? FromDate, DateTime? ToDate) where T : class
        {
            try
            {
                var responseModel = new PagedOutputModel<List<UserLogResponseModel<T>>>();

                if (Id != null)
                {
                    var repository = _unitOfWork?.GetRepository<UserLog>();

                    var allLogs = repository.GetAll(x => x.Table == TableName && x.TableId == Id && x.Module == Module && x.ActionAt >= FromDate && x.ActionAt <= ToDate);

                    var reponse = (await allLogs.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateUserLog<T>).ToList();

                    responseModel = new PagedOutputModel<List<UserLogResponseModel<T>>>(reponse, reponse.Count);
                }
                return responseModel;
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
        public async Task<PagedOutputModel<List<UserLogResponseModel<T>>>> GetAuditUserLog<T>(int? Id, string TableName, string Module, DateTime? FromDate, DateTime? ToDate) where T : class
        {
            try
            {
                var responseModel = new PagedOutputModel<List<UserLogResponseModel<T>>>();

                if (Id != null)
                {
                    var repository = _unitOfWork?.GetRepository<UserLog>();

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@TableId", Id),
                        new SqlParameter("@TableName", TableName),
                        new SqlParameter("@Module", Module),
                        new SqlParameter("@FromDate", FromDate),
                        new SqlParameter("@ToDate", ToDate)
                    };
                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetUserLog, dbParams.ToArray()).ConfigureAwait(false);
                    var userAuditLogVM = MappingHelpers.ConvertDataTable<UserAuditLogResponseModel>(dset.Tables[0]).ToList();
                    List<UserLogResponseModel<T>> usrLogVM = new List<UserLogResponseModel<T>>();
                    foreach (var userAuditLogResponseModel in userAuditLogVM)
                    {
                        UserLogResponseModel<T> userLogObject = new UserLogResponseModel<T>();
                        userLogObject.Id = userAuditLogResponseModel.Id;
                        userLogObject.Table = userAuditLogResponseModel.Table;
                        userLogObject.TableId = userAuditLogResponseModel.TableId;
                        userLogObject.Action = userAuditLogResponseModel.Action;
                        userLogObject.ActionAt = userAuditLogResponseModel.ActionAt;
                        userLogObject.ActionBy = userAuditLogResponseModel.ActionBy;
                        userLogObject.UserName = userAuditLogResponseModel.UserName;
                        userLogObject.UserNumber = userAuditLogResponseModel.UserNumber;
                        userLogObject.Activity= userAuditLogResponseModel.Activity;
                        userLogObject.DataLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<DataLog<T>>(userAuditLogResponseModel?.DataLog);
                        usrLogVM.Add(userLogObject);
                    }

                    responseModel = new PagedOutputModel<List<UserLogResponseModel<T>>>(usrLogVM, userAuditLogVM.Count);
                }
                return responseModel;
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

        public async Task<PagedOutputModel<List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>>> GetPDELoadHistory()
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<UserLog>();

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetPDELoadHistory).ConfigureAwait(false);
                var userAuditLogVM = MappingHelpers.ConvertDataTable<UserAuditLogResponseModel>(dset.Tables[0]).ToList();
                List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>> pdeLoadVm = new List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>();
                foreach (var userAuditLogResponseModel in userAuditLogVM)
                {
                    PDELoadHistoryResponseModel<PDELoadDataLogResponseModel> pdeHistorObj = new PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>();
                    pdeHistorObj.Id = userAuditLogResponseModel.Id;
                    pdeHistorObj.Table = userAuditLogResponseModel.Table;
                    pdeHistorObj.TableId = userAuditLogResponseModel.TableId;
                    pdeHistorObj.Action = userAuditLogResponseModel.Action;
                    pdeHistorObj.ActionAt = userAuditLogResponseModel.ActionAt;
                    pdeHistorObj.ActionBy = userAuditLogResponseModel.ActionBy;
                    pdeHistorObj.UserName = userAuditLogResponseModel.UserName;
                    pdeHistorObj.UserNumber = userAuditLogResponseModel.UserNumber;
                    //  pdeHistorObj.PDEDataLogList = Newtonsoft.Json.JsonConvert.DeserializeObject<PDEDataLogs<PDELoadDataLogResponseModel>>(userAuditLogResponseModel?.DataLog);
                    pdeLoadVm.Add(pdeHistorObj);
                }

                var responseModel = new PagedOutputModel<List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>>(pdeLoadVm, userAuditLogVM.Count);

                return responseModel;
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

        public async Task<PagedOutputModel<List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>>> GetPDELoadHistoryGetById(int Id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<UserLog>();


                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Id", Id),
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetPDELoadHistory, dbParams.ToArray()).ConfigureAwait(false);
                var userAuditLogVM = MappingHelpers.ConvertDataTable<UserAuditLogResponseModel>(dset.Tables[0]).ToList();
                List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>> pdeLoadVm = new List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>();
                foreach (var userAuditLogResponseModel in userAuditLogVM)
                {
                    PDELoadHistoryResponseModel<PDELoadDataLogResponseModel> pdeHistorObj = new PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>();
                    pdeHistorObj.Id = userAuditLogResponseModel.Id;
                    pdeHistorObj.Table = userAuditLogResponseModel.Table;
                    pdeHistorObj.TableId = userAuditLogResponseModel.TableId;
                    pdeHistorObj.Action = userAuditLogResponseModel.Action;
                    pdeHistorObj.ActionAt = userAuditLogResponseModel.ActionAt;
                    pdeHistorObj.ActionBy = userAuditLogResponseModel.ActionBy;
                    pdeHistorObj.UserName = userAuditLogResponseModel.UserName;
                    pdeHistorObj.UserNumber = userAuditLogResponseModel.UserNumber;
                    pdeHistorObj.PDEDataLogList = Newtonsoft.Json.JsonConvert.DeserializeObject<PDEDataLogs<PDELoadDataLogResponseModel>>(userAuditLogResponseModel?.DataLog);
                    pdeLoadVm.Add(pdeHistorObj);
                }

                var responseModel = new PagedOutputModel<List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>>(pdeLoadVm, userAuditLogVM.Count);

                return responseModel;
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

        public async Task<PagedOutputModel<List<UserActivityResponseModel>>> GetUserActivity(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<UserLog>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
#pragma warning restore CA1062 // Validate arguments of public methods
                        new SqlParameter("@SkipCount", inputModel.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel.Sorting),
                        new SqlParameter("@SortDirection", inputModel.Direction)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetUserActivity, dbParams.ToArray()).ConfigureAwait(false);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                var userAuditLogVM = MappingHelpers.ConvertDataTable<UserActivityResponseModel>(dset.Tables[0]).ToList();

                var responseModel = new PagedOutputModel<List<UserActivityResponseModel>>(userAuditLogVM, count);

                return responseModel;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));
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
