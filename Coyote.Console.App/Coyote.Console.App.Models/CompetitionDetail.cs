using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("CompetitionDetail")]
    public class CompetitionDetail : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }

        public int PromotionId { get; set; }
        public virtual Promotion Promotion { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxCompetitionDetailCodeLength, ErrorMessage = ErrorMessages.CompetitionDetailCodeLength)]
        public string Code { get; set; }
         [Required]
        [MaxLength(MaxLengthConstants.MaxCompetitionDetailDescLength, ErrorMessage = ErrorMessages.CompetitionDetailDescLength)]
        public string Desc { get; set; }

        public int ZoneId { get; set; }
        public virtual MasterListItems CompetitionZone { get; set; }
        public int TypeId { get; set; }
        public virtual MasterListItems CompetitionType { get; set; }
        public int ResetCycleId { get; set; }
        public virtual MasterListItems CompetitionResetCycle { get; set; }
        public float LoyaltyFactor { get; set; }
        public float? ComplDiscount { get; set; }
        public bool Status { get; set; }
        public string ImagePath { get; set; }
        public bool PosReceiptPrint { get; set; }
        [MaxLength(MaxLengthConstants.MaxCompetitionDetailMessageLength, ErrorMessage = ErrorMessages.CompetitionDetailMessageLength)]
        public string Message { get; set; }

        //Reward settings Header
        public int RewardTypeId { get; set; }
        public virtual MasterListItems RewardType { get; set; }
        public float? Discount { get; set; }
        public int? RewardExpiration { get; set; }
        public bool ResetCycle { get; set; } = false;

        public bool ForcePrint { get; set; } = false;
        //trigger Settings Header
        public int TriggerTypeId { get; set; }
        public virtual MasterListItems TriggerType { get; set; }
        public float? ActivationPoints { get; set; }
        public float? RewardThreshold { get; set; }


        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<PromotionCompetition> PromotionCompetitions { get; set; }

        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public int TriggerCount { get; set; }
        public virtual ICollection<OutletProduct> OutletProductPromotionCompetition { get; }
    }
}
