using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Product")]
    public class Product : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public long Number { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MinProductDescLength, ErrorMessage = ErrorMessages.ProductDescLength)]
        public string Desc { get; set; }
        [MaxLength(MaxLengthConstants.MinProductPosDescLength, ErrorMessage = ErrorMessages.ProductPosDescLength)]
        public string PosDesc { get; set; }
        public bool Status { get; set; }
        public int CartonQty { get; set; }
        public float UnitQty { get; set; }
        public float CartonCost { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public int CommodityId { get; set; }
        public virtual Commodity Commodity { get; set; }
        public long TaxId { get; set; }
        public virtual Tax Tax { get; set; }
        public int GroupId { get; set; }
        public virtual MasterListItems GroupMasterListItem { get; set; }
        public int CategoryId { get; set; }
        public virtual MasterListItems CategoryMasterListItem { get; set; }
        public int ManufacturerId { get; set; }
        public virtual MasterListItems ManufacturerMasterListItem { get; set; }
        public int TypeId { get; set; }
        public virtual MasterListItems TypeMasterListItem { get; set; }
        public int NationalRangeId { get; set; }
        public virtual MasterListItems NationalRangeMasterListItem { get; set; }
        public int? UnitMeasureId { get; set; }
        public virtual MasterListItems UnitMeasureMasterListItem { get; set; }
        public string AccessOutletIds { get; set; }
        public bool? ScaleInd { get; set; }
        public bool? GmFlagInd { get; set; }
        public bool? SlowMovingInd { get; set; }
        public bool? WarehouseFrozenInd { get; set; }
        public bool? StoreFrozenInd { get; set; }
        public bool? AustMadeInd { get; set; }
        public bool? AustOwnedInd { get; set; }
        public bool? OrganicInd { get; set; }
        public bool? HeartSmartInd { get; set; }
        public bool? GenericInd { get; set; }
        public bool? SeasonalInd { get; set; }
        public long? Parent { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Product ProdParent { get; set; }
        public float? TareWeight { get; set; }
        public float? LabelQty { get; set; }
        [MaxLength(MaxLengthConstants.ProdReplicateLength, ErrorMessage = ErrorMessages.ProductReplicateLength)]
        public string Replicate { get; set; }
        [MaxLength(MaxLengthConstants.MinProductFreightLength, ErrorMessage = ErrorMessages.ProductFreightLength)]
        public string Freight { get; set; }
        [MaxLength(MaxLengthConstants.MinProductSizeLength, ErrorMessage = ErrorMessages.ProductSizeLength)]
        public string Size { get; set; }

        [MaxLength(MaxLengthConstants.MaxInfoLength, ErrorMessage = ErrorMessages.ProductSizeLength)]
        public string Info { get; set; }
        public float? Litres { get; set; }
        public bool? VarietyInd { get; set; }

        [MaxLength(MaxLengthConstants.MinProductHostNumberLength, ErrorMessage = ErrorMessages.ProductHostNumberLength)]
        public string HostNumber { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostNumberLength, ErrorMessage = ErrorMessages.ProductHostNumberLength)]
        public string HostNumber2 { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostNumberLength, ErrorMessage = ErrorMessages.ProductHostNumberLength)]
        public string HostNumber3 { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostNumberLength, ErrorMessage = ErrorMessages.ProductHostNumberLength)]
        public string HostNumber4 { get; set; }

        [MaxLength(MaxLengthConstants.MinProductHostItemTypeLength, ErrorMessage = ErrorMessages.ProductHostItemTypeLength)]
        public string HostItemType { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostItemTypeLength, ErrorMessage = ErrorMessages.ProductHostItemTypeLength)]
        public string HostItemType2 { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostItemTypeLength, ErrorMessage = ErrorMessages.ProductHostItemTypeLength)]
        public string HostItemType3 { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostItemTypeLength, ErrorMessage = ErrorMessages.ProductHostItemTypeLength)]
        public string HostItemType4 { get; set; }
        [MaxLength(MaxLengthConstants.MinProductHostItemTypeLength, ErrorMessage = ErrorMessages.ProductHostItemTypeLength)]

        public string HostItemType5 { get; set; }
        public long? LastApnSold { get; set; }
        public float? Rrp { get; set; }
        public bool? AltSupplier { get; set; }
        public string ImagePath { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DeletedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeactivatedAt { get; set; }
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
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public virtual ICollection<APN> APNProduct { get; }
        public virtual ICollection<OutletProduct> OutletProductProduct { get; }     
        public virtual ICollection<SupplierProduct> SupplierProduct { get; }
        public virtual ICollection<PromotionBuying> PromotionBuying { get; }
        public virtual ICollection<PromotionMemberOffer> PromotionMemberOffer { get; }
        public virtual ICollection<PromotionMixmatchProduct> PromotionMixmatchProduct { get; }
        public virtual ICollection<PromotionOfferProduct> PromotionOfferProduct { get; }
        public virtual ICollection<PromotionSelling> PromotionSellingchProduct { get; }
        public virtual ICollection<OrderDetail> OrderDetailProduct { get; }
        public virtual ICollection<StockAdjustDetail> StockAdjustDetailProduct { get; }
        public virtual ICollection<StockTakeDetail> StockTakeDetailProduct { get; }
        public virtual ICollection<PromotionCompetition> PromotionCompetitionProduct { get; }
        public virtual ICollection<JournalDetail> JournalDetailsProduct { get; }
        public virtual ICollection<Transaction> TransactionProduct { get; }
        public virtual ICollection<TransactionReport> TransactionReportProduct { get; }
        public virtual ICollection<ZonePricing> ProductZonePricing { get; }
        public virtual ICollection<ManualSaleItem> ManualSaleItemProduct { get; }
        public virtual ICollection<Recipe> RecipeProduct { get; }
        public virtual ICollection<Recipe> RecipeIngredientProduct { get; }
        public virtual ICollection<RebateDetails> RebateProducts { get; }
        public virtual ICollection<EPay> EPAYProducts { get; }

        public virtual ICollection<AccountTransaction> AccountTransactionProduct { get; }
        public virtual ICollection<KeypadButton> ButtonnTypeProduct { get; }

        public virtual ICollection<HostUpdChange> HostUpdChanges { get; }
    }
}
