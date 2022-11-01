using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OrderRequestModel
    {

        public int OutletId { get; set; } 

        [Required]
        public long OrderNo { get; set; }

        public int? SupplierId { get; set; }
        public int? StoreIdAsSupplier { get; set; }
        public int CreationTypeId { get; set; } 

        public int TypeId { get; set; } 

        public int StatusId { get; set; } 
         
        public DateTime CreatedDate { get; set; } 

        public DateTime? PostedDate { get; set; }

        [MaxLength(MaxLengthConstants.MaxOrderRefLength, ErrorMessage = ErrorMessages.OrderRefLength)]
        public string Reference { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderDeliveryNoLength, ErrorMessage = ErrorMessages.OrderDeliveryNoLength)]
        public string DeliveryNo { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(MaxLengthConstants.MaxOrderInvoiceNoLength, ErrorMessage = ErrorMessages.OrderInvoiceNoLength)]
        public string InvoiceNo { get; set; }
         
        public DateTime? InvoiceDate { get; set; }
        public float? InvoiceTotal { get; set; }
        public float? SubTotalFreight { get; set; }
        public float? SubTotalAdmin { get; set; }
        public float? SubTotalSubsidy { get; set; }
        public float? SubTotalDisc { get; set; }
        public float? SubTotalTax { get; set; }  

        public DateTime? Posted { get; set; }
        public float? GstAmt { get; set; }
        public int? CoverDays { get; set; }


        public List<OrderDetailRequestModel> OrderDetails { get; set; } 
    }
}
