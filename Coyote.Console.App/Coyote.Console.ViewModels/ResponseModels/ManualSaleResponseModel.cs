using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class ManualSaleResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
        public float TotalSalesAmt { get; set; }
        public List<ManualSaleItemResponseModel> manualSaleItemResponseModels { get; } = new List<ManualSaleItemResponseModel>();
    }

    public class ManualSaleItemResponseModel
    {
        public long Id { get; set; }
        public float Qty { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
        public float Cost { get; set; }
        public string PriceLevel { get; set; }
        public long ProductId { get; set; }       
        public long ProductNumber { get; set; }
        public string ProductDesc { get; set; }
        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
    }
}
