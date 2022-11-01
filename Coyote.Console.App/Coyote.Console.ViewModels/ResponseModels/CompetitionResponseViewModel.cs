using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class CompetitionResponseViewModel : CompetitionRequestViewModel
    {
        public long Id { get; set; }
        public int PromotionId { get; set; }
        public string ImagePath { get; set; }

        //Promotion table
        public string SourceCode { get; set; }
        public string SourceDesc { get; set; }

        public string FrequencyCode { get; set; }
        public string FrequencyDesc { get; set; }

        //competition table
        public string ZoneCode { get; set; }
        public string ZoneDesc { get; set; }

        public string CompetitionTypeCode { get; set; }
        public string CompetitionTypeDesc { get; set; }

        public string ResetCycleCode { get; set; }
        public string ResetCycleDesc { get; set; }

        public string RewardTypeCode { get; set; }
        public string RewardTypeDesc { get; set; }

        public string TriggerTypeCode { get; set; }
        public string TriggerTypeDesc { get; set; }

        public int TriggerCount { get; set; }
        public List<CompTriggerResponseViewModel> CompetitionTriggerResponse { get; set; }

        public List<CompRewardResponseViewModel> CompetitionRewardResponse { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
