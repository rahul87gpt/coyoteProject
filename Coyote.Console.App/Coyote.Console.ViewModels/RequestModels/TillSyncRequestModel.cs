using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class TillSyncRequestModel
    {
        public bool Product { get; set; }
        public bool Cashier { get; set; }
        public bool Keypad { get; set; }
        public bool Account { get; set; }
        public bool RemoveSync { get; set; }
        [Required]
        public List<int> StoreIds { get; set; } = new List<int>();
    }

}
