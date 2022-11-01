using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockTRXSheetResponseViewModel
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public long Product { get; set; }
        public string Description { get; set; }
        public string Outlet { get; set; }
        public string Till { get; set; }
        public int QTY { get; set; }
        public float Cost { get; set; }
        public string SubType { get; set; }
        public DateTime WeekEnding { get; set; }
        public float ExGstCost { get; set; }
        public string Supplier { get; set; }
        public string Commodity { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Group { get; set; }
        public string Member { get; set; }
        public bool Manual { get; set; }
        public float SellUnitQTY { get; set; }
        public float StockMovement { get; set; }
        public int Parent { get; set; }
        public float CtnQTY { get; set; }
    }
}
