using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PromotionDetailRequestModel
    {
        [Required]
        public PromotionRequestModel Promotion { get; set; }
        public PromotionOfferRequestModel PromotionOffer { get; set; }
        public PromotionMixmatchRequestModel PromotionMixmatch { get; set; }
    }


    public class PromotionImportRequestModel
    {
        [Required]
        public PromotionRequestModel Promotion { get; set; }
        public PromotionOfferRequestModel PromotionOffer { get; set; }
        public PromotionMixmatchRequestModel PromotionMixmatch { get; set; }

        public byte[] PromoCSV { get; set; }
    }

}
