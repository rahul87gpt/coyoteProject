using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("PromoOfferProduct")]
    public class PromotionOfferProduct : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }

        public int PromotionOfferId { get; set; }
        public virtual PromotionOffer PromotionOffer { get; set; }
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

        public byte OfferGroup { get; set; } = 1;
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
