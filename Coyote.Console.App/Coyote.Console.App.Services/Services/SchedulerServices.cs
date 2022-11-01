using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.ViewModels;
using Coyote.Console.Common.Exceptions;

namespace Coyote.Console.App.Services.Services
{
    public class SchedulerServices : ISchedulerService
    {
        private IReportService _reportService;
        private IReportSchedulerServices _schedulertService;
        IUnitOfWork _unitOfWork = null;
        private ILoggerManager _iLoggerManager = null;
        private ISendMailService _mailService = null;

        public SchedulerServices(IUnitOfWork repo, IReportService reportService, IReportSchedulerServices schedulertService, ILoggerManager iLoggerManager, ISendMailService imailService)
        {
            _unitOfWork = repo;
            _reportService = reportService;
            _schedulertService = schedulertService;
            _iLoggerManager = iLoggerManager;
            _mailService = imailService;
        }


        /// <summary>
        /// Use only if need to check hangfire and mailing is working or not.
        /// Add your mail Id and test
        /// </summary>
        /// <returns></returns>
        public async Task TestHangfire()
        {
            var repository = _unitOfWork?.GetRepository<Store>();

            var dset = await repository.ExecuteStoredProcedure(StoredProcedures.TestScheduler).ConfigureAwait(false);


            List<ReportMailUserModel> users = new List<ReportMailUserModel>();


            var newUser = new ReportMailUserModel();
            newUser.Email = "archawanjare@cdnsol.com";
            newUser.FirstName = "TEST";
            newUser.Id = 1001;

            users.Add(newUser);

            byte[] stream = Array.Empty<byte>();

            await _mailService.SendReportMail(users, stream, "mail is working", "").ConfigureAwait(false);
        }

        public async Task GetSchedulerInQueue()
        {
            var repository = _unitOfWork?.GetRepository<Store>();

            var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetSchedulerIds).ConfigureAwait(false);
            List<SchedulerIdResponseModel> schedulerIds = MappingHelpers.ConvertDataTable<SchedulerIdResponseModel>(dset.Tables[0]);


            if (schedulerIds.Count > 0)
            {
                foreach (var val in schedulerIds)
                {
                    await SendGeneratedReport(val.Id).ConfigureAwait(false);
                }
            }

            // Check if there are failed schedulers report or mails
            if (dset.Tables.Count > 1)
            {
                //Get Failed Reports data ReportSchedulerLogModel
                List<ReportSchedulerLogModel> failedScheduler = MappingHelpers.ConvertDataTable<ReportSchedulerLogModel>(dset.Tables[1]);

                foreach (var log in failedScheduler)
                {
                    if (log.IsReportGenerated == false && log.ReportTryCount < 3 && log.IsActive != Status.Active && log.IsReported == false)
                    {
                        //generate report again}
                        await SendGeneratedReport(log.SchedulerId).ConfigureAwait(false);
                    }
                    if (log.IsEmailSent == false && log.EmailTryCount < 3 && log.IsActive != Status.Active && log.IsReported == false)
                    {
                        //resend Email //need to find a solution to save generated report and send that again.
                        await SendGeneratedReport(log.SchedulerId).ConfigureAwait(false);
                    }

                }
            }
        }

        public Task<byte[]> GetReportBySchedulerId(ReportSchedulerResponseModel reportScheduler)
        {
            try
            {

                var reportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, ReportRequestModel>(reportScheduler);
                string mime = "application/pdf"; // MIME header with PDF
                byte[] stream = Array.Empty<byte>();

                if (reportScheduler != null && !string.IsNullOrEmpty(reportScheduler.ReportName))
                {
                    //Need to change this after discussion with Manoj sir

                    //if (reportScheduler.PdfExport==true)
                    //{
                    reportScheduler.Format = "PDF";
                    //}
                    //if (reportScheduler.ExcelExport == true)
                    //{
                    //    reportScheduler.Format = "EXCEL";
                    //}


                    switch (reportScheduler.ReportName.Trim())
                    {
                        case "SalesDepartment":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.Department.ToString(), mime);
                            break;
                        case "SalesNilTransaction":
                            var saleseportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesNilTransaction>(reportScheduler);
                            stream = _reportService.GetSalesNilTransaction(saleseportRequest, mime);
                            break;
                        case "SalesCommodity":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.Commodity.ToString(), mime);

                            break;
                        case "HourlySales":
                            stream = _reportService.GetItemsWithHourlySales(reportRequest, ReportType.Commodity.ToString(), mime);

                            break;
                        case "SalesCategory":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.Category.ToString(), mime);

                            break;
                        case "SalesGroup":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.Group.ToString(), mime);

                            break;
                        case "SalesSupplier":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.Supplier.ToString(), mime);

                            break;
                        case "SalesOutlet":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.Outlet.ToString(), mime);

                            break;
                        case "NoSales":
                            stream = _reportService.GetItemSales(reportRequest, ReportType.NoSales.ToString(), mime);

                            break;
                        case "StockVariance":
                            stream = _reportService.GetStockVariance(reportRequest, mime);
                            break;
                        //to add user Id here
                        //case "StockOnHandReport":
                        //    stream = _reportService.GetStockOnHand(reportRequest, mime); 
                        //    break;
                        case "StockAdjustment":
                            stream = _reportService.GetStockAdjustment(reportRequest, mime);

                            break;
                        case "StockWastage":
                            _reportService.GetStockWastageProductWise(reportRequest, mime);

                            break;
                        case "CostVarience":
                            var costReportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, CostVarianceFilters>(reportScheduler);
                            _reportService.CostVarience(costReportRequest, mime);

                            break;
                        case "StockPurchase":
                            var stockReportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, StockPurchaseReportRequest>(reportScheduler);
                            stream = _reportService.GetStockPurchase(stockReportRequest, mime);
                            break;
                        //case "SaleTrxSheet":
                        //    var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                        //    stream = _reportService.GetSalesAndStockTrxSheet(reportRequest, securityViewModel, ReportType.Sales.ToString(), filter), Formatting.Indented));
                        //    break;
                        case "JournalSalesFinancialSummary":
                            var jourReportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, JournalSalesRequestModel>(reportScheduler);
                            stream = _reportService.GetJournalSalesFinancialSummary(jourReportRequest, mime);

                            break;
                        case "JournalSalesRoyaltySummary":
                            var jourRoyaltyReportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, JournalSalesRequestModel>(reportScheduler);
                            stream = _reportService.GetJournalSalesRoyaltyAndAdvertisingSummary(jourRoyaltyReportRequest, ReportType.Royalty.ToString(), mime);
                            break;
                        case "JournalSalesAdvertisingSummary":
                            var jourSalesReportRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, JournalSalesRequestModel>(reportScheduler);
                            stream = _reportService.GetJournalSalesRoyaltyAndAdvertisingSummary(jourSalesReportRequest, ReportType.Advertising.ToString(), mime);
                            break;
                        case "SalesDepartmentSummary":
                            var salesDeptReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesDeptReq, ReportType.Department.ToString(), mime);
                            break;
                        case "SalesCommoditySummary":
                            var salesComReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesComReq, ReportType.Commodity.ToString(), mime);
                            break;
                        case "SalesCategorySummary":
                            var salesCatReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesCatReq, ReportType.Category.ToString(), mime);
                            break;
                        case "SalesGroupSummary":
                            var salesGrpReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesGrpReq, ReportType.Group.ToString(), mime);
                            break;
                        case "SalesSupplierSummary":
                            var salesSuppReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesSuppReq, ReportType.Supplier.ToString(), mime);
                            break;
                        case "SalesOutletSummary":
                            var salesOutReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesOutReq, ReportType.Outlet.ToString(), mime);
                            break;
                        case "SalesMemberSummary":
                            var salesMembReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(salesMembReq, ReportType.Member.ToString(), mime);
                            break;
                        case "RankingByOutlet":
                            var rankOutRequest = MappingHelpers.Mapping<ReportSchedulerResponseModel, RankingOutletRequestModel>(reportScheduler);
                            stream = _reportService.GetRankingByOutlet(rankOutRequest, mime);
                            break;
                        case "ItemWithNoSalesSummary":
                            var salesItemReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            salesItemReq.stockSOHButNoSales = true;
                            salesItemReq.SalesSOH = GeneralFieldFilter.Equals;
                            salesItemReq.salesSOHRange = 0;
                            salesItemReq.OrderByAmt = true;
                            salesItemReq.stockSOHLevel = true;
                            stream = _reportService.GetItemSalesSummary(salesItemReq, ReportType.Commodity.ToString(), mime);
                            break;
                        case "PurchaseDepartmentSummary":
                            var purchDeptReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemPurchaseSummary(purchDeptReq, ReportType.Department.ToString(), mime);
                            break;
                        case "PurchaseCommoditySummary":
                            var purchComReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(purchComReq, ReportType.Commodity.ToString(), mime);
                            break;
                        case "PurchaseCategorySummary":
                            var purchCatReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(purchCatReq, ReportType.Category.ToString(), mime);
                            break;
                        case "PurchaseGroupSummary":
                            var purchGrpReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(purchGrpReq, ReportType.Group.ToString(), mime);
                            break;
                        case "PurchaseSupplierSummary":
                            var purchSuppReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(purchSuppReq, ReportType.Supplier.ToString(), mime);
                            break;
                        case "PurchaseOutletSummary":
                            var purchOutReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(purchOutReq, ReportType.Member.ToString(), mime);
                            break;
                        case "Ranging":
                            var rangingReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, RangingRequestModel>(reportScheduler);
                            stream = _reportService.GetRanging(rangingReq, mime);
                            break;
                        case "NationalRanging":
                            var natRangReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, NationalRangingRequestModel>(reportScheduler);
                            stream = _reportService.GetNationalRanging(natRangReq, mime);
                            break;
                        case "ItemWithNoSalesProduct":
                            var noSaleReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, NoSalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemWithNoSalesProduct(noSaleReq, ReportType.ItemNoSales.ToString(), mime);
                            break;
                        case "LessthenXdaysStock":
                            var xDayReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(xDayReq, ReportType.LessthenXdays.ToString(), mime);
                            break;
                        case "ItemWithNegativeSOH":
                            var negReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(negReq, ReportType.ItemWithNegativeSOH.ToString(), mime);
                            break;
                        case "ItemWithZeroSOH":
                            var zeroReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(zeroReq, ReportType.ItemWithZeroSOH.ToString(), mime);
                            break;
                        case "ItemWithSlowMovingStock":
                            var slowMovReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetItemSalesSummary(slowMovReq, ReportType.ItemWithSlowMovingStock.ToString(), mime);
                            break;
                        case "ProductPriceDeviation":
                            var pricedevReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, NationalRangingRequestModel>(reportScheduler);
                            stream = _reportService.GetProductPriceDeviation(pricedevReq, mime);
                            break;
                        case "NationalLevelSalesSummary":
                            var natReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, NationalLevelRequestModel>(reportScheduler);
                            stream = _reportService.GetNationalLevelSalesSummary(natReq, mime);
                            break;
                        case "KPIRanking":
                            var kpiReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, KPIRankingRequestModel>(reportScheduler);
                            stream = _reportService.GetKPIRanking(kpiReq, mime);
                            break;
                        case "SalesHistoryChartByDepartment":
                            var salesHisReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesHistoryRequestModle>(reportScheduler);
                            stream = _reportService.GetSalesHistoryChart(salesHisReq, mime, ReportType.Department.ToString());
                            break;
                        case "SalesHistoryChartByCommodity":
                            var salesHisComReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesHistoryRequestModle>(reportScheduler);
                            stream = _reportService.GetSalesHistoryChart(salesHisComReq, mime, ReportType.Commodity.ToString());
                            break;
                        case "SalesHistoryChartByOutlet":
                            var salesHisOutReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesHistoryRequestModle>(reportScheduler);
                            stream = _reportService.GetSalesHistoryChart(salesHisOutReq, mime, ReportType.Outlet.ToString());
                            break;
                        case "SalesHistoryChartBySupplier":
                            var salesHisSuppReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesHistoryRequestModle>(reportScheduler);
                            stream = _reportService.GetSalesHistoryChart(salesHisSuppReq, mime, ReportType.Supplier.ToString());
                            break;
                        case "SalesHistoryChartByCategory":
                            var salesHisCatReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesHistoryRequestModle>(reportScheduler);
                            stream = _reportService.GetSalesHistoryChart(salesHisCatReq, mime, ReportType.Category.ToString());
                            break;
                        case "SalesHistoryChartByGroup":
                            var salesHisGroupReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, SalesHistoryRequestModle>(reportScheduler);
                            stream = _reportService.GetSalesHistoryChart(salesHisGroupReq, mime, ReportType.Group.ToString());
                            break;
                        case "Financial":
                            var finReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, FinancialSummaryRequestModel>(reportScheduler);
                            stream = _reportService.GetFinancialSummary(finReq, mime);
                            break;
                        case "StockTakePrint":
                            var stockReq = MappingHelpers.Mapping<ReportSchedulerResponseModel, StockTakePrintRequestModel>(reportScheduler);
                            stream = _reportService.GetStockTakePrint(stockReq, mime);
                            break;

                    }
                }
                return Task.FromResult(stream);
            }
            catch (Exception ex)
            {
                //Report generation failed, save this in table.
                _schedulertService.SaveReportFailLog((long)reportScheduler.ID, reportScheduler.ReportName, ex.Message).ConfigureAwait(false);
                byte[] stream = Array.Empty<byte>();
                return Task.FromResult(stream);

            }


        }


        public async Task SendGeneratedReport(long schedulerId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Store>();
                var reportModel = await _schedulertService.GetActiveSchedulerById(schedulerId, true).ConfigureAwait(false);

                if (reportModel.IsActive == Status.Active)
                {
                    var report = await GetReportBySchedulerId(reportModel).ConfigureAwait(false);

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Id",schedulerId)
                    };

                    var dsetUser = await repository.ExecuteStoredProcedure(StoredProcedures.GetSchedulerEmails, dbParams.ToArray()).ConfigureAwait(false);
                    List<ReportMailUserModel> users = MappingHelpers.ConvertDataTable<ReportMailUserModel>(dsetUser.Tables[0]);

                    if (users.Count > 0 && report != null)
                    {
                        if (!await _mailService.SendReportMail(users, report, reportModel.FilterBody, "").ConfigureAwait(false))
                        {
                            await _schedulertService.SaveReportEmailFailLog(schedulerId, reportModel.ReportName, report).ConfigureAwait(false);
                        }
                    }

                    await _schedulertService.UpdateLastRun(schedulerId).ConfigureAwait(false);
                }
            }

#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                //Report generation failed, save this in table.
                await _schedulertService.SaveReportFailLog(1, "", ex.Message).ConfigureAwait(false);

            }
        }

    }
}
