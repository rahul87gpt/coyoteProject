using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("RolesDefaultPermissions")]
    public class RolesDefaultPermissions : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.ModuleLength, ErrorMessage = ErrorMessages.ModuleMaxLength)]
        public string ModuleName { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.VerbLength, ErrorMessage = ErrorMessages.VerbMaxLength)]
        public string HttpVerb { get; set; }
        [Required]
        public string DefaultRolePermissionss { get; set; }


        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
    }
}
