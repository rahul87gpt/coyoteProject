using System;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PromotionFilter : PagedInputModel
    {
        public DateTime? PromotionStartDate { get; set; }
        public DateTime? PromotionEndDate { get; set; }
        public bool? ExcludePromoBuy { get; set; }
        public string Code { get; set; }
    }

    public class PromotionCloneFilter
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
        public int? OutletId { get; set; }        
        public int? ZoneId { get; set; }
    }
}
