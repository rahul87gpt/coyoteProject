using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockProductResponseViewModel
    {
        public float? SumOHandCost { get; set; }
        public float? SumOnHand { get; set; }
        public float? SumOnHandCTNS { get; set; }
        public long ProductNumber { get; set; }
        public string ProdDesc { get; set; }
        public string DepartmentCode { get; set; }
        public int CartonQty { get; set; }
        public string TaxCode { get; set; }
        public string Replicate { get; set; }
        public string DepartmentDesc { get; set; }
        public float? SumOnHandExCost { get; set; }
        public int OutletId { get; set; }
    }
}
