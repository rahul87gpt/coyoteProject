namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderTabletLoadRequestModel
    {
        public int OutletId { get; set; }
        public int? SupplierId { get; set; }
        public long OrderNo { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
    }
}
