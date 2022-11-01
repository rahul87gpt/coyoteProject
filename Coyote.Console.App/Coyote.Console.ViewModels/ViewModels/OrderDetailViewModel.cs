using Coyote.Console.Common;
using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class OrderDetailViewModel : OrderDetailRequestModel
    {
        public long Id { get; set; }
        public long OrderHeaderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }


        public string OrderTypeName { get; set; }
        public string OrderTypeCode { get; set; }
        //public long Number { get; set; }
        //public string Desc { get; set; }
        public string SupplierProductDesc { get; set; }
        public string SupplierProductItem { get; set; }
        public string SupplierDesc { get; set; }
        public string SupplierCode { get; set; }

        public string TaxCode { get; set; }
    }
}
