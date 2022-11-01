using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class SupplierProductRequestModel
    {
        public int SupplierId { get; set; }
        [MaxLength(MaxLengthConstants.MaxSupplierItemCode, ErrorMessage = ErrorMessages.SupplierProductItemCodeLength)]
        public string SupplierItem { get; set; }
        public long ProductId { get; set; }
        [MaxLength(MaxLengthConstants.MinSupplierProductDescLength, ErrorMessage = ErrorMessages.SupplierProductDescLength)]
        public string Desc { get; set; }
        public bool Status { get; set; }
        public float CartonCost { get; set; }
        public int? MinReorderQty { get; set; }
        public bool? BestBuy { get; set; }

    }
}
