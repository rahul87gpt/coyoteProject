using System;
using System.Collections.Generic;
using System.Data;
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
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class ReportSchedulerServices : IReportSchedulerServices
    {
        private IUnitOfWork _unitOfWork = null;
        private ILoggerManager _iLoggerManager = null;
        private ISendMailService _mailService = null;
        /// <summary>
        /// MasterListService Constructor, initilize IMasterListRepository,IAutoMappingServices
        /// </summary>
        /// <param name="IMasterListRepository"></param>
        /// <param name="iAutoMapperService"></param>
        public ReportSchedulerServices(IUnitOfWork repo, ILoggerManager iLoggerManager, ISendMailService imailService)
        {
            _unitOfWork = repo;
            _iLoggerManager = iLoggerManager;
            _mailService = imailService;
        }

        public async Task<ReportSchedulerResponseModel> InsertReportSchedulers(ReportSchedulerResponseModel requestModel, int userId)
        {
            try
            {
                var response = new ReportSchedulerResponseModel();

                if (requestModel == null)
                    throw new NullReferenceException();

                if (requestModel.ExcelExport == false && requestModel.PdfExport == false && requestModel.CsvExport == false)
                    throw new BadRequestException(ErrorMessages.ExportReq.ToString(CultureInfo.CurrentCulture));

                var repository = _unitOfWork?.GetRepository<ReportScheduler>();

                var scheduler = MappingHelpers.Mapping<ReportSchedulerResponseModel, ReportScheduler>(requestModel);

                if (string.IsNullOrEmpty(requestModel.ReportName))
                {
                    throw new BadRequestException(ErrorMessages.ReportNameReq.ToString(CultureInfo.CurrentCulture));
                }

                if (requestModel.UserIds != null)
                {
                    scheduler.SchedulerUser = new List<SchedulerUser>();
                    foreach (var user in requestModel.UserIds)
                    {
                        var newUser = new SchedulerUser();
                        newUser.UserId = user;
                        newUser.SchedulerId = scheduler.ID;
                        newUser.CreatedBy = userId;
                        newUser.ModifiedBy = userId;
                        newUser.IsActive = Status.Active;
                        newUser.CreatedDate = DateTime.UtcNow;
                        newUser.ModifiedDate = DateTime.UtcNow;
                        scheduler.SchedulerUser.Add(newUser);

                    }
                }

                var masterRepo = _unitOfWork?.GetRepository<MasterListItems>();
                var reportid = await masterRepo.GetAll().Where(x => x.Name.ToLower() == requestModel.ReportName.ToLower() && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                if (reportid <= 0)
                {
                    throw new BadRequestException(ErrorMessages.ReportNameNotFound.ToString(CultureInfo.CurrentCulture));
                }

                scheduler.ReportId = reportid;
                scheduler.CreatedDate = DateTime.UtcNow;
                scheduler.CreatedBy = userId;
                scheduler.ModifiedDate = DateTime.UtcNow;
                scheduler.ModifiedBy = userId;
                scheduler.LastRun = null;
                var result = await repository.InsertAsync(scheduler).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                if (result.ID > 0)
                {
                    response = await GetActiveSchedulerById(result.ID).ConfigureAwait(false);
                }
                return response;
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
        public async Task<ReportSchedulerResponseModel> UpdateReportSchedulers(ReportSchedulerResponseModel requestModel, long Id, int userId)
        {
            try
            {
                if (requestModel == null)
                    throw new NullReferenceException();

                if (string.IsNullOrEmpty(requestModel.UserIds.ToString()))
                    throw new BadRequestException(ErrorMessages.UserIdReq.ToString(CultureInfo.CurrentCulture));

                var repository = _unitOfWork?.GetRepository<ReportScheduler>();

                if (requestModel.ExcelExport == false && requestModel.PdfExport == false && requestModel.CsvExport == false)
                    throw new BadRequestException(ErrorMessages.ExportReq.ToString(CultureInfo.CurrentCulture));

                var exists = await repository.GetAll().Include(c => c.SchedulerUser).Where(x => x.ID == Id && x.IsActive != Status.Deleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                if (exists == null)
                {
                    throw new NotFoundException(ErrorMessages.SchedulerNotExists.ToString(CultureInfo.CurrentCulture));
                }

                if (exists.SchedulerUser == null)
                {
                    exists.SchedulerUser = new List<SchedulerUser>();
                }


                //Save users
                if (requestModel.UserIds != null)
                {
                    foreach (var user in requestModel.UserIds)
                    {
                        var userExist = exists.SchedulerUser.Where(x => x.UserId == user).FirstOrDefault();
                        if (userExist != null && userExist.IsActive != Status.Active)
                        {
                            userExist.ModifiedDate = DateTime.UtcNow;
                            userExist.ModifiedBy = userId;
                        }
                        else if (userExist == null)
                        {
                            var newUser = new SchedulerUser();
                            newUser.UserId = user;
                            newUser.SchedulerId = exists.ID;
                            newUser.CreatedBy = userId;
                            newUser.ModifiedBy = userId;
                            newUser.CreatedDate = DateTime.UtcNow;
                            newUser.ModifiedDate = DateTime.UtcNow;
                            exists.SchedulerUser.Add(newUser);
                        }
                    }
                }

                // var scheduler = MappingHelpers.Mapping<ReportSchedulerResponseModel, ReportScheduler>(requestModel);

                exists.ID = Id;
                exists.IntervalBracket = requestModel.IntervalBracket;
                exists.IsActive = requestModel.IsActive;
                exists.IntervalInd = requestModel.IntervalInd;
                exists.ModifiedDate = DateTime.UtcNow;
                exists.ModifiedBy = userId;

                repository.DetachLocal(_ => _.ID == Id);
                repository.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                var response = await GetActiveSchedulerById(Id).ConfigureAwait(false);

                return response;
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
        public async Task<ReportSchedulerResponseModel> GetSchedulerById(long schedulerId)
        {
            try
            {
                if (schedulerId <= 0)
                {
                    throw new BadRequestException(ErrorMessages.CashierIdRequired.ToString(CultureInfo.CurrentCulture));
                }

                var repository = _unitOfWork?.GetRepository<ReportScheduler>();
                var response = new ReportSchedulerResponseModel();
                var scheduler = await repository.GetAll().Include(x => x.Report).Include(c => c.SchedulerUser).Where(x => x.ID == schedulerId && x.IsActive != Status.Deleted && !x.Report.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                if (scheduler == null)
                {

                    throw new NotFoundException(ErrorMessages.SchedulerNotFound.ToString(CultureInfo.CurrentCulture));
                }

                response = MappingHelpers.Mapping<ReportScheduler, ReportSchedulerResponseModel>(scheduler);

                if (scheduler.SchedulerUser != null)
                {
                    response.UserIds = new List<int>();
                    response.UserIds.AddRange(scheduler.SchedulerUser.Select(x => x.UserId).ToList());
                }

                return response;
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

        public async Task<ReportSchedulerResponseModel> GetActiveSchedulerById(long schedulerId, bool useFilter = false)
        {
            try
            {
                if (schedulerId <= 0)
                {
                    throw new BadRequestException(ErrorMessages.CashierIdRequired.ToString(CultureInfo.CurrentCulture));
                }

                var repository = _unitOfWork?.GetRepository<ReportScheduler>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@Id", schedulerId)
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetReportSchedulerById, dbParams.ToArray()).ConfigureAwait(false);
                var response = new ReportSchedulerResponseModel();
                response = MappingHelpers.ConvertDataTable<ReportSchedulerResponseModel>(dset.Tables[0]).FirstOrDefault();

                if (response == null)
                {
                    throw new NotFoundException(ErrorMessages.SchedulerNotFound.ToString(CultureInfo.CurrentCulture));
                }

                if (useFilter)
                {
                    response.FilterBody = GetEmailTemplate(dset);
                }
                return response;
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

        public async Task<PagedOutputModel<List<ReportSchedulerResponseModel>>> GetAllScheduler(PagedInputModel inputModel)
        {

            try
            {
                var repository = _unitOfWork?.GetRepository<ReportScheduler>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                     new SqlParameter("@SortColumn", inputModel?.Sorting),
                     new SqlParameter("@SortDirection", inputModel?.Direction),
                     new SqlParameter("@PageNumber", inputModel?.SkipCount),
                     new SqlParameter("@PageSize", inputModel?.MaxResultCount)
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllReportScheduler, dbParams.ToArray()).ConfigureAwait(false);
                List<ReportSchedulerResponseModel> responseModel =
                    MappingHelpers.ConvertDataTable<ReportSchedulerResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<ReportSchedulerResponseModel>>(responseModel, count);
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
        public async Task UpdateLastRun(long Id)
        {
            try
            {
                if (Id <= 0)
                { throw new NotFoundException(ErrorMessages.SchedulerNotExists.ToString(CultureInfo.CurrentCulture)); }
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<ReportScheduler>();
                    var scheduler = await repository.GetAll().Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (scheduler != null)
                    {
                        scheduler.LastRun = DateTime.UtcNow;

                        repository.DetachLocal(_ => _.ID == Id);
                        repository.Update(scheduler);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    }
                }

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

        public async Task SaveReportFailLog(long schedulerId, string reportName, string error = null)
        {
            //var schdRepository = _unitOfWork?.GetRepository<ReportSchedulerLog>();
            //var scheduler = await schdRepository.GetAll().Where(x => x.ID == schedulerId && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);


            var repository = _unitOfWork?.GetRepository<ReportSchedulerLog>();
            var existingLog = await repository.GetAll().Where(x => x.SchedulerId == schedulerId && (x.IsActive == Status.Active || x.IsReported == false)).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

            if (existingLog != null)
            {
                if (existingLog.ReportTryCount < 3)
                {
                    existingLog.IsReportGenerated = false;
                    existingLog.ReportTryCount = existingLog.ReportTryCount == null ? 1 : existingLog.ReportTryCount + 1;
                    existingLog.ModifiedDate = DateTime.UtcNow;
                    existingLog.ErrorMessage = error;
                }


                if (existingLog.ReportTryCount == 3)
                {
                    existingLog.IsReportGenerated = false;
                    existingLog.ModifiedDate = DateTime.UtcNow;
                    existingLog.IsReported = true;
                    //Send Email with error message

                    //Move these details to seperate table later.
                    SendEmailViewModel mailModel = new SendEmailViewModel();
                    mailModel.ToAddress = "archawanjare@cdnsol.com";
                    mailModel.EmailSubject = "Coyote Console Report Error";
                    mailModel.MsgBody = $"Dear Admin, <br/><br/>" +
                                          $"There is some issues in generating report : {reportName} " +
                                          $"in Coyote Console App.<br/>" +
                                          $"Please Check and resolve.<br/>" +
                                          $"Error Message : {error}<br/>";
                    await _mailService.SendReportFailMail(mailModel).ConfigureAwait(false);
                    //Confirm with Manoj Sir, whether to set inactive after mail or not.
                    existingLog.IsActive = Status.Inactive;
                }

                repository.DetachLocal(_ => _.ID == existingLog.ID);
                repository.Update(existingLog);

            }
            else
            {
                var newLog = new ReportSchedulerLog();

                newLog.CreatedDate = DateTime.UtcNow;
                newLog.ModifiedDate = DateTime.UtcNow;
                newLog.IsActive = Status.Active;
                newLog.IsReportGenerated = false;
                newLog.ReportTryCount = 1;
                newLog.SchedulerId = schedulerId;
                newLog.ErrorMessage = error;
                newLog.IsReported = false;
                await repository.InsertAsync(newLog).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
            }

            //var result = await repository.InsertAsync(scheduler).ConfigureAwait(false);
            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

        }

        public async Task SaveReportEmailFailLog(long schedulerId, string reportName, byte[] report, string error = null)
        {
            var schdRepository = _unitOfWork?.GetRepository<ReportSchedulerLog>();
            var scheduler = await schdRepository.GetAll().Where(x => x.ID == schedulerId && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);


            var repository = _unitOfWork?.GetRepository<ReportSchedulerLog>();
            var existingLog = await repository.GetAll().Where(x => x.SchedulerId == schedulerId && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

            if (existingLog != null && existingLog.IsEmailSent == false)
            {
                if (existingLog.EmailTryCount < 3)
                {
                    existingLog.IsEmailSent = false;
                    existingLog.EmailTryCount = existingLog.ReportTryCount == null ? 1 : existingLog.ReportTryCount + 1;
                    existingLog.ModifiedDate = DateTime.UtcNow;
                    existingLog.ErrorMessage = error;
                }


                if (existingLog.EmailTryCount == 3)
                {
                    existingLog.IsEmailSent = false;
                    existingLog.ModifiedDate = DateTime.UtcNow;
                    existingLog.IsReported = true;
                    //Send Email with error message

                    //Move these details to seperate table later.
                    SendEmailViewModel mailModel = new SendEmailViewModel();
                    mailModel.ToAddress = "archawanjare@cdnsol.com";
                    mailModel.EmailSubject = "Coyote Console Report Error";
                    mailModel.MsgBody = $"Dear Admin,<br/>" +
                                          $"There is some issues in sending email for Report : {reportName} <br/>" +
                                          $"in Coyote Console App.<br/>" +
                                          $"Please Check and resolve.<br/>" +
                                          $"Error Message : {error}";
                    await _mailService.SendReportFailMail(mailModel).ConfigureAwait(false);
                    //Confirm with Manoj Sir, whether to set inactive after mail or not.
                    // existingLog.IsActive = Status.Inactive;
                }

                repository.DetachLocal(_ => _.ID == existingLog.ID);
                repository.Update(existingLog);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);


            }
            else
            {
                var newLog = new ReportSchedulerLog();

                newLog.CreatedDate = DateTime.UtcNow;
                newLog.ModifiedDate = DateTime.UtcNow;
                newLog.IsActive = Status.Active;
                newLog.IsEmailSent = false;
                newLog.EmailTryCount = 1;
                newLog.SchedulerId = schedulerId;
                newLog.ErrorMessage = error;
                newLog.IsReported = false;
                //                newLog.ReportGenerated = "";// convert byte[] to pdf and save it.
                var result = await repository.InsertAsync(newLog).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
            }



        }

        public static string GetEmailTemplate(DataSet dataSet)
        {
            var body = "";
            try
            {
                if (dataSet != null)
                {
                    //to avoid error on others
                    if (dataSet.Tables.Count > 2)
                    {
                        ReportFilterDataViewModel filter = MappingHelpers.ConvertDataTable<ReportFilterDataViewModel>(dataSet.Tables[2]).FirstOrDefault();
                        body = $"";
                        body = body + filter.startDate != null ? $"</br> <strong>From : </strong>{filter.startDate}  <strong> To : </strong> {filter.endDate} </br>" : "";
                        body = filter.productStartId != 0 ? body + $"</br> </br> <strong>Product Start Id :</strong> {filter.productStartId}" : body + "";
                        body = filter.productEndId != 0 ? body + $"</br> </br> <strong> Product End Id : </strong>{filter.productEndId}" : body + "";
                        body = !string.IsNullOrEmpty(filter.till) ? body + $"</br> </br><strong> Till :</strong> {filter.till}" : body + "";
                        body = filter.contineous != false ? body + $"</br> </br><strong> Continuous : </strong> YES" : body + "";
                        body = filter.drillDown != false ? body + $"</br><strong> Drilldown : </strong> YES" : body + "";
                        body = filter.summary != false ? body + $"</br><strong> Summary: </strong>  YES" : body + "";
                        body = filter.promoSales != false ? body + $"</br> <strong>Promo Sales :</strong> YES" : body + "";
                        body = filter.promoCode != null ? body + $"</br> <strong>Promo Code: </strong>  {filter.promoCode }" : body + "";
                        body = filter.merge != false ? body + $"</br> <strong>Merge :</strong>  YES" : body + "";
                        body = filter.variance != false ? body + $"</br><strong> Variance: </strong> YES" : body + "";
                        body = filter.wastage != false ? body + $"</br><strong> Wastage : </strong>  YES" : body + "";
                        body = filter.salesAMT != 0 ? body + $"</br> <strong>Sales AMT : </strong> {filter.salesAMT  }" : body + "";
                        body = filter.salesSOH != 0 ? body + $"</br><strong> Sales SOH :</strong> {filter.salesSOH  }" : body + "";
                        body = filter.salesAMTRange != 0 ? body + $"</br> <strong>Sales Amount Range :</strong> {filter.salesAMTRange }" : body + "";
                        body = filter.salesSOHRange != 0 ? body + $"</br> <strong>Sales SOH Range : </strong>{filter.salesSOHRange }" : body + "";
                        body = filter.NillTransactionInterval != null ? body + $"</br> <strong>Nil Transaction Interval :</strong>  {filter.NillTransactionInterval  }" : body + "";
                        body = filter.Shrinkage != null ? body + $"</br> <strong>Shrinkage :</strong> {filter.Shrinkage }" : body + "";
                    }

                    if (dataSet.Tables.Count > 3 && dataSet.Tables[3].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[3].Rows[0]["Store"]) ? body + "" : body + "</br><strong>Stores Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[3].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[3].Rows[i]["Store"]) ? body + "" : body + $"{dataSet.Tables[3].Rows[i]["Store"]}</br>";
                        }
                    }
                    if (dataSet.Tables.Count > 4 && dataSet.Tables[4].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[4].Rows[0]["Dept"]) ? body + "" : body + "</br><strong>Departments Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[4].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[4].Rows[i]["Dept"]) ? body + "" : body + $"</br> \n {dataSet.Tables[4].Rows[i]["Dept"]}";
                        }
                    }


                    if (dataSet.Tables.Count > 5 && dataSet.Tables[5].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[5].Rows[0]["Commodity"]) ? body + "" : body + "</br><strong>Commodity Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[5].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[5].Rows[i]["Commodity"]) ? body + "" : body + $"</br> \n {dataSet.Tables[5].Rows[i]["Commodity"]}";
                        }
                    }


                    if (dataSet.Tables.Count > 6 && dataSet.Tables[6].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[6].Rows[0]["Category"]) ? body + "" : body + "</br><strong>Category Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[6].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[6].Rows[i]["Category"]) ? body + "" : body + $"</br> \n {dataSet.Tables[6].Rows[i]["Category"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 7 && dataSet.Tables[7].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[7].Rows[0]["Groups"]) ? body + "" : body + "</br><strong>Groups Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[7].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[7].Rows[i]["Groups"]) ? body + "" :body + $"</br> \n {dataSet.Tables[7].Rows[i]["Groups"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 8 && dataSet.Tables[8].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[8].Rows[0]["Supplier"]) ? body + "" : body + "<strong>Supplier Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[8].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[8].Rows[i]["Supplier"]) ? body +"":body + $"</br> \n {dataSet.Tables[8].Rows[i]["Supplier"]}";
                        }
                    }


                    if (dataSet.Tables.Count > 9 && dataSet.Tables[9].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[9].Rows[0]["Manufacturer"]) ? body + "" : body + "<strong>Manufacturer Selected :</strong> ";
                        for (int i = 0; i < dataSet.Tables[9].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[9].Rows[i]["Manufacturer"]) ? body + "":body + $"</br> \n {dataSet.Tables[9].Rows[i]["Manufacturer"]}";
                        }
                    }


                    if (dataSet.Tables.Count > 10 && dataSet.Tables[10].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[10].Rows[0]["Cashier"]) ? body + "" : body + "</br><strong>Cashier Selected : </strong> ";
                        for (int i = 0; i < dataSet.Tables[10].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[10].Rows[i]["Cashier"]) ? body + "" : body + $"</br> \n {dataSet.Tables[10].Rows[i]["Cashier"]}";
                        }
                    }



                    if (dataSet.Tables.Count > 11 && dataSet.Tables[11].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[11].Rows[0]["Zones"]) ? body + "" : body + "</br><strong>Zones Selected :</strong> ";
                        for (int i = 0; i < dataSet.Tables[11].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[11].Rows[i]["Zones"]) ? body + "" : body + $"</br> \n {dataSet.Tables[11].Rows[i]["Zones"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 12 && dataSet.Tables[12].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[12].Rows[0]["Item"]) ? body + "" : body + "</br><strong>Days Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[12].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[12].Rows[i]["Item"]) ? body + "" : body + $"</br> \n {dataSet.Tables[12].Rows[i]["Item"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 13 && dataSet.Tables[13].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[13].Rows[0]["Promotion"]) ? body + "" : body + "</br><strong>Promotions Selected : </strong>";
                        for (int i = 0; i < dataSet.Tables[13].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[13].Rows[i]["Promotion"]) ? body + "" : body + $"</br> \n {dataSet.Tables[13].Rows[i]["Promotion"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 14 && dataSet.Tables[14].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[14].Rows[0]["NationalRange"]) ? body + "" : body + "</br><strong>National Range Selected :</strong> ";
                        for (int i = 0; i < dataSet.Tables[14].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[14].Rows[i]["NationalRange"]) ? body + "" : body + $"</br> \n {dataSet.Tables[14].Rows[i]["NationalRange"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 15 && dataSet.Tables[15].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[15].Rows[0]["Member"]) ? body + "" : body + "</br><strong>Members Selected :</strong> ";
                        for (int i = 0; i < dataSet.Tables[15].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[15].Rows[i]["Member"]) ? body + "" : body + $"</br> \n {dataSet.Tables[15].Rows[i]["Member"]}";
                        }
                    }

                    if (dataSet.Tables.Count > 16 && dataSet.Tables[16].Rows.Count > 0)
                    {
                        body = string.IsNullOrEmpty((string)dataSet.Tables[16].Rows[0]["Till"]) ? body + "" : body + "</br><strong>Tills Selected :</strong> ";
                        for (int i = 0; i < dataSet.Tables[16].Rows.Count; i++)
                        {
                            body = string.IsNullOrEmpty((string)dataSet.Tables[16].Rows[i]["Till"]) ? body + "" : body + $"</br> \n {dataSet.Tables[16].Rows[i]["Till"]}";
                        }
                    }


                    return body;
                }
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
            }
            return body;
        }

        public async Task<bool> DeleteScheduler(int Id, int userId)
        {
            try
            {
                if (Id <= 0)
                { throw new NotFoundException(ErrorMessages.SchedulerNotExists.ToString(CultureInfo.CurrentCulture)); }
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<ReportScheduler>();
                    var scheduler = await repository.GetAll().Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (scheduler != null)
                    {
                        scheduler.IsActive = Status.Deleted;
                        scheduler.ModifiedBy = userId;
                        scheduler.ModifiedDate = DateTime.UtcNow;

                        repository.DetachLocal(_ => _.ID == Id);
                        repository.Update(scheduler);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    }

                    return true;
                }

                return false;

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
    }

}


