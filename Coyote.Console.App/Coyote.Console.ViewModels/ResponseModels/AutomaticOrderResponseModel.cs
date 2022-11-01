using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
   public  class AutomaticOrderResponseModel
    {
        public OrderHeaderResponseViewModel OrderHeaders { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; } = new List<OrderDetailViewModel>();

        public List<InvestmentBuyProducts> IncludedProducts { get; } = new List<InvestmentBuyProducts>();
        public List<InvestmentBuyProducts> ExcludeProducts { get; } = new List<InvestmentBuyProducts>();

        public byte[] SuggestedOrder { get; set; }
    }

    public class InvestmentBuyProducts
    {
        public long? ProductId { get; set; }
        public long? ProductNumber { get; set; }
        public float? PromoCost { get; set; }
        public float? RegularCost { get; set; }
        public float? Discount { get; set; }
        public float? RebateTotal { get; set; }
    }

    public class NewOrderNumber
    { 
    public int OrderCount { get; set; }
    public long OrderNo { get; set; }

    }
}
