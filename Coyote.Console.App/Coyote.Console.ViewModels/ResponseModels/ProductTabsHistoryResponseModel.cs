using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class ProductTabsHistoryResponseModel
    {
        public PagedOutputModel<List<PurchaseHistoryModel>> PurchaseList { get; set; }
        public PagedOutputModel<List<WeeklySalesHistoryModel>> WeeklySalesList { get; set; }
        public PagedOutputModel<List<TransactionHistoryModel>> TransactionList { get; set; }
        public PagedOutputModel<List<ProductChildModel>> ProductChildrenList { get; set; } 
        public PagedOutputModel<List<ProductStockMovementModel>> StockMovementList { get; set; } 

        public PagedOutputModel<List<ProductZonePricingModel>> ZonePricingList { get; set; }
        public PagedOutputModel<List<UserLogResponseModel<APNResponseViewModel>>> APNProductHistory { get; set; } 
        public PagedOutputModel<List<UserLogResponseModel<OutletProductResponseViewModel>>> OutletProductHistroy { get; set; } 
        public PagedOutputModel<List<UserLogResponseModel<ProductResponseModel>>> ProductHistory { get; set; } 
    }

    public class PurchaseHistoryModel
    {
        public int Outlet { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
        public long OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DatePosted { get; set; }
        public string DocType { get; set; }
        public string DocStatus { get; set; }
        public string SupplierCode { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public float Cartons { get; set; }
        public float Units { get; set; }
        public float? CartonCost { get; set; }
        public float LineTotal { get; set; }
        public float CartonQty { get; set; }
        public string SupplierItem { get; set; }
        public float? InvoiceTotal { get; set; }
        public string DelDocNo { get; set; }
        public DateTime? DelDocDate { get; set; }
        public DateTime Timestamp { get; set; }


    }

    public class WeeklySalesHistoryModel
    {
        public int OutletId { get; set; }
        public string OutletDesc { get; set; }
        public string OutletCode { get; set; }
        public DateTime WeekEnding { get; set; }
        public float Quantity { get; set; }
        public float SalesCost { get; set; }
        public float SalesAmt { get; set; }
        public float AvgItemPrice { get; set; }
        public float Margin { get; set; }
        public float GP { get; set; }
        public double Discount { get; set; }
        public float PromoSales { get; set; }
    }

    public class TransactionHistoryModel
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Type { get; set; }
        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public int TillId { get; set; }
        public string TillCode { get; set; }
        public string TillDesc { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public double AmtGst { get; set; }
        public double Cost { get; set; }
        public double CostGst { get; set; }
        public double Discount { get; set; }
        public string SubType { get; set; }
        public int PromSellId { get; set; }
        public string PromSellCode { get; set; }
        public string PromSellDesc { get; set; }
        public int PromBuyId { get; set; }
        public string PromBuyCode { get; set; }
        public string PromBuyDesc { get; set; }
        public float? StockUnitMovement { get; set; }
        public float? Parent { get; set; }
        public long ProductId { get; set; }
        public long ProductNumber { get; set; }
        public string ProductDesc { get; set; }
        public long? Member { get; set; }
        public float? NewUnitOnHand { get; set; }
        public string Reference { get; set; }
        public bool? Manual { get; set; }
        public float? CartonQty { get; set; }
        public float? SellUnit { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int HistDeptId { get; set; }
        public string HistDeptCode { get; set; }
        public string HistDeptDesc { get; set; }
        public int HistSupplierId { get; set; }
        public string HistSupplierCode { get; set; }
        public string HistSupplierDesc { get; set; }

    }

    public class ProductChildModel
    {
        public long Id { get; set; }
        public long Number { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        public string TypeCode { get; set; }
        public string Type { get; set; }
        public float UnitQty { get; set; }
        public int CartonQty { get; set; }
    }

    public class ProductStockMovementModel
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public long ProductId { get; set; }
        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public double Qty { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierDesc { get; set; }
        public float? Parent { get; set; }
        public float? StockMovement { get; set; }
        public float? NewOnHand { get; set; }
        public DateTime Timestamp { get; set; }
    }


    public class ProductZonePricingModel {

        public int Id { get; set; }
        public int PriceZoneId { get; set; }
        public string PriceZoneCode { get; set; }
        public string PriceZoneName { get; set; }
        public float? CtnCost { get; set; }
        public DateTime? CtnCostDate { get; set; }
        public float? CtnCostSV1 { get; set; }
        public DateTime? CtnCostDateSV1 { get; set; }
        public float? CtnCostSV2 { get; set; }

        public DateTime? CtnCostDateSV2 { get; set; }

        public float? CtnCostStd { get; set; }
        public float? Price { get; set; }
        public float? PriceSV1 { get; set; }
        public float? PriceSV2 { get; set; }

        public DateTime? PriceDate { get; set; }
        public DateTime? PriceDateSV1 { get; set; }
        public DateTime? PriceDateSV2 { get; set; }
        public float? MinReorderQty { get; set; }
        public DateTime CreatedAt { get; set; }    

    }
}
