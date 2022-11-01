using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
   public class SupplierOrderScheduleResponseModel : SupplierOrderScheduleRequestModel
    {
       // public int Id { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierDesc { get; set; }
        public string UserName { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
