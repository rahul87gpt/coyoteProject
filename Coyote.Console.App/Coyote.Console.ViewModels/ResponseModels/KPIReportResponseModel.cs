using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class KPIReportResponseModel
    {
        public List<KPIReportStoreList> ReportList { get; } = new List<KPIReportStoreList>();
        public KPIReportTotal ReportTotal { get; set; }
        public KPIReportTotal ReportAverage { get; set; }
        public List<KPIReportDepartment> DepartmentReport { get; } = new List<KPIReportDepartment>();
    }

    public class KPIReportStoreList
    {
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public decimal YTDTY { get; set; }
        public decimal YTDDiffTYLY { get; set; }
        public decimal Sales { get; set; }
        public decimal SalesLY { get; set; }
        public decimal SalesTYLY { get; set; }
        public decimal SalesTYLYPerc { get; set; }
        public decimal GPPerc { get; set; }
        public decimal GPPercTYLY { get; set; }
        public decimal CustDiffTYLY { get; set; }
        public decimal CustCount { get; set; }
        public decimal SalesCustTY { get; set; }
        public decimal SalesCustTYLY { get; set; }
        public decimal ItemBsktTY { get; set; }
        public decimal ItemBsktTYLY { get; set; }

        public List<KPIReportDepartment> DepartmentReport { get; } = new List<KPIReportDepartment>();
    }
    public class KPIReportTotal
    {
        public decimal GtYtdAmt { get; set; }
        public decimal GtAmtDiffYtd { get; set; }
        public decimal GtAmt { get; set; }
        public decimal GtAmtLY { get; set; }
        public decimal GtAmtDiff { get; set; }
        public decimal GtAmtDiffPcnt { get; set; }
        public decimal GtGP { get; set; }
        public decimal GtGPDiffPcnt { get; set; }
        public decimal GtCustomers { get; set; }
        public decimal GtCustDiff { get; set; }
        public decimal GtAvgSale { get; set; }
        public decimal GtAvgSaleDiff { get; set; }
        public decimal GtAvgItems { get; set; }
        public decimal GtAvgItemsDiff { get; set; }
    }
    public class KPIReportAverage
    {
        public decimal GtYtdAmt { get; set; }
        public decimal GtAmtDiffYtd { get; set; }
        public decimal GtAmt { get; set; }
        public decimal GtAmtLY { get; set; }
        public decimal GtAmtDiff { get; set; }
        public decimal GtAmtDiffPcnt { get; set; }
        public decimal GtGP { get; set; }
        public decimal GtGPDiffPcnt { get; set; }
        public decimal GtCustomers { get; set; }
        public decimal GtCustDiff { get; set; }
        public decimal GtAvgSale { get; set; }
        public decimal GtAvgSaleDiff { get; set; }
        public decimal GtAvgItems { get; set; }
        public decimal GtAvgItemsDiff { get; set; }
    }

    public class KPIReportDepartment
    {
        public long Id { get; set; }
        public int StoreId { get; set; }
        public int DeptId { get; set; }
        public string DeptPerct { get; set; }
        public string DeptPerctText { get; set; }
        public string DeptTYLY { get; set; }
        public string DeptTYLYText { get; set; }
        public decimal DeptPerctValue { get; set; }
        public decimal DeptTYLYValue { get; set; }
    }
}
