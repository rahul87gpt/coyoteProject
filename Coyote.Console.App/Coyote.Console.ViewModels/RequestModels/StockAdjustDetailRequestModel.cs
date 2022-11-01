using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockAdjustDetailRequestModel
    { 
        public int LineNo { get; set; }
        public long ProductId { get; set; } 
        public long OutletProductId { get; set; } 
        public float Quantity { get; set; }
        public float LineTotal { get; set; }
        public float ItemCost { get; set; }
        public int ReasonId { get; set; } 
    }
}
