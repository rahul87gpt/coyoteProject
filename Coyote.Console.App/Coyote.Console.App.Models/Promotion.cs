using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Promotion")]
    public class Promotion : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionCodeLengthDB, ErrorMessage = ErrorMessages.PromotionCodeLengthDB)]
        [Required]
        public string Code { get; set; }
        public int PromotionTypeId { get; set; }
        public virtual MasterListItems PromotionType { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionDescLength, ErrorMessage = ErrorMessages.PromotionDescLength)]
        [Required]
        public string Desc { get; set; }
        public bool Status { get; set; }
        public int SourceId { get; set; }
        public virtual MasterListItems PromotionSource { get; set; }
        public int ZoneId { get; set; }
        public virtual MasterListItems PromotionZone { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionRptGroupLength, ErrorMessage = ErrorMessages.PromotionRptGroupLength)]
        public string RptGroup { get; set; }
        public int? FrequencyId { get; set; }
        public virtual MasterListItems PromotionFrequency { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionAvailibilityLength, ErrorMessage = ErrorMessages.PromotionAvailibilityLength)]
        public string Availibility { get; set; }
        public string ImagePath { get; set; }

        public virtual ICollection<PromotionBuying> PromotionBuying { get; }
        public virtual ICollection<PromotionMemberOffer> PromotionMemberOffer { get; }
        public virtual ICollection<PromotionMixmatch> PromotionMixmatch { get; }
        public virtual ICollection<PromotionOffer> PromotionOffer { get; }
        public virtual ICollection<PromotionSelling> PromotionSelling { get; }
        public virtual CompetitionDetail CompetitionDetails { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }

        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public virtual ICollection<JournalDetail> JournalDetailSellPromo { get; }
        public virtual ICollection<JournalDetail> JournalDetailMixMatchPromo { get; }
        public virtual ICollection<JournalDetail> JournalDetailOfferPromo { get; }
        public virtual ICollection<JournalDetail> JournalDetailCompPromo { get; }
        public virtual ICollection<JournalDetail> JournalDetailMemberOfferPromo { get; }

        public virtual ICollection<Transaction> TransactionPromoBuys { get; }
        public virtual ICollection<Transaction> TransactionPromoSells { get; }
        public virtual ICollection<TransactionReport> TransactionReportPromoBuys { get; }
        public virtual ICollection<TransactionReport> TransactionReportPromoSells { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionPromoBuys { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionPromoSells { get; }

        public virtual ICollection<OutletProduct> OutletProductPromotionMixMatch1 { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionMixMatch2 { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionOffer1 { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionOffer2 { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionOffer3 { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionOffer4 { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionSell { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionBuy { get; }
        public virtual ICollection<OutletProduct> OutletProductPromotionMember { get; }
        public virtual ICollection<HostUpdChange> HostUpdChangePromotion { get; }
    }
}
