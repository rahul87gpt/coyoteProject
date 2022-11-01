using System;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderPostRequestModel
    {
        public int OutletId { get; set; }
        public int? SupplierId { get; set; }
        public long OrderNo { get; set; }
        public int TypeId { get; set; }
        public int CreationTypeId { get; set; }
        public int StatusId { get; set; }
        public string ReferenceNumber { get; set; }
        public bool PostOrderNow { get; set; }
        public DateTime TimeStamp { get; set; }
        public string DeliveryNo { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public float? InvoiceTotalAmount { get; set; }
        public float? SubTotal { get; set; }
        public float? LessDisccount { get; set; }
        public float? LessSubsidy { get; set; }
        public float? PlusAdmin { get; set; }
        public float? PlusFreight { get; set; }
        public float? PlusGst { get; set; }
        public float? OrderGSGTotal { get; set; }
    }
}

