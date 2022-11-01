using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("OutletProduct")]
    public class OutletProduct : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public bool Status { get; set; }
        public bool? Till { get; set; }
        public bool? OpenPrice { get; set; }
        public float NormalPrice1 { get; set; }
        public float? NormalPrice2 { get; set; }
        public float? NormalPrice3 { get; set; }
        public float? NormalPrice4 { get; set; }
        public float? NormalPrice5 { get; set; }
        public float CartonCost { get; set; }
        public float? CartonCostHost { get; set; }
        public float? CartonCostInv { get; set; }
        public float? CartonCostAvg { get; set; }
        //public string MemeberOfferPromoCode { get; set; }
        //public string SellPromoCode { get; set; }
        //public string BuyPromoCode { get; set; }
        public float QtyOnHand { get; set; }
        public float MinOnHand { get; set; }
        public float? MaxOnHand { get; set; }
        public float? MinReorderQty { get; set; }
        public int? PickingBinNo { get; set; } = 0;
        public bool? ChangeLabelInd { get; set; }
        public bool? ChangeTillInd { get; set; }
        public string HoldNorm { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ChangeLabelPrinted { get; set; }
        public float? LabelQty { get; set; }
        public bool? ShortLabelInd { get; set; }
        public bool SkipReorder { get; set; }
        public float? SpecPrice { get; set; }
        public string SpecCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SpecFrom { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SpecTo { get; set; }
        public string GenCode { get; set; }
        public float? SpecCartonCost { get; set; }
        public float? ScalePlu { get; set; }
        public bool? FifoStock { get; set; }
        public float? Mrp { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }

        // Added Columns for Promotion

        public float? PromoPrice1 { get; set; }
        public float? PromoPrice2 { get; set; }
        public float? PromoPrice3 { get; set; }
        public float? PromoPrice4 { get; set; }
        public float? PromoPrice5 { get; set; }
        public float? PromoCartonCost { get; set; }


        public int? PromoMixMatch1Id { get; set; }
        public virtual Promotion PromotionMixMatch1 { get; set; }
        public int? PromoMixMatch2Id { get; set; }
        public virtual Promotion PromotionMixMatch2 { get; set; }
        public int? PromoOffer1Id { get; set; }
        public virtual Promotion PromotionOffer1 { get; set; }
        public int? PromoOffer2Id { get; set; }
        public virtual Promotion PromotionOffer2 { get; set; }
        public int? PromoOffer3Id { get; set; }
        public virtual Promotion PromotionOffer3 { get; set; }
        public int? PromoOffer4Id { get; set; }
        public virtual Promotion PromotionOffer4 { get; set; }
        public long? PromoCompId { get; set; }
        public virtual CompetitionDetail PromotionComp { get; set; }
        public int? PromoSellId { get; set; }
        public virtual Promotion PromotionSell { get; set; }
        public int? PromoBuyId { get; set; }
        public virtual Promotion PromotionBuy { get; set; }
        public int? PromoMemeberOfferId { get; set; }
        public virtual Promotion PromotionMember { get; set; }

        public float? PromoMemOffPrice1 { get; set; }
        public float? PromoMemOffPrice2 { get; set; }
        public float? PromoMemOffPrice3 { get; set; }
        public float? PromoMemOffPrice4 { get; set; }
        public float? PromoMemOffPrice5 { get; set; }
        public bool? PromoMemOff { get; set; }
        public bool? AllMember { get; set; }
        public bool? PromoBuy{ get; set; }
        public bool? PromoSell { get; set; }
        public bool? PromoMix1 { get; set; }
        public bool? PromoMix2 { get; set; }
        public bool? PromoOffer1 { get; set; }
        public bool? PromoOffer2 { get; set; }
        public bool? PromoOffer3 { get; set; }
        public bool? PromoOffer4 { get; set; }
        public bool? PromoComp { get; set; }

    public virtual Users UserCreatedBy { get; set; }
    public virtual Users UserUpdatedBy { get; set; }
    public virtual ICollection<StockAdjustDetail> StockAdjustDetailOutletProduct { get; }
    public virtual ICollection<StockTakeDetail> StockTakeDetailOutletProduct { get; }
}
}
