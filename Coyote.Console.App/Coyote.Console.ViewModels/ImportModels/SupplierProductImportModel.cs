using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class SupplierProductImportModel
    {
        public string SupplierCode { get; set; }
        public string SupplierItem { get; set; }
        public long ProductNumber { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        public float CartonCost { get; set; }
        public int? MinReorderQty { get; set; }
        public bool BestBuy { get; set; }
    }
}
