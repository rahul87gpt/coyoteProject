using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class DeactivateProductListResponseModel
    {
        public long OutletProductId { get; set; }
        public long Number { get; set; }
        public float QtyOnHand { get; set; }
        public string Desc { get; set; }
        public string Department { get; set; }
    }
}
