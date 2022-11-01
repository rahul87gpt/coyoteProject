using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StoreRequestModel
    {
        [Required]
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreCode)]
        public string Code { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.StoreDescLength, ErrorMessage = ErrorMessages.StoreDesc)]
        public string Desc { get; set; }
        public int? GroupId { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress1)]
        public string Address1 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress2)]
        public string Address2 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress3)]
        public string Address3 { get; set; }
        [MaxLength(MaxLengthConstants.StorePhoneNoLength, ErrorMessage = ErrorMessages.StorePhoneNumber)]
        public string PhoneNumber { get; set; }
        [MaxLength(MaxLengthConstants.StorePhoneNoLength, ErrorMessage = ErrorMessages.StoreFax)]
        public string Fax { get; set; }
        [MaxLength(MaxLengthConstants.StorePostCodeLength, ErrorMessage = ErrorMessages.StorePostCode)]
        public string PostCode { get; set; }

        public bool Status { get; set; }
      
        public int? PriceZoneId { get; set; }
        public int? CostZoneId { get; set; }
        public bool SellingInd { get; set; } = false;
        public bool StockInd { get; set; } = false;
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelName)]
        public string DelName { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelAddr1)]
        public string DelAddr1 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelAddr2)]
        public string DelAddr2 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelAddr3)]
        public string DelAddr3 { get; set; }
        [MaxLength(MaxLengthConstants.StorePostCodeLength, ErrorMessage = ErrorMessages.StoreDelPostCode)]
        public string DelPostCode { get; set; }
        [MaxLength(MaxLengthConstants.StoreStatusLength, ErrorMessage = ErrorMessages.StoreCostType)]
        public string CostType { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAbn)]
        public string Abn { get; set; }

        public double? BudgetGrowthFact { get; set; }
        public bool? CostInd { get; set; }

        [MaxLength(MaxLengthConstants.MinStoreLength, ErrorMessage = ErrorMessages.StoreEntityNumber)]
        public string EntityNumber { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StorePriceLevelDesc)]
        public string PriceLevelDesc1 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StorePriceLevelDesc)]
        public string PriceLevelDesc2 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StorePriceLevelDesc)]
        public string PriceLevelDesc3 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StorePriceLevelDesc)]
        public string PriceLevelDesc4 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StorePriceLevelDesc)]
        public string PriceLevelDesc5 { get; set; }
        public int? WarehouseId { get; set; }

        public int? OutletPriceFromOutletId { get; set; }

        public int? PriceFromLevel { get; set; }


        public bool? FuelSite { get; set; }

        public int? LabelTypeShelfId { get; set; }

        public int? LabelTypePromoId { get; set; }

        public int? LabelTypeShortId { get; set; }

        public StoreTradingHoursRequest StoreTradingHours { get; set; }
        public AppStoreRequestModel AppStoreDetails { get; set; }
        public List<SupplierOrderScheduleRequestModel> OutletSupplierSchedules { get; set; }
        public List<RoyaltyScalesRequestModel> RoyaltyScales { get; set; }
        public List<RoyaltyScalesRequestModel> AdvertisingRoyaltyScales { get; set; }
    }

    public class StoreTradingHoursRequest
    {
        //        public int OutetId { get; set; }
        public string MonOpenTime { get; set; }
        public string MonCloseTime { get; set; }
        public string TueOpenTime { get; set; }
        public string TueCloseTime { get; set; }
        public string WedOpenTime { get; set; }
        public string WedCloseTime { get; set; }
        public string ThuOpenTime { get; set; }
        public string ThuCloseTime { get; set; }
        public string FriOpenTime { get; set; }
        public string FriCloseTime { get; set; }
        public string SatOpenTime { get; set; }
        public string SatCloseTime { get; set; }
        public string SunOpenTime { get; set; }
        public string SunCloseTime { get; set; }
    }

    public class AppStoreRequestModel
    {
        [MaxLength(MaxLengthConstants.StoreLatitudeLength, ErrorMessage = ErrorMessages.StoreNameOnApp)]
        public string NameOnApp { get; set; }

        [MaxLength(MaxLengthConstants.StoreLatitudeLength, ErrorMessage = ErrorMessages.StoreAddressOnApp)]
        public string AddressOnApp { get; set; }

        public bool? DisplayOnApp { get; set; }

        public bool? AppOrders { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(MaxLengthConstants.StoreOpenHoursLength, ErrorMessage = ErrorMessages.StoreOpenHours)]
        public string OpenHours { get; set; }
        [MaxLength(MaxLengthConstants.MaxEmailLength, ErrorMessage = ErrorMessages.StoreEmail)]
        public string Email { get; set; }

    }

    public class RoyaltyScalesRequestModel 
    {
        public float ScalesFrom { get; set; }
        public float ScalesTo { get; set; }
        public float Percent { get; set; }
        public bool IncGST { get; set; }
    }
}
