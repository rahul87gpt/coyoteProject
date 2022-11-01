using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class OutletRoyaltyScalesResponseModel
    {
        public long Id { get; set; }
        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public RoyaltyScale Type { get; set; }
        public float ScalesFrom { get; set; }
        public float ScalesTo { get; set; }
        public float Percent { get; set; }
        public bool IncGST { get; set; }
    }

    public class SalesRoyaltyAdvertisingResponseModel
    {
        public int OutletId { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public double SalesAmt { get; set; }
        public double SalesAmtExGST { get; set; }
        public double DepartmentRate { get; set; }
        public double Values { get; set; }

        public double GSTAmount { get; set; }
    }

    public class ProductPriceDeviationResponseModel {
        public int OutletId { get; set; }
        public long Number { get; set; }
        public string Description { get; set; }
        public double Selling { get; set; }
        public double GroupSelling { get; set; }
        public double Deviation { get; set; }
    }
}
