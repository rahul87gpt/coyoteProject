
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("OrderHeader")]
    public class OrderHeader : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }

        public int OutletId { get; set; }


        public virtual Store Store { get; set; }


        [MaxLength(MaxLengthConstants.StoreCodeLength)]
        public string StoreCode { get; set; }

        public long OrderNo { get; set; }

        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [MaxLength(MaxLengthConstants.SuppCodeLength)]
        public string SupplierCode { get; set; }

        public int? StoreIdAsSupplier { get; set; }
        public virtual Store StoreAsSupplier { get; set; }

        public int CreationTypeId { get; set; }
        public virtual MasterListItems CreationType { get; set; }

        [MaxLength(MaxLengthConstants.MinListItemCodeLength, ErrorMessage = ErrorMessages.ListItemCode)]
        public string CreationTypeCode { get; set; }

        public int TypeId { get; set; }
        public virtual MasterListItems Type { get; set; }

        [MaxLength(MaxLengthConstants.MinListItemCodeLength, ErrorMessage = ErrorMessages.ListItemCode)]
        public string TypeCode { get; set; }

        public int StatusId { get; set; }
        public virtual MasterListItems Status { get; set; }

        [MaxLength(MaxLengthConstants.MinListItemCodeLength, ErrorMessage = ErrorMessages.ListItemCode)]
        public string StatusCode { get; set; }

        [Column(TypeName = "datetime")]
        public System.DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]

        public System.DateTime? PostedDate { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderRefLength, ErrorMessage = ErrorMessages.OrderRefLength)]
        public String Reference { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderDeliveryNoLength, ErrorMessage = ErrorMessages.OrderDeliveryNoLength)]
        public String DeliveryNo { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime? DeliveryDate { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderInvoiceNoLength, ErrorMessage = ErrorMessages.OrderInvoiceNoLength)]
        public String InvoiceNo { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime? InvoiceDate { get; set; }
        public float? InvoiceTotal { get; set; }
        public float? SubTotalFreight { get; set; }
        public float? SubTotalAdmin { get; set; }
        public float? SubTotalSubsidy { get; set; }
        public float? SubTotalDisc { get; set; }
        public float? SubTotalTax { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime? Posted { get; set; }
        public float? GstAmt { get; set; }
        public int? CoverDays { get; set; }

       

        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public virtual ICollection<OrderDetail> OrderDetail { get; set; }

    }
}



