using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class WarehouseResponseViewModel: WarehouseRequestModel
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string HostFormatName { get; set; }
        public string SupplierCode { get; set; }
        public string HostFormatCode { get; set; }
    }
}
