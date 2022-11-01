using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class PromotionOfferProductViewModel
    {
        public int Id { get; set; }

        public int PromotionOfferId { get; set; }
        //common
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

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
