using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("AccountLoyalty")]
    public class AccountLoyalty
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxAccountLoyaltyAccountNumberLength, ErrorMessage = ErrorMessages.AccountLoyaltyAccountNumberLength)]
        public int AccountNumber { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxAccountLoyaltyCompLength, ErrorMessage = ErrorMessages.MaxAccountLoyaltyCompLength)]
        public string Comp { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CycleStart { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CycleEnd { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountLoyaltyCompLength, ErrorMessage = ErrorMessages.MaxAccountLoyaltyStatusLength)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastTransactionDate { get; set; }
        public int? TriggerType { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountLoyaltyComplianceLength, ErrorMessage = ErrorMessages.AccountLoyaltyComplianceLength)]
        public float? TriggCompliance { get; set; }
        public int? RewardType { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountLoyaltyCompLength, ErrorMessage = ErrorMessages.AccountLoyaltyRewardInfoLength)]
        public string RewardInfo { get; set; }
        public DateTime? RewardDate { get; set; }
        public float? LoyalityPoints { get; set; }
        [MaxLength(MaxLengthConstants.MaxAccountLoyaltyCompLength, ErrorMessage = ErrorMessages.AccountLoyaltyReedeemInfoLength)]
        public string RedeemInfo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RedeemDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RedeemExp { get; set; }
        public int? RedeemCount { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users AccountLoyaltyCreatedBy { get; set; }
        public virtual Users AccountLoyaltyUpdatedBy { get; set; }
    }
}
