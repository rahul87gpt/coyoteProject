using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class SupplierResponseViewModel : SupplierRequestModel
    {
        public int Id { get; set; }
         
        public DateTime CreatedAt { get; set; }
         
        public DateTime UpdatedAt { get; set; }
         
        public int CreatedById { get; set; }
         
        public int UpdatedById { get; set; }
    }
}
