using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class RebateResponseModel: RebateHeaderResponseModel
    {
        public List<int> RebateOutletsList { get; set; } = new List<int>();
        public List<RebateDetailResponseModel> RebateDetailsList { get; set; } = new List<RebateDetailResponseModel>();
    }

    public class RebateDetailResponseModel : RebateDetailsRequestModel
    {
        public string ProductDesc { get; set; }
        public long Number { get; set; }
        public int CartonQty { get; set; }
    }

    public class RebateOutletResponseModel 
    {
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
    }
}
