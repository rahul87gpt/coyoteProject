using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class HostUpdChgRequestModels
    {
        public string Action { get; set; }
        public string Desc { get; set; }
        public long HostSettingId { get; set; }
        public long Number { get; set; }
        public float BeforeCtnQty { get; set; }
        public float AfterCtnQty { get; set; }
        public float BeforeCtnCost { get; set; }
        public float AfterCtnCost { get; set; }
        public long OutletId { get; set; }
        public float OldPriceSell { get; set; }
        public float NewPriceSell { get; set; }
    }
}
