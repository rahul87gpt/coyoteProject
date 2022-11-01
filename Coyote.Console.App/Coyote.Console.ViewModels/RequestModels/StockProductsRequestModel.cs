using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockProductsRequestModel : ReportRequest
    {
       
        public bool SetNegativeOnHandAsZero { get; set; }
        
        //paging
        //public int? MaxResultCount { get; set; } 
        //public int? SkipCount { get; set; } = 0;

    }
}
