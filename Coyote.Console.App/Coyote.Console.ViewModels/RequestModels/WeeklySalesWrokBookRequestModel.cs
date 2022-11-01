using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class WeeklySalesWrokBookRequestModel
    {
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
        public string StoreIds { get; set; }
    }
}
