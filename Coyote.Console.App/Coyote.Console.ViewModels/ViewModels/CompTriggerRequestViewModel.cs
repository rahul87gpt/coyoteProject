using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class CompTriggerRequestViewModel
    {
        public int TriggerProductGroupID { get; set; } 
        public bool? Share { get; set; }
        public float LoyaltyFactor { get; set; }

        public long ProductId { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxPromotionProductDescLength, ErrorMessage = ErrorMessages.PromotionProductDescLength)]
        public string Desc { get; set; }

    }
}
