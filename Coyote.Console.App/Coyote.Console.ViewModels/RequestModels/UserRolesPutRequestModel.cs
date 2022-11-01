using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class UserRolesPutRequestModel
    {
        [Required(ErrorMessage = ErrorMessages.UserRoleDefaultRoleRequired)]
        public int DefaultRoleId { get; set; }

        public List<int> Roles { get; set; }
    }
}
