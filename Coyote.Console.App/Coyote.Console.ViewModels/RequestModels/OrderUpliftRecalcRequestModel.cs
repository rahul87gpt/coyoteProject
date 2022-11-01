using System;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderUpliftRecalcRequestModel
    {
        public int OutletId { get; set; }
        public int SupplierId { get; set; }
        public long OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public float UpliftFactor { get; set; }
        public string Type { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int CoverDays { get; set; }
    }
}
