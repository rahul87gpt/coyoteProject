using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("CostPriceZones")]
    public class CostPriceZones : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string Code { get; set; }
        public CostPriceZoneType Type { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.SuppDescLength, ErrorMessage = ErrorMessages.SuppDesc)]
        public string Description { get; set; }
        [Required]
        public int? HostSettingID { get; set; }
        public virtual HostSettings HostSettings { get; set; }
        public float? Factor1 { get; set; }
        public float? Factor2 { get; set; }
        public float? Factor3 { get; set; }
        [Required]
        public bool SuspUpdOutlet { get; set; }
        public Status IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public virtual Users UserModifiedBy { get; set; }
        public virtual ICollection<Store> StorePriceZones { get; }
        public virtual ICollection<Store> StoreCostZones { get; }
        public virtual ICollection<ZonePricing> ZonePricingCostZone { get; }
    }
}
