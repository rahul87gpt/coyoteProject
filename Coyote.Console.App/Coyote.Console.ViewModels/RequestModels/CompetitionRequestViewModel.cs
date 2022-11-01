using Coyote.Console.Common;
using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class CompetitionRequestViewModel
    {
        //Promotion and Competition Detials Data

        [Required]
        [MaxLength(MaxLengthConstants.MaxCompetitionDetailCodeLength, ErrorMessage = ErrorMessages.CompetitionDetailCodeLength)]
        public string Code { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxCompetitionDetailDescLength, ErrorMessage = ErrorMessages.CompetitionDetailDescLength)]
        public string Desc { get; set; }

        // Promotion Data
        public int PromotionTypeId { get; set; }
        public int SourceId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int FrequencyId { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionAvailibilityLength, ErrorMessage = ErrorMessages.PromotionAvailibilityLength)]
        public string Availibility { get; set; }

        //promotion and Competition Details Data
        public int ZoneId { get; set; }
        public int TypeId { get; set; }
        public int ResetCycleId { get; set; }
        public float LoyaltyFactor { get; set; }
        public float? ComplDiscount { get; set; }
        public bool Status { get; set; }
        public bool PosReceiptPrint { get; set; }
        [MaxLength(MaxLengthConstants.MaxCompetitionDetailMessageLength, ErrorMessage = ErrorMessages.CompetitionDetailMessageLength)]
        public string Message { get; set; }
        public bool ForcePrint { get; set; } = false;

        //Reward settings Header
        public int RewardTypeId { get; set; }
        public float? Discount { get; set; }
        public int? RewardExpiration { get; set; }
        public bool ResetCycle { get; set; } = false;
        public byte[] Image { get; set; }
        public string ImageName { get; set; }


        //trigger Settings Header
        public int TriggerTypeId { get; set; }
        public float? ActivationPoints { get; set; }
        public float? RewardThreshold { get; set; }

        //List of Trigger Products
        public virtual ICollection<CompTriggerRequestViewModel> TriggerProds { get; set; }

        //List of Reward Products
        public virtual ICollection<CompRewardRequestViewModel> RewardProds { get; set; }

    }
}
