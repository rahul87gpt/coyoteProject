using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockTakeHeaderResponseViewModel //: StockTakeHeaderRequestViewModel
    {
        public long Id { get; set; }

        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
     
        public DateTime? PostToDate { get; set; }
        public string Desc { get; set; }
        public float Total { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<StockTakeDetailResponseModel> StockDetail { get; } = new List<StockTakeDetailResponseModel>();
    }
}
