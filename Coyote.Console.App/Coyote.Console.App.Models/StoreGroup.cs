using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("StoreGroup")]
    public class StoreGroup : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(MaxLengthConstants.MinStoreGroupLength, ErrorMessage = ErrorMessages.StoreGroupCode)]
        [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.MaxStoreGroupLength, ErrorMessage = ErrorMessages.StoreGroupName)]
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public virtual ICollection<Store> Stores { get; }

        public virtual ICollection<Cashier> CashierStoreGroup { get; }
    }
}
