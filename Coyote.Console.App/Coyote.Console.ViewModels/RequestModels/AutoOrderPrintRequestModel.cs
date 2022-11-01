using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderPrintRequestModel
    {
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        [Required]
        public int StoreId { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public int OrderNo { get; set; }
    }
}
