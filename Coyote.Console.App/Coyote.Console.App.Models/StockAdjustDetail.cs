using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("StockAdjustDetail")]
    public class StockAdjustDetail : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public long StockAdjustHeaderId { get; set; }
        public virtual StockAdjustHeader StockAdjustHeader { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public long OutletProductId { get; set; }
        public virtual OutletProduct OutletProduct { get; set; }
        public float Quantity { get; set; }
        public float LineTotal { get; set; }
        public float ItemCost { get; set; }

        public int ReasonId { get; set; }
        public virtual MasterListItems Reason { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

    }
}
