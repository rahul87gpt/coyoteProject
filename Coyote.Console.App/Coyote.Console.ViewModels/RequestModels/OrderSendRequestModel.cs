namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderSendRequestModel
    {
        public int OutletId { get; set; }
        public int? SupplierId { get; set; }
        public int TypeId { get; set; }
        public long OrderNo { get; set; }
        public bool Offline { get; set; }
        public string DeliveryInstructions { get; set; }
    }
}
