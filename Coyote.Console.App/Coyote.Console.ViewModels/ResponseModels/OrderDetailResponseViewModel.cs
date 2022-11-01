using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OrderDetailResponseViewModel
    {
        public OrderHeaderResponseViewModel OrderHeaders { get; set; } 
        public List<OrderDetailViewModel> OrderDetails { get; set; } 
    }
}
