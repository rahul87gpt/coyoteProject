using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class PromotionDetailResponseViewModel
    {
        public PromotionResponseViewModel Promotion { get; set; }
        public PromotionMixmatchViewModel PromotionMixmatch { get; set; } 
        public PromotionOfferViewModel PromotionOffer { get; set; }
        public PromotionCompetitionViewModel  PromotionCompetition { get; set; }

    }

    
}
