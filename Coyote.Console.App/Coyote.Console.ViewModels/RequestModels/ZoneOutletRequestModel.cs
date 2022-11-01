using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class ZoneOutletRequestModel
    {
        [Required]
        public string StoreIds { get; set; }

        [Required]
        public int ZoneId { get; set; }
    }
}
