using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OptimalOrderBatchResponseModel
    {
        public double Outlet { get;set;}
        public double OrderNo { get;set;}
        public string Supplier { get;set;}
        public string SupplierName { get;set;}
        public DateTime OrderDate { get;set;}
        public string OrderType { get;set;}
        public string OrderStatus { get;set;}
        public double LineNum { get;set;}
        public double Product { get;set;}
        public string ProducDesc { get;set;}
        public bool InvBuy { get;set;}
        public bool PromoBuy { get;set;}
        public bool NormalBuy { get;set;}
        public int NomalCoverDays { get;set;}
        public int CoverDaysUsed { get;set;}
        public double MinReorderQty { get;set;}
        public double NonPromoSalesRateDaily { get;set;}
        public double PromoSalesRateDaily { get;set;}
        public double MinOnHand { get;set;}
        public double PromoUnits { get;set;}
        public double OnHand { get;set;}
        public double OnOrder { get;set;}
        public double CartonQty { get;set;}
        public double Cartons { get;set;}
        public double Units { get;set;}
        public int? TradingCoverDays { get;set;}
        public double NormalCartonCost { get;set;}
        public double FinalCartonCostUsed { get;set;}
        public double LineTotal { get;set;}
        public string SalePromoCode { get;set;}
        public string SalePromoEndDate { get;set;}
        public string BuyPromoCode { get;set;}
        public double BuyPromoDisc { get;set;}
        public DateTime? BuyPromoEndDate { get;set;}
        public bool CheckSupplier { get;set;}
        public string CheaperSupplier { get;set;}
        public bool Perishable { get;set;}
        public double NonPromoSales56Days { get;set;}
        public double PromoSales56Days { get;set;}
                                                                   
    }
}
