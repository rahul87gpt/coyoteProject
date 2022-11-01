using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;

namespace Coyote.Console.App.Models
{
    [Table("ManualSaleItem")]
    public class ManualSaleItem : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public long ManualSaleId { get; set; }
        public virtual ManualSale ManualSale { get; set; }
        public int TypeId { get; set; }
        public virtual MasterListItems TypeMasterListItem { get; set; }   
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int OutletId { get; set; }
        public virtual Store Store { get; set; }
        public float Qty { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
        public float Cost { get; set; }
        public string PriceLevel { get; set; }
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
