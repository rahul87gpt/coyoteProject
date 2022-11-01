using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Warehouse")]
    public class Warehouse : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.MinWarehouseCodeLength, ErrorMessage = ErrorMessages.WarehouseCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MinWarehouseDescLength, ErrorMessage = ErrorMessages.WarehouseDesc)]
        [Required]
        public string Desc { get; set; }
        public int SupplierId { get; set; }
        public int HostFormatId { get; set; }
        public bool Status { get; set; }
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
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Store> StoreWarehouse { get; }
        public virtual MasterListItems HostFormatMasterListItem { get; set; }
        public virtual ICollection<HostSettings> HostSettingsWarehouse { get; }

    }
}
