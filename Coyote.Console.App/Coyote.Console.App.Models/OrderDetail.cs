using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;

namespace Coyote.Console.App.Models
{
    [Table("OrderDetail")]
    public class OrderDetail : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public long OrderHeaderId { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
        public int OrderTypeId { get; set; }
        public virtual MasterListItems OrderType { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength, ErrorMessage = ErrorMessages.ListItemCode)]
        public string TypeCode { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }

        public long? ProductNumber { get; set; }
        public long? SupplierProductId { get; set; }
        public virtual SupplierProduct SupplierProduct { get; set; }

        [MaxLength(MaxLengthConstants.MaxSupplierItemCode, ErrorMessage = ErrorMessages.SupplierProductItemCodeLength)]
        public string SupplierItem { get; set; }
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
        [Column(TypeName = "datetime")]
        public System.DateTime? PostedDate { get; set; }
        public float? FinalLineTotal { get; set; }
        public float? FinalCartonCost { get; set; }
        public float? OnHand { get; set; }
        public float? OnOrder { get; set; }
        public float? UnitsSold { get; set; }
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
        [Column(TypeName = "datetime")]
        public System.DateTime? BuyPromoEndDate { get; set; }
        public float? BuyPromoDisc { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderSalePromoCodeLength, ErrorMessage = ErrorMessages.OrderSalePromoCodeLength)]
        public string SalePromoCode { get; set; }
        public System.DateTime? SalePromoEndDate { get; set; }
        public bool? CheckSuuplier { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderBuyPromoCodeLength, ErrorMessage = ErrorMessages.OrderBuyPromoCodeLength)]
        public int? CheaperSupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [MaxLength(MaxLengthConstants.SuppCodeLength, ErrorMessage = ErrorMessages.SuppCode)]
        public string SupplierCode { get; set; }
        public bool NewProduct { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }


        public bool? PromoBuy { get; set; }
        public bool? NonPromoBuy { get; set; }
        public bool? InvestBuy { get; set; }

        public string StoreCode { get; set; }
        public string CheaperSupplierCode { get; set; }

        [Column(TypeName = "datetime")]
        public System.DateTime? OrderCreatedAt { get; set; }

        public long? OrderNo { get; set; }

        public int? TradingCoverDays { get; set; }

        public string Remarks { get; set; }
        public float? FinalGSTAMT { get; set; }
        public bool? GSTInd { get; set; }

    }
}




