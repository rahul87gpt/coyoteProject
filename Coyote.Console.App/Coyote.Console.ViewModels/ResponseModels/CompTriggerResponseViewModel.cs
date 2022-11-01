using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class CompTriggerResponseViewModel : CompTriggerRequestViewModel
    {
        public long Id { get; set; }

        //promo comp
        public int CompPromoId { get; set; }
         
        public long ProductNumber { get; set; }

        public string ProductGroupCode { get; set; }
        public string ProductGroupDesc { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }

    }
}
