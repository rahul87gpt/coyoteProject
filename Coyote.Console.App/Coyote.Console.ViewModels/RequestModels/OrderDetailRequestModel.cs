using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderDetailRequestModel
    {

        public int OrderTypeId { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }
        public long? SupplierProductId { get; set; }
        public float? CartonCost { get; set; }
        public float Cartons { get; set; }
        public float Units { get; set; }
        public float TotalUnits { get; set; }
        public float? DeliverCartons { get; set; }
        public float? DeliverUnits { get; set; }
        public float? DeliverTotalUnits { get; set; }
        public float LineTotal { get; set; }
        public float CartonQty { get; set; }
        public bool? BonusInd { get; set; }
        public DateTime? PostedDate { get; set; }
        public float? FinalLineTotal { get; set; }
        public float? FinalCartonCost { get; set; }
        public float? OnHand { get; set; }
        public float? OnOrder { get; set; }
        public float? Sold { get; set; }
        public float? CoverUnits { get; set; }
        public float? NonPromoMinOnHand { get; set; }
        public float? PromoMinOnHand { get; set; }
        public float? NonPromoAvgDaily { get; set; }
        public float? PromoAvgDaily { get; set; }


        public int? NormalCoverDays { get; set; }
        public int? CoverDaysUsed { get; set; }
        public int? MinReorderQty { get; set; }
        public bool? Perishable { get; set; }
        public float? NonPromoSales56Days { get; set; }
        public float? PromoSales56Days { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderBuyPromoCodeLength, ErrorMessage = ErrorMessages.OrderBuyPromoCodeLength)]
        public string BuyPromoCode { get; set; }
        public DateTime? BuyPromoEndDate { get; set; }
        public float? BuyPromoDisc { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderSalePromoCodeLength, ErrorMessage = ErrorMessages.OrderSalePromoCodeLength)]
        public string SalePromoCode { get; set; }

        public DateTime? SalePromoEndDate { get; set; }
        public bool? CheckSuuplier { get; set; }
        public int? CheaperSupplierId { get; set; }
        public bool NewProduct { get; set; }
    
        //Used for Supplier Item Update only

        public long? Number { get; set; }
        public string Desc { get; set; }
    
    }

    public class OrderDetailRefreshRequestModel
    {
        public int Id { get; set; }
        public int OrderTypeId { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }
        public long? SupplierProductId { get; set; }
        public float? CartonCost { get; set; }
        public float Cartons { get; set; }
        public float Units { get; set; }
        public float TotalUnits { get; set; }
        public float? DeliverCartons { get; set; }
        public float? DeliverUnits { get; set; }
        public float? DeliverTotalUnits { get; set; }
        public float LineTotal { get; set; }
        public float CartonQty { get; set; }
        public bool? BonusInd { get; set; }
        public DateTime? PostedDate { get; set; }
        public float? FinalLineTotal { get; set; }
        public float? FinalCartonCost { get; set; }
        public float? OnHand { get; set; }
        public float? OnOrder { get; set; }
        public float? Sold { get; set; }
        public float? CoverUnits { get; set; }
        public float? NonPromoMinOnHand { get; set; }
        public float? PromoMinOnHand { get; set; }
        public float? NonPromoAvgDaily { get; set; }
        public float? PromoAvgDaily { get; set; }


        public int? NormalCoverDays { get; set; }
        public int? CoverDaysUsed { get; set; }
        public int? MinReorderQty { get; set; }
        public bool? Perishable { get; set; }
        public float? NonPromoSales56Days { get; set; }
        public float? PromoSales56Days { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderBuyPromoCodeLength, ErrorMessage = ErrorMessages.OrderBuyPromoCodeLength)]
        public string BuyPromoCode { get; set; }
        public DateTime? BuyPromoEndDate { get; set; }
        public float? BuyPromoDisc { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderSalePromoCodeLength, ErrorMessage = ErrorMessages.OrderSalePromoCodeLength)]
        public string SalePromoCode { get; set; }

        public DateTime? SalePromoEndDate { get; set; }
        public bool? CheckSuuplier { get; set; }
        public int? CheaperSupplierId { get; set; }
        public bool NewProduct { get; set; }

        //Used for Supplier Item Update only

        public long? Number { get; set; }
        public string Desc { get; set; }

    }
}
