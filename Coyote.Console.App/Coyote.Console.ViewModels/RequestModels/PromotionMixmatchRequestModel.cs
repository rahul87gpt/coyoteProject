using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PromotionMixmatchRequestModel
    {
        public int? Id { get; set; }
        public bool Status { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Qty1 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Amt1 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? DiscPcnt1 { get; set; }

        public int? PriceLevel1 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Qty2 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Amt2 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? DiscPcnt2 { get; set; }

        public int? PriceLevel2 { get; set; }
        public bool CumulativeOffer { get; set; }
        public short Group { get; set; }
    }
}
