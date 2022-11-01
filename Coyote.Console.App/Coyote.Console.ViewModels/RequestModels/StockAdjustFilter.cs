using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public  class StockAdjustFilter : PagedInputModel
    {
        public string StoreId { get; set; }
    }
}
