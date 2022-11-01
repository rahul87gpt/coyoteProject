using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IUserRoleService
    {
        Task<bool> DeleteUserRole(int Id, int userId);
        Task<PagedOutputModel<List<UserRoleResponseModel>>> GetAllActiveUserRoles(PagedInputModel filter = null, int? UserId = null);
        Task<UserRoleViewModel> GetUserRoleById(int roleId);
        Task<bool> Update(UserRoleViewModel viewModel, int userId);
        Task<int> Insert(UserRoleViewModel viewModel, int userId);
    }
}
