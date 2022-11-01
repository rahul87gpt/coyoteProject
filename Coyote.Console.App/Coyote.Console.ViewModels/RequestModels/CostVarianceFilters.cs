using System;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class CostVarianceFilters 
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public long StoreId { get; set; }
        public string SupplierId { get; set; }
        public DateTime InvoiceDateFrom { get; set; }
        public DateTime InvoiceDateTo { get; set; }
        public bool IsHostCost { get; set; }
        public bool IsNormalCost { get; set; }
        public bool IsSupplierBatchCost { get; set; }
        public string SupplierBatch { get; set; }
    }
}
