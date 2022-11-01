using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class AutomaticOrderRequestModel
    {
        public int StoreId { get; set; }
        public int SupplierId { get; set; }
        public int OrderType { get; set; }  //1 for AUTO 2 For Investment Buy
        public int? AltSupplierId { get; set; }
        public bool IngnoreStockLevel { get; set; } = false;
        public bool ExcludePromo { get; set; } = false;
        public int DaysHistory { get; set; }
        public int CoverDays { get; set; }
        public int? DiscountThreshold { get; set; }
        public int? InvestmentBuyDays { get; set; }
        public int? ExistingOrderNo { get; set; }
        public bool MetcashNormal { get; set; } = false;
        public bool MetcashSlow { get; set; } = false;
        public bool MetcashVariety { get; set; } = false;
        public bool CompareDirectSuppliers { get; set; } = false;
        public string DepartmentIds { get; set; }
        public long? ProductId { get; set; }

    }
}
