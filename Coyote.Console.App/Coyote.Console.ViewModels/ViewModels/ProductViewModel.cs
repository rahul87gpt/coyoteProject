using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; }

        public long Number { get; set; }

        [MaxLength(MaxLengthConstants.MinProductDescLength, ErrorMessage = ErrorMessages.ProductDescLength)]
        [Required]
        public string Desc { get; set; }

        [MaxLength(MaxLengthConstants.MinProductPosDescLength, ErrorMessage = ErrorMessages.ProductPosDescLength)]
        public string PosDesc { get; set; }

        public bool Status { get; set; }

        public int CartonQty { get; set; }

        public float UnitQty { get; set; }

        public float CartonCost { get; set; }

        public int DepartmentId { get; set; }


        public int SupplierId { get; set; }

        public int CommodityId { get; set; }

        public long TaxId { get; set; }

        public int GroupId { get; set; }

        public int CategoryId { get; set; }

        public int ManufacturerId { get; set; }

        public int TypeId { get; set; }

        public int NationalRangeId { get; set; }

        public int? UnitMeasureId { get; set; }

        public int? AccessOutletId { get; set; }

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
        public float? LabelQty { get; set; }
        public long? Replicate { get; set; }
        [MaxLength(MaxLengthConstants.MinProductFreightLength, ErrorMessage = ErrorMessages.ProductFreightLength)]
        public string Freight { get; set; }
        [MaxLength(MaxLengthConstants.MinProductSizeLength, ErrorMessage = ErrorMessages.ProductSizeLength)]
        public string Size { get; set; }
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

        [JsonIgnore]
        public DateTime? DeletedAt { get; set; }
        [JsonIgnore]
        public DateTime? DeactivatedAt { get; set; }
        [JsonIgnore]
        public System.DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public System.DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public int CreatedById { get; set; }
        [JsonIgnore]
        public int UpdatedById { get; set; }
    }
}
