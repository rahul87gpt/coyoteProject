using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("PromoOffer")]
    public class PromotionOffer : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public int PromotionId { get; set; }
        public virtual Promotion Promotion { get; set; }
        public bool Status { get; set; }

        public float? TotalQty { get; set; }
        public float? TotalPrice { get; set; }
        public float? Group1Qty { get; set; }
        public float? Group2Qty { get; set; }
        public float? Group3Qty { get; set; }
       
        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength)]
        public string  Group1Price { get; set; }
        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength)]
        public string  Group2Price { get; set; }
        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength)]
        public string  Group3Price { get; set; }
        public short Group { get; set; }
        public virtual ICollection<PromotionOfferProduct> PromotionOfferProduct { get; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

    }
}
