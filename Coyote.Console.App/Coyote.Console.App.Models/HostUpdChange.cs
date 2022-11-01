using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("HostUpdChange")]
    public class HostUpdChange : IKeyIdentifier<long>, IAuditable<int>
    {
        [Key]
        public long Id { get; set; }
        public int HostUpdId { get; set; }
        public int HostId { get; set; }
        public int ChangeTypeId { get; set; }
        public long? ProductId { get; set; }
        public int? PromotionId { get; set; }
        public int? OutletId { get; set; }
        public bool? Status { get; set; }
        public float CtnCostBefore { get; set; }
        public float CtnCostAfter { get; set; }
        public float Price1Before { get; set; }
        public float CtnCostSuggested { get; set; }
        public float CtnQtyBefore { get; set; }
        public float CtnQtyAfter { get; set; }

        public long HostUpdTimeStamp { get; set; }

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

        public string Promocode { get; set; }
        public string PromoDesc { get; set; }
        public virtual HostProcessing HostUpd { get; set; }
        public virtual HostSettings Host { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual Product Product { get; set; }
        public virtual Store Outlets { get; set; }
        public virtual MasterListItems ChangeType { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
    }
}
