using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("SupplierProduct")]
    public class SupplierProduct : IKeyIdentifier<long>, IAuditable<int>
    {
        [Key]
        public long Id { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        [MaxLength(MaxLengthConstants.MaxSupplierItemCode, ErrorMessage =ErrorMessages.SupplierProductItemCodeLength)]
        public string SupplierItem { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        [MaxLength(MaxLengthConstants.MinSupplierProductDescLength, ErrorMessage = ErrorMessages.SupplierProductDescLength)]
        public string Desc { get; set; }
        public bool Status { get; set; }
        public float CartonCost { get; set; }
        public int? MinReorderQty { get; set; }
        public bool? BestBuy { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual ICollection<OrderDetail> OrderDetailSupplierItem { get; }
    }
}
