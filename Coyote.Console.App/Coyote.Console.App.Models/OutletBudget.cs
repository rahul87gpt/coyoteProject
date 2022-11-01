using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("OutletBudget")]
    public class OutletBudget : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }

        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        public float? Budget { get; set; }
        public float? EmployHours { get; set; }
        public float? MDROver { get; set; }
        public float? StockOnHand { get; set; }
        public float? StockPurchase { get; set; }
        public float? StockAdjust { get; set; }
        public float? Wastage { get; set; }
        public float? Voids { get; set; }
        public float? Refunds { get; set; }
        public float? Markdown { get; set; }
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
