using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockTakeHeaderRequestModel
    {
        [Required]
        public int OutletId { get; set; }
        public DateTime? PostToDate { get; set; }
        [MaxLength(MaxLengthConstants.StoreDescLength, ErrorMessage = ErrorMessages.StockTakeLength)]
        [Required(ErrorMessage = ErrorMessages.DescriptionRequired)]
        public string Desc { get; set; }
        public float Total { get; set; }

        public List<StockTakeDetailRequestModel> StockTakeDetail { get; set; }
    }
}
