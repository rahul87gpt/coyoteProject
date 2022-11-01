using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class HOSTUPDUserActivityResponseModel
    {
        public PagedOutputModel<List<UserLogResponseModel<HOSTUPDDetails>>> HostUPD { get; set; }       
    }

    public class HOSTUPDDetails
    {
        public long Id { get; set; }
        public long ProductNumber { get; set; }
        public long OutLetId { get; set; }
        public string ProductDesc { get; set; }
        public string ShortDesc { get; set; }
        public string APNNUMBE { get; set; }
        public long HostSettingId { get; set; }
        public string HostCode { get; set; }
        public float BeforeCtnQty { get; set; }
        public float AfterCtnQty { get; set; }
        public float BeforeCtnCost { get; set; }
        public float AfterCtnCost { get; set; }
        public float OldPriceSell { get; set; }
        public float NewPriceSell { get; set; }
      
    }
}
