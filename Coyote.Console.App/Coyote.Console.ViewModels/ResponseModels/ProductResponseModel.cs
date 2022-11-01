using Coyote.Console.App.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
  public class ProductResponseModel : ProductNumberModel
  {

    public bool? Status { get; set; }
    public long Id { get; set; }
    // public long Number { get; set; }
    public string Desc { get; set; }
    public int? ActiveProductCount { get; set; }
    public int CommodityId { get; set; }
    public bool CommodityIsDeleted { get; set; }
    public string CommodityCode { get; set; }
    public string Commodity { get; set; }
    public int DepartmentId { get; set; }
    public bool DepartmentIsDeleted { get; set; }
    public string DepartmentCode { get; set; }
    public string Department { get; set; }

    public int GroupId { get; set; }
    public bool GroupIsDeleted { get; set; }

    public string GroupCode { get; set; }
    public string Group { get; set; }

    public int CategoryId { get; set; }
    public bool CategoryIsDeleted { get; set; }

    public string CategoryCode { get; set; }
    public string Category { get; set; }

    public int SupplierId { get; set; }
    public bool SupplierIsDeleted { get; set; }
    public string SupplierCode { get; set; }
    public string Supplier { get; set; }
    public string SupplierProductItem { get; set; }
    public long SupplierProductId { get; set; }
    public int? SupplierProductCount { get; set; }

    public string Replicate { get; set; }
    public int TypeId { get; set; }
    public bool TypeIsDeleted { get; set; }
    public string Type { get; set; }
    public string TypeCode { get; set; }
    public long TaxId { get; set; }
    public bool TaxIsDeleted { get; set; }
    public string TaxCode { get; set; }
    public string Tax { get; set; }

    public int ManufacturerId { get; set; }
    public bool ManufacturerIsDeleted { get; set; }

    public string ManufacturerCode { get; set; }
    public string Manufacturer { get; set; }

    public int? UnitMeasureId { get; set; }
    public bool UnitMeasureIsDeleted { get; set; }
    public string UnitMeasureCode { get; set; }
    public string UnitMeasure { get; set; }

    public int CartonQty { get; set; }

    public float? CartonCost { get; set; }
    public float? TareWeight { get; set; }
    public float UnitQty { get; set; }
    public long? Parent { get; set; }
    public float? ParentCartonQty { get; set; }
    public string ParentDesc { get; set; }

    public int? ChildrenProductCount { get; set; }
    public string PosDesc { get; set; }
    public int NationalRangeId { get; set; }
    public bool NationalRangeIsDeleted { get; set; }
    public string NationalRange { get; set; }
    public string NationalRangeCode { get; set; }
    public List<int> AccessOutlets { get; } = new List<int>();

        public string AccessOutletsCSV { get; set; }

        public List<long> APNNumbers { get; } = new List<long>();
    public bool? VarietyInd { get; set; }
    public bool? SlowMovingInd { get; set; }
    public string ImagePath { get; set; }
    public string ImageUploadStatusCode { get; set; }


    public bool? ScaleInd { get; set; }
    public bool? GmFlagInd { get; set; }
    public bool? WarehouseFrozenInd { get; set; }
    public bool? StoreFrozenInd { get; set; }
    public bool? AustMadeInd { get; set; }
    public bool? AustOwnedInd { get; set; }
    public bool? OrganicInd { get; set; }
    public bool? HeartSmartInd { get; set; }
    public bool? GenericInd { get; set; }
    public bool? SeasonalInd { get; set; }
    public float? LabelQty { get; set; }

    public string Freight { get; set; }
    public string Size { get; set; }
    public string Info { get; set; }
    public float? Litres { get; set; }
    public string HostCode4 { get; set; }
    public string HostCode5 { get; set; }

    public string HostNumber { get; set; }
    public string HostNumber2 { get; set; }

    public string HostNumber3 { get; set; }

    public string HostNumber4 { get; set; }

    public string HostItemType { get; set; }
    public string HostItemType2 { get; set; }
    public string HostItemType3 { get; set; }
    public string HostItemType4 { get; set; }
    public string HostItemType5 { get; set; }
    public long? LastApnSold { get; set; }
    public float? Rrp { get; set; }
    public bool? AltSupplier { get; set; }

    public DateTime? DeletedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public System.DateTime CreatedAt { get; set; }
    public System.DateTime UpdatedAt { get; set; }
    public int CreatedById { get; set; }
    public int UpdatedById { get; set; }

    public List<PromotionResponseViewModel> Promotions { get; } = new List<PromotionResponseViewModel>();

    public List<SupplierProductResponseViewModel> SupplierProducts { get; } = new List<SupplierProductResponseViewModel>();
    public int SupplierProductsCount { get; set; }
    public List<OutletProductResponseViewModel> OutletProducts { get; } = new List<OutletProductResponseViewModel>();

    public int? OutletProductCount { get; set; }
  }

  public class ProductNumberModel
  {

    public long Number { get; set; }
    public string HostCode { get; set; }
    public string HostCode2 { get; set; }
    public string HostCode3 { get; set; }
    public List<string> HostCodes { get; } = new List<string>();
    public List<MasterListItemResponseViewModel> UnitMeasureList { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> CategoryList { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> NationalRangeList { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> GroupList { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> TypeList { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> ManufacturerList { get; } = new List<MasterListItemResponseViewModel>();
    public List<TaxResponseModel> TaxList { get; } = new List<TaxResponseModel>();
    public List<CommodityResponseModel> CommodityList { get; } = new List<CommodityResponseModel>();
    public List<DepartmentResponseModel> DepartmentList { get; } = new List<DepartmentResponseModel>();
    public List<SupplierResponseViewModel> SupplierList { get; } = new List<SupplierResponseViewModel>();
    public List<ProductChildModel> ProductChildrenList { get; } = new List<ProductChildModel>();
  }

}
