namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderFinishRequestModel
    {
        public int OutletId { get; set; }
        public long OrderNo { get; set; }
        public int? SupplierId { get; set; }
        public int TypeId { get; set; } 
        public int StatusId { get; set; }
        public string ReferenceNumber { get; set; }
        public bool IsDelete { get; set; }
    }
}
