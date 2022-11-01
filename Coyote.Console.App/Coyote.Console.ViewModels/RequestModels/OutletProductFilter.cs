using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class OutletProductFilter : PagedInputModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string StoreId { get; set; }

    }
}
