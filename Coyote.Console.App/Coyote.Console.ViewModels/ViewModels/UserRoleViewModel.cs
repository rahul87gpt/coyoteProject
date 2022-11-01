using System;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class UserRoleViewModel
    {
        public int Id { get; set; }
        //[MaxLength(MaxLengthConstants.MinUserRoleLength, ErrorMessage = ErrorMessages.UserRoleCode)]
        //[Required(ErrorMessage = ErrorMessages.UserRoleCodeRequired)]
        //public string Code { get; set; }
        //[MaxLength(MaxLengthConstants.MaxUserRoleLength, ErrorMessage = ErrorMessages.UserRoleName)]
        //[Required(ErrorMessage = ErrorMessages.UserRoleNameRequired)]
        //public string Name { get; set; }
        //[MaxLength(MaxLengthConstants.MaxUserRoleLength, ErrorMessage = ErrorMessages.UserRoleType)]
        //[Required(ErrorMessage = ErrorMessages.UserRoleTypeRequired)]
        //public string Type { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public bool IsDefault { get; set; }
    }
}
