using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("JournalDetail")]
    public class JournalDetail : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long JournalHeaderId { get; set; }
        public virtual JournalHeader JournalHeader { get; set; }
        [Required]
        public int Sequence { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxJournalTypeLength, ErrorMessage = ErrorMessages.JournalHeaderTypeLength)]
        public string Type { get; set; }
        public bool Status { get; set; }
        public long? ProductId { get; set; }
        public long? ProductNumber { get; set; }
        public virtual Product Product { get; set; }
        [MaxLength(MaxLengthConstants.MaxJournalDetailProductDesc, ErrorMessage = ErrorMessages.JournalDetailProductDescLength)]
        public float Quantity { get; set; }
        public float Amount { get; set; }
        public float DiscountAmount { get; set; }
        public float GSTAmount { get; set; }
        public float Cost { get; set; }
        public float GSTCost { get; set; }
        public int CashierId { get; set; }
        public long? CashierNumber { get; set; }
        public virtual Cashier Cashier { get; set; }
        public int PriceLevel { get; set; }
        //As discussed add 5 different columns for Promotions //will remove unused columns in future
        public int? PromoSellId { get; set; }

        [MaxLength(MaxLengthConstants.MaxPromotionCodeLengthDB, ErrorMessage = ErrorMessages.PromotionCodeLengthDB)]
        public string PromoSellCode { get; set; }
        public virtual Promotion PromotionSell { get; set; }
        public int? PromoMixMatchId { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionCodeLengthDB, ErrorMessage = ErrorMessages.PromotionCodeLengthDB)]
        public string PromoMixCode { get; set; }
        public virtual Promotion PromotionMixMatch { get; set; }
        public int? PromoOfferId { get; set; }

        [MaxLength(MaxLengthConstants.MaxPromotionCodeLengthDB, ErrorMessage = ErrorMessages.PromotionCodeLengthDB)]
        public string PromoOfferCode { get; set; }
        public virtual Promotion PromotionOffer { get; set; }
        public int? PromoCompId { get; set; }

        [MaxLength(MaxLengthConstants.MaxPromotionCodeLengthDB, ErrorMessage = ErrorMessages.PromotionCodeLengthDB)]
        public string PromoCompCode { get; set; }
        public virtual Promotion PromotionCompetition { get; set; }
        public int? PromoMemeberOfferId { get; set; }

        [MaxLength(MaxLengthConstants.MaxPromotionCodeLengthDB, ErrorMessage = ErrorMessages.PromotionCodeLengthDB)]
        public string PromoMemberOfferCode { get; set; }
        public virtual Promotion PromotionMemberOffer { get; set; }
        public bool PostStatus { get; set; }

        public long? APNNumber { get; set; }
        public long? APNSold { get; set; }
        public virtual APN APN { get; set; }
        //START these columns can be changed //Manoj sir
        public string LoyaltyInfo { get; set; }
        [MaxLength(MaxLengthConstants.MaxJournalDetailReferenceType,ErrorMessage=ErrorMessages.JournalDetailReferenceType)]
        public string ReferenceType { get; set; }
        [MaxLength(MaxLengthConstants.MaxJournalDetailReference, ErrorMessage = ErrorMessages.JournalDetailReference)]
        public string Reference { get; set; }
        [MaxLength(MaxLengthConstants.MaxJournalDetailTermRebateCode, ErrorMessage = ErrorMessages.JournalDetailTermRebateCode)]
        public string TermsRebateCode { get; set; }
        public float? TermsRebate { get; set; }
        [MaxLength(MaxLengthConstants.MaxJournalDetailScanRebateCode,ErrorMessage =ErrorMessages.JournalDetailScanRebateCode)]
        public string PromoScanRebateCode { get; set; }
        public float? PromoScanRebate { get; set; }
        [MaxLength(MaxLengthConstants.MaxJournalDetailPurchaseRebateCode,ErrorMessage =ErrorMessages.JournalDetailPurchaseRebateCode)]
        public string PromoPurchaseRebateCode { get; set; }
        public float? PromoPurchaseRebate { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users JournalDetailCreatedBy { get; set; }
        public virtual Users JournalDetailUpdatedBy { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxJournalDetailDesc, ErrorMessage = ErrorMessages.JournalHeaderTypeLength)]
        public string Desc { get; set; }
        public int? AccountNumber { get; set; }

        public int? JournalDetailDate { get; set; }
        public int? JournalDetailTime { get; set; }


        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress1)]
        public string OutletCode { get; set; }
        [MaxLength(MaxLengthConstants.MaxTillCodeLength, ErrorMessage = ErrorMessages.TillCodeLength)]
        public string TillCode { get; set; }
        public int? TransactionNo { get; set; }
    }
}
