using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class GLAccountResponseModel : GLAccountRequestModel
    {
        public int Id { get; set; }
        public string AccountSystem { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierDesc { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
