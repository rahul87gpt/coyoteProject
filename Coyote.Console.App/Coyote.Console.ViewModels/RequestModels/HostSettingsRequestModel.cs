using System;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class HostSettingsRequestModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string InitialLoadFileWeekly { get; set; }
        [Required]
        public string WeeklyFile { get; set; }
        [Required]
        public int FilePathID { get; set; }
        [Required]
        public float NumberFactor { get; set; }
        public int? SupplierID { get; set; }
        [Required]
        public int WareHouseID { get; set; }
        [Required]
        public int HostFormatID { get; set; }
        [Required]
        public string BuyPromoPrefix { get; set; }
        [Required]
        public string SellPromoPrefix { get; set; }
    }
}
