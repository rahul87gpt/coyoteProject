using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PromotionBuyingRequestModel
    {
        public long ProductId { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionProductDescLength, ErrorMessage = ErrorMessages.PromotionProductDescLength)]
        public string Desc { get; set; }
        public bool Status { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionProductActionLength, ErrorMessage = ErrorMessages.PromotionProductActionLength)]
        public string Action { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionProductHostPromoTypeLength, ErrorMessage = ErrorMessages.PromotionProductHostPromoTypeLength)]
        public string HostPromoType { get; set; }
        public float? AmtOffNorm1 { get; set; }
        public float? PromoUnits { get; set; }
        //common
        //from product table
        public DateTime? CostStart { get; set; }
        public DateTime? CostEnd { get; set; }
        public bool? CostIsPromInd { get; set; }

        //from product table

        public float CartonCost { get; set; }
        public int CartonQty { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
    }
}
