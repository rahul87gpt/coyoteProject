using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("BulkOrderFromTablet")]
    public class BulkOrderFromTablet : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public int OutletId { get; set; }
        public virtual Store Store { get; set; }
        public long ProductNumber  { get; set; }    
        [Column(TypeName = "datetime")]
        public DateTime OrderBatch { get; set; }
        public float Qty { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastImport { get; set; }
        [MaxLength(MaxLengthConstants.ReferenceLength)]
        public string Type { get; set; }
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
