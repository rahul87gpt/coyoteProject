using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("OutletSupplierSetting")]
    public class OutletSupplierSetting : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        public int StoreId { get; set; }
        public virtual Store Store { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

		public bool Status { get; set; } = true;

        [Required]
        [MaxLength(MaxLengthConstants.MaxOutletSupplierSettingDescLength, ErrorMessage = ErrorMessages.OutletSupplierSettingDescLength)]
        public string Desc { get; set; }

        [MaxLength(MaxLengthConstants.MaxOutletSupplierSettingDescLength)]
        public string CustomerNumber { get; set; }

        public int? StateId { get; set; }
        public virtual MasterListItems StateMasterListItem { get; set; }

        public int? DivisionId { get; set; }
        public virtual MasterListItems DivisionMasterListItem { get; set; }

        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string QtyDefault { get; set; }
        public bool BuyCartoon { get; set; } = true;


        public string PostedOrder { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }

        public bool IsDeleted { get; set; } = false ;
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
    }
}

