using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class MassPriceUpdateRequestModel
    {
        public int StoreId { get; set; }
        public int DepartmentId { get; set; }
        public int? CommodityId { get; set; }
        public string PriceZone { get; set; }
        public bool OnlyHostCodes { get; set; }
        public float ChangeCost { get; set; }
        public float ChaneSellGP { get; set; }
        public bool RoundSellPrice { get; set; }
        [Required]
        public string OutletPassword { get; set; }
        [Required]
        public string SystemPassword { get; set; }
    }

    public class MassPriceUpdateResultModel
    {
        public string UpdatedRec { get; set; }
    }
}
