using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IUserServices
    {
        Task<bool> DeleteUser(int Id, int userId);
        Task<PagedOutputModel<List<UserSavedResponseViewModel>>> GetAllActiveUsers(PagedInputModel filter = null);
        Task<UserSavedResponseViewModel> GetUserById(int roleId);
        Task<bool> Update(UserRequestModel viewModel, int userId, string imagePath=null);
        Task<int> Insert(UserRequestModel viewModel, int userId);
        Task<bool> UpdateUserRoles(UserRolesPutRequestModel viewModel, int id, int userId);
        SecurityViewModel GetUserAllowedStoresId(string email);
        List<string> GetUserPermission(string email);
        Task SwitchDefaultRole(int roleId, int userId, int currUserId);
        Task<PagedOutputModel<List<UserSavedResponseViewModel>>> GetUserByAccessStores(PagedInputModel inputModel, int userId);
    }
}
