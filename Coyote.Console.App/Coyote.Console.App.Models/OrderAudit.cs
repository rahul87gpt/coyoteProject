using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("OrderAudit")]
    public class OrderAudit : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }
        public int? OutletId { get; set; }
        public virtual Store Store { get; set; }
        public long OrderNo { get; set; }
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        [MaxLength(MaxLengthConstants.MaxOrderInvoiceNoLength, ErrorMessage = ErrorMessages.OrderInvoiceNoLength)]
        public String InvoiceNo { get; set; }
        public int? TypeId { get; set; }
        public virtual MasterListItems Type { get; set; }
        public int? StatusId { get; set; }
        public virtual MasterListItems Status { get; set; }
        public int? NewStatusId { get; set; }
        public virtual MasterListItems NewStatus { get; set; }
        [MaxLength(150, ErrorMessage = ErrorMessages.OrderInvoiceNoLength)]
        public String Action { get; set; }
        public bool IsDeleted { get; set; } = false;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int UpdatedById { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
    }
}
