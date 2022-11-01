using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("RebateHeader")]
    public class RebateHeader : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public RebateType Type  { get; set; }
        public int ManufacturerId { get; set; }
        public MasterListItems Manufacturer { get; set; }
        public int? ZoneId { get; set; }
        public MasterListItems Zone { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual ICollection<RebateOutlets> RebateOutlets { get; set; }
        public virtual ICollection<RebateDetails> RebateDetails { get;set; }
    }
}
