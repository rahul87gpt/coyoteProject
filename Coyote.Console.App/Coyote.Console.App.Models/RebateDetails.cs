using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("RebateDetails")]
    public class RebateDetails : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }

        public long RebateHeaderId { get; set; }
        public RebateHeader RebateHeader { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public float Amount { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
    }
}
