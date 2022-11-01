using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class AutoOrderDefaultSettingsResponseViewModel
    {
        public int SupplierId { get; set; }
        public int HistoryDays { get; set; }
        public float CoverDays { get; set; }
        public int InvestmentBuyDays { get; set; }
    }
}
