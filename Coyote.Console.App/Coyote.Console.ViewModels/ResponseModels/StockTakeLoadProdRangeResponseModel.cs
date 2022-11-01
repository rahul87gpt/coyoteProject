using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockTakeLoadProdRangeResponseModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long? Number { get; set; }
        public string ProductDesc { get; set; }
        public float? UnitOnHand { get; set; } = 0;
        public float? UnitCost { get; set; } = 0;
        public float? VarUnit { get; set; } = 0;
        public float? VarCost { get; set; } = 0;
        public float? UnitsCount { get; set; } = 0;
        public float ItemCount { get; set; } = 0;
        public float LineCost { get; set; } = 0;
        public float LineTotal { get; set; } = 0;       
    }
}
