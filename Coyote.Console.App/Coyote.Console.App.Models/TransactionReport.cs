using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("TransactionReport")]
    public class TransactionReport : IAuditable<int>, IKeyIdentifier<long>
    {
        public long Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxTransactionTypeLength, ErrorMessage = ErrorMessages.TransactionTypeLength)]
        public string Type { get; set; }
        [Required]
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        [Required]
        public int OutletId { get; set; }
        public virtual Store Store { get; set; }
       
        public int? TillId { get; set; }
        public virtual Till Till { get; set; }
        [Required]
        public int Sequence { get; set; }


        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public int? ManufacturerId { get; set; }
        public virtual MasterListItems Manufacturer { get; set; }

        public int? Group { get; set; }
        //TODO: Department can't be null //add default id
        [Required]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int? CommodityId { get; set; }
        public virtual Commodity Commodity { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual MasterListItems Category { get; set; }

        //TODO: FK relation to be checked by Manoj sir 
        public int? SubRange { get; set; }

        [MaxLength(MaxLengthConstants.MaxTransactionReferenceLength, ErrorMessage = ErrorMessages.TransactionReferenceLength)]
        public string Reference { get; set; }

        // Null as discussed with Yogesh sir
        public int UserId { get; set; }
        public virtual Users User { get; set; }
        public float? Qty { get; set; }
        public float? Amt { get; set; }
        public float? ExGSTAmt { get; set; }
        public float? Cost { get; set; }
        public float? ExGSTCost { get; set; }
        public float? Discount { get; set; }
        public float? Price { get; set; }
        public int? PromoBuyId { get; set; }
        public virtual Promotion PromotionBuy { get; set; }
        public int? PromoSellId { get; set; }
        public virtual Promotion PromotionSell { get; set; }
        [Required]
        public DateTime Weekend { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxTransactionDay, ErrorMessage = ErrorMessages.TransactionDayLength)]
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
        public TransactionReport TrxReportParent { get; set; }
        public float? StockMovement { get; set; }

        //TODO: FK relation to be checked
        [MaxLength(MaxLengthConstants.MaxTransactionTenderLength, ErrorMessage = ErrorMessages.TransactionTenderLength)]
        public string Tender { get; set; }
        public bool? ManualInd { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionGLAccountLength, ErrorMessage = ErrorMessages.TransactionGLAccountLength)]
        public string GLAccount { get; set; }
        public bool? GLPostedInd { get; set; }
        public float? PromoSales { get; set; }
        public float? PromoSalesGST { get; set; }

        //TODO: FK relation to be checked
        public float? Debtor { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionFlags, ErrorMessage = ErrorMessages.TransactionFlagsLength)]
        public string Flags { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionReferenceType, ErrorMessage = ErrorMessages.TransactionReferenceType)]
        public string ReferenceType { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionReferenceNumber, ErrorMessage = ErrorMessages.TransactionReferenceNumber)]
        public string ReferenceNumber { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionTermsRebateCode, ErrorMessage = ErrorMessages.TransactionTermsRebateCode)]
        public string TermsRebateCode { get; set; }
        public float? TermsRebate { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionScanRebateCode, ErrorMessage = ErrorMessages.TransactionScanRebateCode)]
        public string ScanRebateCode { get; set; }
        public float? ScanRebate { get; set; }
        [MaxLength(MaxLengthConstants.MaxTransactionPurchaseRebateCode, ErrorMessage = ErrorMessages.TransactionPurchaseRebateCode)]
        public string PurchaseRebateCode { get; set; }
        public float? PurchaseRebate { get; set; }


        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users TransactionReportCreatedBy { get; set; }
        public virtual Users TransactionReportUpdatedBy { get; set; }
    }
}
