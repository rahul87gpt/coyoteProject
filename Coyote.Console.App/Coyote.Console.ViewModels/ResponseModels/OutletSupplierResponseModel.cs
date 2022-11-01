using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OutletSupplierResponseModel : OutletSupplierRequestModel
    {
        public int Id { get; set; }

        public string StoreCode { get; set; }
        public string StoreName { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierDesc { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }

        
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }

        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
