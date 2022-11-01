using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class PromotionSellingViewModel
    {
        public int Id { get; set; }
        public int PromotionId { get; set; }

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

        public float Price { get; set; }
        public float? Price1 { get; set; }
        public float? Price2 { get; set; }
        public float? Price3 { get; set; }
        public float? Price4 { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
