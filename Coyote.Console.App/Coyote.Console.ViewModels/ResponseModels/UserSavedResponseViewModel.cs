using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class UserSavedResponseViewModel : UserRequestModel
    {
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }

        public string ImageUploadStatusCode { get; set; }
        public string ImagePath { get; set; }

        public List<StoreResponseModel> StoreList { get; } = new List<StoreResponseModel>();
        public List<MasterListItemResponseViewModel> ZoneList { get; } = new List<MasterListItemResponseViewModel>();
        public List<RoleResponseViewModel> UserRolesList { get; } = new List<RoleResponseViewModel>();
    }
}
