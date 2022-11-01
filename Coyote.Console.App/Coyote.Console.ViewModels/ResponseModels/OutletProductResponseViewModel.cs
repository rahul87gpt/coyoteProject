using Coyote.Console.App.Models;
using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OutletProductResponseViewModel : OutletProductRequestModel
    {
        public long Id { get; set; }
        public long ProductNumber { get; set; }
        public string ProductDesc { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierDesc { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public string MixMatch1PromoCode { get; set; }
        public string MixMatch2PromoCode { get; set; }
        public string Offer1PromoCode { get; set; }
        public string Offer2PromoCode { get; set; }
        public string Offer3PromoCode { get; set; }
        public string Offer4PromoCode { get; set; }
        public string MemberOfferCode { get; set; }

        public int CartonQty { get; set; }
        public string Replicate { get; set; }
        public float UnitQty { get; set; }
        public int TaxId { get; set; }
        public string TaxCode { get; set; }
        public string TaxDesc { get; set; }
        public int CommodityId { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityDesc { get; set; }
        public int GroupId { get; set; }
        public  string GroupCode { get; set; }
        public  string GroupDesc { get; set; }
        public int CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }


        public int TypeId { get; set; }
        public string TypeCode { get; set; }
        public string Type { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentDesc { get; set; }
        public string Parent { get; set; }

        public float? ItemCost { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public int CreatedById { get; set; } 
        public int UpdatedById { get; set; }

        public decimal? GP { get; set; }
        public bool? DefaultSupplier { get; set; }
        public int? PriceFromLevel { get; set; }
        public int? OutletPriceFromOutletId { get; set; }
        public string OutletPriceFromOutletCode { get; set; }
        public string OutletPriceFromOutletDesc { get; set; }

        //public float? CartonCostInv { get; set; }

    }
}
