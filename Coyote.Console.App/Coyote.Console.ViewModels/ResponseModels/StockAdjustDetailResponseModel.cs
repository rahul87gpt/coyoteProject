using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockAdjustDetailResponseModel : StockAdjustDetailRequestModel
    {
        public long Id { get; set; }
        public long StockAdjustHeaderId { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        //public long ProductNumber { get; set; }
        public long Number { get; set; }
        public string ProductDesc { get; set; }
        public string ReasonName { get; set; }
        public string ReasonCode { get; set; }
        public float UnitOnHand { get; set; }
    }
}
