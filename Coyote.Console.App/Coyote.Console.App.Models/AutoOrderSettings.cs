using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("AutoOrderSettings")]
    public class AutoOrderSettings : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; } 
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public int HistoryDays { get; set; }
        public float CoverDays { get; set; }
        public int InvestmentBuyDays { get; set; }

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

    }
}
