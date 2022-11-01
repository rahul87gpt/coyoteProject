using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("ZonePricing")]
    public class ZonePricing : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int PriceZoneId { get; set; }
        public virtual CostPriceZones PriceZoneCostPrice { get; set; }
        public float? CtnCost { get; set; }
        public DateTime? CtnCostDate { get; set; }
        public float? CtnCostSV1 { get; set; }
        public DateTime? CtnCostDateSV1 { get; set; }
        public float? CtnCostSV2 { get; set; }

        public DateTime? CtnCostDateSV2 { get; set; }

        public float? CtnCostStd { get; set; }
        public float? Price { get; set; }
        public float? PriceSV1 { get; set; }
        public float? PriceSV2 { get; set; }

        public DateTime? PriceDate { get; set; }
        public DateTime? PriceDateSV1 { get; set; }
        public DateTime? PriceDateSV2 { get; set; }
        public float? MinReorderQty { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }

    }
}
