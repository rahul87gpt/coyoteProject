using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class MassPriceUpdateResponseModel
    {
        public int StoreId { get; set; }
        public long ProductId { get; set; }
        public long ProductNumber { get; set; }
        public float NormalPrice1 { get; set; }
        public float? NormalPrice2 { get; set; }
        public float? NormalPrice3 { get; set; }
        public float? NormalPrice4 { get; set; }
        public float? NormalPrice5 { get; set; }
        public float CartonCost { get; set; }
        public float? CartonCostHost { get; set; }

        public int CartonQty { get; set; }
        public int UnitQty { get; set; }
        public int DepartmentId { get; set; }
        public float ZoneCartonCost { get; set; }
        public float PricingZone { get; set; }
    }
}
