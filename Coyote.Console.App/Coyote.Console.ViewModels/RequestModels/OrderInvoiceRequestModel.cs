using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderInvoiceRequestModel : ReportRequest
    {
        public bool ShowOrderHistory { get; set; }
        public bool UseInvoiceDates { get; set; }
        public DateTime? OrderPostedDateFrom { get; set; }
        public DateTime? OrderPostedDateTo { get; set; }

        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }

    }
}
