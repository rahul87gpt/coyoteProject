using System;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class APNFilter : PagedInputModel
    {
        public string Number { get; set; }
        public string ProductId { get; set; }
        public DateTime? SoldDateFrom { get; set; }
        public DateTime? SoldDateTo { get; set; }
    }
}
