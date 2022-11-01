using Coyote.Console.Common;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class RolesViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorMessages.RoleNameRequired)]
        [MaxLength(MaxLengthConstants.MaxRoleLength, ErrorMessage = ErrorMessages.RoleName)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.RoleCodeRequired)]
        [MaxLength(MaxLengthConstants.MinRoleLength, ErrorMessage = ErrorMessages.RoleCode)]
        public string Code { get; set; }

        public bool? Status { get; set; }

        [Required(ErrorMessage = ErrorMessages.RolePermissionRequired)]
        public string PermissionSet { get; set; }

        [Required(ErrorMessage = ErrorMessages.RoleTypeRequired)]
        [MaxLength(MaxLengthConstants.MaxRoleLength, ErrorMessage = ErrorMessages.RoleType)]
        public string Type { get; set; }

        public string PermissionDeptSet { get; set; }
    }
}
