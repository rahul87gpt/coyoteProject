using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class SupplierProductResponseViewModel : SupplierProductRequestModel
    {
        public long Id { get; set; }
        public long ProductNumber { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierDesc { get; set; }
        public float? LastInvoiceCost { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public int CreatedById { get; set; } 
        public int UpdatedById { get; set; } 

    }
}
