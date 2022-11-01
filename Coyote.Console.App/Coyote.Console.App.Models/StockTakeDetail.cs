using Coyote.Console.App.Models.EntityContracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("StockTakeDetail")]
    public class StockTakeDetail : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public long StockTakeHeaderId { get; set; }
        public long OutletProductId { get; set; }
        public string Desc { get; set; }
        public int OnHandUnits { get; set; }
        public float Quantity { get; set; }
        public float ItemCost { get; set; }
        public float LineCost { get; set; }
        public float LineTotal { get; set; }
        public int ItemCount { get; set; }
        public float VarQty { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual StockTakeHeader StockTakeHeader { get; set; }
        public virtual OutletProduct OutletProduct { get; set; }
    }
}
