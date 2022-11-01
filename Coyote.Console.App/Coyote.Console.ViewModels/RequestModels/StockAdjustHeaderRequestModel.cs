using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockAdjustHeaderRequestModel
    {
        [Required]
        public int OutletId { get; set; } 
        public DateTime? PostToDate { get; set; }
        [MaxLength(MaxLengthConstants.MaxStockAdjustReferenceLength, ErrorMessage = ErrorMessages.StockAdjustLength)]
        //[Required(ErrorMessage =ErrorMessages.ReferenceRequired)]
        public string Reference { get; set; }
        public float Total { get; set; }
        public float? ConfirmTotal { get; set; }
        [MaxLength(MaxLengthConstants.MaxStockAdjustDescriptionLength, ErrorMessage = ErrorMessages.StockAdjustmentDescriptionLength)]
        [Required]
        public string Description { get; set; }
        public List<StockAdjustDetailRequestModel> StockAdjustDetail { get; set; }
    }
}
