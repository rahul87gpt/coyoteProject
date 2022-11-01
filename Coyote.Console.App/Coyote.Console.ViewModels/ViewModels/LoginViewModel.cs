using Coyote.Console.Common;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = ErrorMessages.UserEmailForLogin)]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = ErrorMessages.UserNameRequired)]
        public string UserName { get; set; }

        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = ErrorMessages.NewUserPassword)]
        //[Required(ErrorMessage = ErrorMessages.UserPasswordForLogin)]
        public string Password { get; set; }

        public string Token { get; set; }
        public DateTimeOffset? TokenExpiration { get; set; }
        public int? TokenTimeOut { get; set; }
        public string RefreshToken { get; set; }
        public bool FirstLogin { get; set; }
        public int? UserId { get; set; }
        public List<RoleResponseViewModel> Roles { get; set; }
        public int DefaultRoleId { get; set; }
        public string DefaultRolePermissions { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Image { get; set; }
        public string DataSource { get; set; }
        public string Database { get; set; }
        public string MigrationName { get; set; }



    }

    public class LoginRequestViewModel
    {
        [Required(ErrorMessage = ErrorMessages.UserEmailForLogin)]
        //commented as UserEmail can have Username
        //[EmailAddress]
        public string UserEmail { get; set; }
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = ErrorMessages.NewUserPassword)]

        [Required(ErrorMessage = ErrorMessages.UserPasswordForLogin)]
        public string Password { get; set; }

    }

    public class RevokeTokenRequestModel { 
    
        [Required]
        public int userId { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }

    public class SwitchRoleRequestModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
