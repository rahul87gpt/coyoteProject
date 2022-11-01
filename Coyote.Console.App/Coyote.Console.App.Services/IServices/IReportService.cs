using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface IReportService
    {
        byte[] GetItemSales(ReportRequestModel request, string type, string mime);
        byte[] GetStockPurchase(StockPurchaseReportRequest request, string mime);
        byte[] GetStockVariance(ReportRequestModel request, string mime);
        byte[] GetStockOnHand(ReportRequestModel request, string mime,int userId);
        byte[] GetStockWastageProductWise(ReportRequestModel request, string mime);
        byte[] GetStockAdjustment(ReportRequestModel request, string mime);
        byte[] CostVarience(CostVarianceFilters request, string mime);
        byte[] GetJournalSalesFinancialSummary(JournalSalesRequestModel request, string mime);
        byte[] GetJournalSalesRoyaltyAndAdvertisingSummary(JournalSalesRequestModel request, string type, string mime);
        DataSet GetSalesAndStockTrxSheet(StockTrxSheetRequestModel request, SecurityViewModel securityViewModel, string type, PagedInputModel filter = null);
        byte[] GetPrintLabel(PrintLabelRequestModel request, string mime);
        Task<ReportFilterationResponseModel> ReportFilterationDropdownList(SecurityViewModel securityViewModel, PagedInputModel filter = null);
        DataSet SalesChart(ReportRequestModel request, string reportFor);
        DataSet SalesChartById(ReportRequestModel request, string reportFor);
        DataSet SalesChartDetailed(ReportRequestModel request, string reportFor);
        byte[] GetItemSalesSummary(SalesSummaryRequestModel request, string type, string mime);
        byte[] GetItemWithNoSalesProduct(NoSalesSummaryRequestModel request, string type, string mime);
        byte[] GetRankingByOutlet(RankingOutletRequestModel request, string mime);
        byte[] GetItemPurchaseSummary(SalesSummaryRequestModel request, string type, string mime);
        byte[] GetRanging(RangingRequestModel request, string mime);
        byte[] GetNationalRanging(NationalRangingRequestModel request, string mime);
        byte[] GetProductPriceDeviation(NationalRangingRequestModel request, string mime);
        byte[] GetNationalLevelSalesSummary(NationalLevelRequestModel request, string mime);
        byte[] GetItemsWithHourlySales(ReportRequestModel request, string type, string mime);
        byte[] GetKPIRanking(KPIRankingRequestModel request, string mime);
        byte[] GetSalesHistoryChart(SalesHistoryRequestModle request, string mime, string reportFor);
        byte[] GetSalesNilTransaction(SalesNilTransaction request, string mime);
        byte[] GetFinancialSummary(FinancialSummaryRequestModel request, string mime); // GetStockTakePrint
        byte[] GetStockTakePrint(StockTakePrintRequestModel request, string mime);

        byte[] GetStoreDashboard(StoreDashboardRequest request, string mime);
        string GetReporterStoreKPI(ReporterStoreKPIReport request);

        DataSet GetPrintLabelDataSet(PrintLabelRequestModel request, string mime);
        byte[] GetTillJournal(TillJournalReportModel requestModel, string mime, SecurityViewModel securityViewModel = null);
        byte[] GetBasketIncidentReport(BasketIncidentFilters requestModel, string mime, SecurityViewModel securityViewModel = null);
        AutomaticOrderResponseModel AutomaticOrder(AutomaticOrderRequestModel viewModel, string mime, int userId);
        byte[] AutomaticOrderPrint(OrderPrintRequestModel requestModel, string mime);
        byte[] NormalOrderPrint(OrderPrintRequestModel reportRequest, string mime);
		byte[] WeeklySalesWorkBook(WeeklySalesWorkBookRequestModel request, string mime);
        byte[] StockAdjustmentPrint(StockAdjustPrintRequestModel reportRequest, string mime);
        DataSet GetReportData(string commandText, Microsoft.Data.SqlClient.SqlParameter[] sqlParameters);
        DataSet GetItemSales2(ReportRequestModel reportRequest, string v, string mime);
    }
}