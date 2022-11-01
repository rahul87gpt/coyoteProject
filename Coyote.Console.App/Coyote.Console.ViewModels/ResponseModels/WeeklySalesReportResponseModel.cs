using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class WeeklySalesWorkbookResponseModel
    {

        public List<WeeklySalesReportResponseModel> WeeklySalesReports { get; } = new List<WeeklySalesReportResponseModel>();
        public WeeklySalesTotal WeeklySalesTotal { get; set; }
        public List<OutletBudgetTargetModel> OutletBudgetTargets { get; } = new List<OutletBudgetTargetModel>();
        public OutletBudgetTotal OutletBudgetTotal { get; set; }
    }

    public class WeeklySalesReportResponseModel
    {
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public decimal AvgItemsTy { get; set; }
        public decimal AvgItemsLy { get; set; }
        public decimal CustomersTy { get; set; }
        public decimal CustomersLy { get; set; }
        public decimal AvgSaleTy { get; set; }
        public decimal AvgSaleLy { get; set; }
        public decimal SalesIncTy { get; set; }
        public decimal SalesIncLy { get; set; }
        public decimal BudgetTy { get; set; }
        public decimal ProfitTy { get; set; }
        public decimal ProfitLy { get; set; }
        public decimal SalesExTy { get; set; }
        public decimal SalesExLy { get; set; }
        public decimal GPTy { get; set; }
        public decimal GPLy { get; set; }
    }

    public class WeeklySalesTotal
    {
        public decimal? CustomersTyTotal { get; set; }
        public decimal? CustomersLyTotal { get; set; }
        public decimal? AvgSalesTyTotal { get; set; }
        public decimal? AvgSalesLyTotal { get; set; }
        public decimal? SalesIncTyTotal { get; set; }
        public decimal? SalesIncLyTotal { get; set; }
        public decimal? BudgetTotal { get; set; }
        public decimal? ProfitTyTotal { get; set; }
        public decimal? ProfitLyTotal { get; set; }
        public decimal? SalesExTyTotal { get; set; }
        public decimal? SalesExLyTotal { get; set; }
        public decimal? GPTy { get; set; }
        public decimal? GPLy { get; set; }
    }

    public class OutletBudgetTargetModel
    {

        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public float? EmployHoursTarget { get; set; }
        public float? EmployHoursActual { get; set; }
        public float? MDROverTarget { get; set; }
        public float? MDROverActual { get; set; }
        public float? StockOnHandTarget { get; set; }
        public float? StockOnHandActual { get; set; }
        public float? StockPurchTarget { get; set; }
        public float? StockPurchActual { get; set; }
        public float? StockAdjustTarget { get; set; }
        public float? StockAdjustActual { get; set; }
        public float? WastageTarget { get; set; }
        public float? WastageActual { get; set; }
        public float? VoidsTarget { get; set; }
        public float? VoidsActual { get; set; }
        public float? RefundsTarget { get; set; }
        public float? RefundsActual { get; set; }
        public float? MarkdownTarget { get; set; }
        public float? MarkdownActual { get; set; }
    }
    public class OutletBudgetTotal
    {
        public float? EmployHoursTargetTotal { get; set; }
        public float? MDROverTargetTotal { get; set; }
        public float? StockOnHandTargetTotal { get; set; }
        public float? StockPurchaseTargetTotal { get; set; }
        public float? StockAdjustTargetTotal { get; set; }
        public float? WastageTargetTotal { get; set; }
        public float? VoidsTargetTotal { get; set; }
        public float? RefundsTargetTotal { get; set; }
        public float? MarkdownTargetTotal { get; set; }
        public float? StockOnHandActualTotal { get; set; }
        public float? StockPurchaseActualTotal { get; set; }
        public float? StockAdjustActualTotal { get; set; }
        public float? WastageActualTotal { get; set; }
        public float? VoidsActualTotal { get; set; }
        public float? RefundsActualTotal { get; set; }
        public float? MarkdownActualTotal { get; set; }
    }
}
