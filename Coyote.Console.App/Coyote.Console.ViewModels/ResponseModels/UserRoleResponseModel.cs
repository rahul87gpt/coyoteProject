using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class UserRoleResponseModel : UserRoleViewModel
    {
        //public StoreResponseViewModel Store { get; set; }
        public RoleResponseViewModel Role { get; set; }
        public UserDetailResponseModel User { get; set; }
    }
    public class UserRoleListResponseModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public List<RoleResponseViewModel> Roles { get; set; }
        public UserDetailResponseModel User { get; set; }
    }
}
