using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockTakeTabletResponseModel
    {
        public long Id { get; set; }
        public int OutletId { get; set; }
        public long ProductNumber { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public DateTime PrintBatch { get; set; }
        public float Qty { get; set; }
        public DateTime LastImport { get; set; }
    }
}
