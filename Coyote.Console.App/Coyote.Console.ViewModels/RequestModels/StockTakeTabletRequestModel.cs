using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockTakeTabletRequestModel
    {
        [Required]
        public int OutletId { get; set; }
        public long OutletNo { get; set; }     
    }
}
