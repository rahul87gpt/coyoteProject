using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class PromotionResponseViewModel : PromotionRequestModel
    {
        public int Id { get; set; }

        public string PromotionDetail { get; set; }
        public string FullName { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public string PromotionType { get; set; }
        public string Zone { get; set; }
        public string ZoneCode { get; set; }
        public string Frequency { get; set; }
        public string FrequencyDesc { get; set; }


        //will  be used in Product?Promotions tab only

        public string Action { get; set; }
        public float? CartonCost { get; set; }
        public float? Price1 { get; set; }
        public float? Price2 { get; set; }
        public float? Price3 { get; set; }
        public float? Price4 { get; set; }
        public string Mixmatch { get; set; }
        public string Offer { get; set; }

        public string DefaultLabelType { get; set; }
        public string DefaultLabelTypeDesc { get; set; }
        public int? Group { get; set; }
    }
}
