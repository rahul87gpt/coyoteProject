using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class ProductHistoryFilter : PagedInputModel
    {
        public int? ProductId { get; set; }
        public int? StoreId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ModuleName { get; set; }
    }
}
