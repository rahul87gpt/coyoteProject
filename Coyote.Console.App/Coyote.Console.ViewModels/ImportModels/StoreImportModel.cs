using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class StoreImportModel
    {
        public string Code { get; set; }
        public string GroupCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string PostCode { get; set; }
        public bool Status { get; set; }
        public string Desc { get; set; }
        public string Email { get; set; }
        public bool SellingInd { get; set; }
        public bool StockInd { get; set; }
        public string DelName { get; set; }
        public string DelAddr1 { get; set; }
        public string DelAddr2 { get; set; }
        public string DelAddr3 { get; set; }
        public string DelPostCode { get; set; }
        public string CostType { get; set; }
        public string Abn { get; set; }
        public float? BudgetGrowthFact { get; set; }
        public string EntityNumber { get; set; }
        public string WarehouseCode { get; set; }
        public int? PriceFromLevel { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string OpenHours { get; set; }
        public bool? FuelSite { get; set; }
        public string NameOnApp { get; set; }
        public string AddressOnApp { get; set; }
        public bool? DisplayOnApp { get; set; }
        public bool? AppOrders { get; set; }
        public string CostZoneCode { get; set; }
        public string PriceLevelDesc1 { get; set; }
        public string PriceLevelDesc2 { get; set; }
        public string PriceLevelDesc3 { get; set; }
        public string PriceLevelDesc4 { get; set; }
        public string PriceLevelDesc5 { get; set; }
        public string PriceZoneCode { get; set; }
        public string LabelTypePromoCode { get; set; }
        public string LabelTypeShelfCode { get; set; }
        public string LabelTypeShortCode { get; set; }
        public string OutletPriceFromOutletCode { get; set; }
        public bool? CostInd { get; set; }
    }
}
