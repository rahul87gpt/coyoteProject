using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("PromoMixmatchProduct")]
    public class PromotionMixmatchProduct : IKeyIdentifier<long>, IAuditable<int>
    {
        [Key]
        public long Id { get; set; }
        public int PromotionMixmatchId { get; set; }
        public virtual PromotionMixmatch PromotionMixmatch { get; set; }
        //common
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
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

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
    }
}
