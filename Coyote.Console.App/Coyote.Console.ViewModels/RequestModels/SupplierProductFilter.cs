using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class SupplierProductFilter: PagedInputModel
    {
        public string SupplierId { get; set; }
        public string ProductId { get; set; }
    }
}
