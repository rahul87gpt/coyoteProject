using Coyote.Console.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class ProductRequestModel
    {
     //   public int Id { get; set; }
       [Required]
        public long Number { get; set; }
        [MaxLength(MaxLengthConstants.MinProductDescLength, ErrorMessage = ErrorMessages.ProductDescLength)]
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

        public List<int> AccessOutletIds { get; set; }
        
        public List<long> APNNumber { get; set; }

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
        public float? TareWeight { get; set; }
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
        public IFormFile Image { get; set; }

        public List<OutletProductRequestModel> OutletProduct { get; set; }
        public List<SupplierProductRequestModel> SupplierProduct { get; set; }

    }
}
