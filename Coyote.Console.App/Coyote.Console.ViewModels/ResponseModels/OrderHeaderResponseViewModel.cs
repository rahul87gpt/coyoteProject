using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OrderHeaderResponseViewModel : OrderRequestModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
       // public long? OrderNo { get; set; }
        public string StoreDesc { get; set; }
        public string StoreCode { get; set; }
        public string SupplierDesc { get; set; }
        public string SupplierCode { get; set; }
        public string CreationTypeCode { get; set; }
        public string CreationTypeName { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }


    }
}
