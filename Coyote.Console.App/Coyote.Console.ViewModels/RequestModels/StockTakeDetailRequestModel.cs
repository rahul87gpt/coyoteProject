namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockTakeDetailRequestModel
    {
        public long StockTakeHeaderId { get; set; }
        public long OutletProductId { get; set; }
        public long ProductId { get; set; }
        public string Desc { get; set; }
        public float OnHandUnits { get; set; }
        public float Quantity { get; set; }
        public float ItemCost { get; set; }
        public float LineCost { get; set; }
        public float LineTotal { get; set; }
        public int ItemCount { get; set; }


        public long Id { get; set; }
    }
}
