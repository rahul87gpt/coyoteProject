using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class ReportFilterDataViewModel
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string selectOutlets { get; set; }
        public long productStartId { get; set; }
        public long productEndId { get; set; }
        public string till { get; set; }
        public bool contineous { get; set; }
        public bool drillDown { get; set; }
        public bool summary { get; set; }
        public bool promoSales { get; set; }
        public string promoCode { get; set; }
        public bool merge { get; set; }
        public bool variance { get; set; }
        public bool wastage { get; set; }
        public string reportName { get; set; }
        public bool orderByQty { get; set; }
        public bool orderByAmt { get; set; }
        public bool orderByGP { get; set; }
        public bool orderByMargin { get; set; }
        public bool orderBySOH { get; set; }
        public bool orderByAlph { get; set; }
        public bool stockNegativeOH { get; set; }
        public bool stockSOHLevel { get; set; }
        public bool stockSOHButNoSales { get; set; }
        public bool stockLowWarn { get; set; }
        public int stockNoOfDaysWarn { get; set; }
        public int salesAMT { get; set; }
        public int salesSOH { get; set; }
        public double salesAMTRange { get; set; }
        public double salesSOHRange { get; set; }
        public string ReportDuration { get; set; }
        public bool Chart { get; set; }
        public string ChartReportTitle { get; set; }
        public int TotalAvgQty { get; set; }
        public float TotalAvgAmount { get; set; }
        public string CurrentDate { get; set; }
        public string NillTransactionInterval { get; set; }
        public string Shrinkage { get; set; }
        //public string GrandTotalGPPer { get; set; }
        //public string HeaderDateColumnName { get; set; }
        //public string IsContineous { get; set; }

    }
}
