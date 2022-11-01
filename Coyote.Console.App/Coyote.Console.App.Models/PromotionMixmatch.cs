using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("PromoMixmatch")]
    public class PromotionMixmatch : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public int PromotionId { get; set; }
        public virtual Promotion Promotion { get; set; }
        public float? Qty1 { get; set; }
        public float? Amt1 { get; set; }
        public float? DiscPcnt1 { get; set; }
        public int? PriceLevel1 { get; set; }
        public float? Qty2 { get; set; }
        public float? Amt2 { get; set; }
        public float? DiscPcnt2 { get; set; }
        public int? PriceLevel2 { get; set; }
        public bool CumulativeOffer { get; set; }
        public short Group { get; set; }
        public virtual ICollection<PromotionMixmatchProduct> PromotionMixmatchProduct { get; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

    }
}
