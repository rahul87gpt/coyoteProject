using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockPurchaseReportRequest : ReportRequestModel
    {
        public DateTime OrderInvoiceStartDate { get; set; }
        public DateTime OrderInvoiceEndDate { get; set; }
        public bool UseInvoiceDates { get; set; }
        public bool? IsRebates { get; set; }
    }

}