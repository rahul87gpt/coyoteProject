using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("AccountTransaction")]
    public class AccountTransaction : IAuditable<int>, IKeyIdentifier<long>
    {
        public long Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxAccountTransactionTypeLength, ErrorMessage = ErrorMessages.AccountTransactionTypeLength)]
        public string Type { get; set; }
        public long? ProductId { get; set; }
        public long? ProductNumber { get; set; }
        public virtual Product Product { get; set; }
        public int? OutletId { get; set; }
        public string OutletCode { get; set; }
        public virtual Store Store { get; set; }

        public int? TillId { get; set; }
        public string TillCode { get; set; }
        public virtual Till Till { get; set; }
        [Required]
        public int Sequence { get; set; }


        public int? SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public virtual Supplier Supplier { get; set; }

        public int? ManufacturerId { get; set; }
        public string  ManufacturerCode { get; set; }
        public virtual MasterListItems Manufacturer { get; set; }

        public int? Group { get; set; }
        public string GroupCode { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public virtual Department Department { get; set; }

        public int? CommodityId { get; set; }
        public string CommodityCode { get; set; }
        public virtual Commodity Commodity { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public virtual MasterListItems Category { get; set; }

        //TODO: FK relation to be checked by Manoj sir 
        public int? SubRange { get; set; }
        public string SubRangeCode { get; set; }

        [MaxLength(MaxLengthConstants.MaxAccountTransactionReferenceLength, ErrorMessage = ErrorMessages.AccountTransactionReferenceLength)]
        public string Reference { get; set; }

        // Null as discussed with Yogesh sir
        public int UserId { get; set; }
        public virtual Users User { get; set; }
        public float Qty { get; set; }
        public float Amt { get; set; }
        public float ExGSTAmt { get; set; }
        public float Cost { get; set; }
        public float ExGSTCost { get; set; }
        public float Discount { get; set; }
        public float? Price { get; set; }
        public int? PromoBuyId { get; set; }
        public string PromoBuyCode { get; set; }
        public virtual Promotion PromotionBuy { get; set; }
        public int? PromoSellId { get; set; }
        public string PromoSellCode { get; set; }
        public virtual Promotion PromotionSell { get; set; }
        [Required]
        public DateTime Weekend { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxAccountTransactionDay, ErrorMessage = ErrorMessages.AccountTransactionDayLength)]
        public string Day { get; set; }
        public float? NewOnHand { get; set; }

        //TODO: FK relation to be checked
        public long? Member { get; set; }
        public float? Points { get; set; }
        public float? CartonQty { get; set; }
        public float? UnitQty { get; set; }
        //to be discussed
        public float? Parent { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public AccountTransaction AccountTrxParent { get; set; }

        public float? StockMovement { get; set; }

        //TODO: FK relation to be checked
        [MaxLength(MaxLengthConstants.MaxAccountTransactionTenderLength, ErrorMessage = ErrorMessages.AccountTransactionTenderLength)]
        public string Tender { get; set; }
        public bool? ManualInd { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionGLAccountLength, ErrorMessage = ErrorMessages.AccountTransactionGLAccountLength)]
        public string GLAccount { get; set; }
        public bool? GLPostedInd { get; set; }
        public float PromoSales { get; set; }
        public float PromoSalesGST { get; set; }

        //TODO: FK relation to be checked
        public float Debtor { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionFlags, ErrorMessage = ErrorMessages.AccountTransactionFlagsLength)]
        public string Flags { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionReferenceType, ErrorMessage = ErrorMessages.AccountTransactionReferenceType)]
        public string ReferenceType { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionReferenceNumber, ErrorMessage = ErrorMessages.AccountTransactionReferenceNumber)]
        public string ReferenceNumber { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionTermsRebateCode, ErrorMessage = ErrorMessages.AccountTransactionTermsRebateCode)]
        public string Comp { get; set; }
        public float? TriggCompliance { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionScanRebateCode, ErrorMessage = ErrorMessages.AccountTransactionScanRebateCode)]
        public string RedeemInfo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RedeemDate { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountTransactionPurchaseRebateCode, ErrorMessage = ErrorMessages.AccountTransactionPurchaseRebateCode)]
        public string RewardInfo { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users AccountTransactionCreatedBy { get; set; }
        public virtual Users AccountTransactionUpdatedBy { get; set; }

        public int? AccountNumber { get; set; }
        public string TransactionNumber { get; set; }

    }
}
