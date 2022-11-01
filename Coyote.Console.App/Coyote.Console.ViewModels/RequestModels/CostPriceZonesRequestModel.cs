using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class CostPriceZonesRequestModel
    {
        [Required]
        public string Code { get; set; }
        public CostPriceZoneType Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int? HostSettingID { get; set; }
        public float? Factor1 { get; set; }
        public float? Factor2 { get; set; }
        public float? Factor3 { get; set; }
        [Required]
        public bool SuspUpdOutlet { get; set; }
    }
}
