using Coyote.Console.Common;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string TempPassword { get; set; }
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = ErrorMessages.NewUserPassword)]
        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
