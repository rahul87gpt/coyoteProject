using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Role")]
    public class Roles : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.MinRoleLength, ErrorMessage = ErrorMessages.RoleCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxRoleLength, ErrorMessage = ErrorMessages.RoleName)]
        [Required]
        public string Name { get; set; }
        public bool Status { get; set; }
        public string PermissionSet { get; set; }
        [MaxLength(MaxLengthConstants.MaxRoleLength, ErrorMessage = ErrorMessages.RoleType)]
        [Required]
        public string Type { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public string PermissionDeptSet { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; }
       // public virtual ICollection<UserLog> UserLogRoles { get; }
    }
}
