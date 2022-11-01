using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PromotionOfferRequestModel
    {
        public int? Id { get; set; }
        public bool Status { get; set; }

        [Range(0, float.MaxValue,ErrorMessage =ErrorMessages.PositiveNumberOnly)]
        public float? TotalQty { get; set; }
       
        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? TotalPrice { get; set; }
      
        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Group1Qty { get; set; }
     
        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Group2Qty { get; set; }
        
        [Range(0, float.MaxValue, ErrorMessage = ErrorMessages.PositiveNumberOnly)]
        public float? Group3Qty { get; set; }

        public string Group1Price { get; set; }
        public string Group2Price { get; set; }
        public string Group3Price { get; set; }
        public short Group { get; set; }
    }
}
