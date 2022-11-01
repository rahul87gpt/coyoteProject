using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class ProductImportModel
    {
        public long Number { get; set; }
        public string Desc { get; set; }
        public string PosDesc { get; set; }
        public bool Status { get; set; }
        public int CartonQty { get; set; }
        public float UnitQty { get; set; }
        public float CartonCost { get; set; }
        public string Department { get; set; }
        public string Supplier { get; set; }
        public string Commodity { get; set; }
        public string Tax { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string Type { get; set; }
        public string NationalRange { get; set; }
        public string UnitMeasure { get; set; }
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
        public string Replicate { get; set; }
        public string Freight { get; set; }
        public string Size { get; set; }
        public float? Litres { get; set; }
        public bool? VarietyInd { get; set; }
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
        public string AccessOutletIds { get; set; }
        public int? StoreId { get; set; }
        public float? TareWeight { get; set; }
        public string Info { get; set; }
    }
}
