using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class UserResponseViewModel
    {
        public UserSavedResponseViewModel User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public List<RoleResponseViewModel> Roles { get; set; }
    }
}
