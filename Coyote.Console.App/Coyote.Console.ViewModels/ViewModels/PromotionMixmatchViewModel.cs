using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class PromotionMixmatchViewModel : PromotionMixmatchRequestModel
    {
       // public int Id { get; set; }
        public int PromotionId { get; set; }   

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
