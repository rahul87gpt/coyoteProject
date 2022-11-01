using Coyote.Console.App.Models.EntityContracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("UserRole")]
    public class UserRoles : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDefault { get; set; }

        public virtual Roles Roles { get; set; }
        public virtual Users UserRoleList { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }

    }
}
