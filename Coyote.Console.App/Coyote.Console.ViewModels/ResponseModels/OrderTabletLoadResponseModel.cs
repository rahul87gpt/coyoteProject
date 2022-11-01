using System;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OrderTabletLoadResponseModel
    {
        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public DateTime OrderBatch { get; set; }
        public DateTime LastImport { get; set; }
    }
}
