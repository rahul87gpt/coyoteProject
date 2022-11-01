using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("AccessDepartment")]
   public class AccessDepartment
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public int RoleId { get; set; }
        public virtual Roles Roles { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public virtual Department Departments { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
    }
}
