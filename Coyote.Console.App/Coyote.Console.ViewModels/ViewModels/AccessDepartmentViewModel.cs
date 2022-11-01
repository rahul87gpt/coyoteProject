using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Coyote.Console.ViewModels.ViewModels
{
    public class AccessDepartmentViewModel
    {

        public int Id { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        //public int CreatedById { get; set; }
        //public int UpdatedById { get; set; }
        //[Column(TypeName = "datetime")]
        //public DateTime CreatedAt { get; set; }
        //[Column(TypeName = "datetime")]
        //public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
