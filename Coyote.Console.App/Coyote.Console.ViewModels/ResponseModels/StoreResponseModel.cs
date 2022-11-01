using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StoreResponseModel// : StoreRequestModel
    {
        public int Id { get; set; }
        public bool GroupIsDeleted { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string StoreDetail { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TillResponseModel> Tills { get; } = new List<TillResponseModel>();
        public List<int> Zones { get; } = new List<int>();
        public string Code { get; set; }
        public string Desc { get; set; }
        public int? GroupId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string PostCode { get; set; }

        public bool Status { get; set; }

        public int? PriceZoneId { get; set; }
        public int? CostZoneId { get; set; }
        public bool SellingInd { get; set; } = false;
        public bool StockInd { get; set; } = false;
        public string DelName { get; set; }
        public string DelAddr1 { get; set; }
        public string DelAddr2 { get; set; }
        public string DelAddr3 { get; set; }
        public string DelPostCode { get; set; }
        public string CostType { get; set; }
        public string Abn { get; set; }

        public double? BudgetGrowthFact { get; set; }
        public bool? CostInd { get; set; }


        public string EntityNumber { get; set; }
        public string PriceLevelDesc1 { get; set; }
        public string PriceLevelDesc2 { get; set; }
        public string PriceLevelDesc3 { get; set; }
        public string PriceLevelDesc4 { get; set; }
        public string PriceLevelDesc5 { get; set; }
        public int? WarehouseId { get; set; }

        public int? OutletPriceFromOutletId { get; set; }

        public int? PriceFromLevel { get; set; }


        public bool? FuelSite { get; set; }

        public int? LabelTypeShelfId { get; set; }

        public int? LabelTypePromoId { get; set; }

        public int? LabelTypeShortId { get; set; }

        public bool? LabelTypeShelfIsDeleted { get; set; }
        public string LabelTypeShelfCode { get; set; }
        public string LabelTypeShelfDesc { get; set; }
        public bool? LabelTypePromoIsDeleted { get; set; }
        public string LabelTypePromoCode { get; set; }
        public string LabelTypePromoDesc { get; set; }
        public bool? LabelTypeShortIsDeleted { get; set; }
        public string LabelTypeShortCode { get; set; }
        public string LabelTypeShortDesc { get; set; }
        public int? PriceZoneIsdeleted { get; set; }
        public string PriceZoneCode { get; set; }
        public string PriceZoneDesc { get; set; }
        public int? CostZoneIsDeleted { get; set; }
        public string CostZoneCode { get; set; }
        public string CostZoneDesc { get; set; }
        public bool? WarehouseIsDeleted { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseDesc { get; set; }
        public StoreTradingHoursRequest StoreTradingHours { get; set; }
        public AppStoreRequestModel AppStoreDetails { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public List<SupplierOrderScheduleResponseModel> OutletSupplierSchedules { get; set; } = new List<SupplierOrderScheduleResponseModel>();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning disable CA2227 // Collection properties should be read only
        public List<RoyaltyScalesRequestModel> RoyaltyScales { get; set; } = new List<RoyaltyScalesRequestModel>();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning disable CA2227 // Collection properties should be read only
        public List<RoyaltyScalesRequestModel> AdvertisingRoyaltyScales { get; set; } = new List<RoyaltyScalesRequestModel>();
#pragma warning restore CA2227 // Collection properties should be read only
    }

    public class AnonymousStoreResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }        
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string PostCode { get; set; }

        public bool Status { get; set; }
        public string Email { get; set; }
        public string IpAddress { get; set; }
        public string Network { get; set; }
    }
}
