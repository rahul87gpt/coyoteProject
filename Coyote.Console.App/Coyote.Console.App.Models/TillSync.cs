using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("TillSync")]
    public class TillSync : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }
        public int TillId { get; set; }
        public virtual Till Till { get; set; }
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        public TillSyncType? Product { get; set; }
        public TillSyncType? Cashier { get; set; }
        public TillSyncType? Account { get; set; }
        public TillSyncType? Keypad { get; set; }
        public DateTime? TillActivity { get; set; }
        [MaxLength(MaxLengthConstants.MaxTillVersionLength)]
        public string ClientVersion { get; set; }
        [MaxLength(MaxLengthConstants.MaxTillVersionLength)]
        public string PosVersion { get; set; }
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
