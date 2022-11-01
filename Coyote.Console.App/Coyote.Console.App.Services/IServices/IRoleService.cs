using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IRoleService
    {
        Task<bool> DeleteRole(int Id, int userId);
        Task<PagedOutputModel<List<RolesViewModel>>> GetAllActiveRoles(PagedInputModel inputModel);
        Task<RolesViewModel> GetRoleById(int roleId);
        Task<bool> Update(RolesViewModel viewModel, int userId);
        Task<int> Insert(RolesViewModel viewModel, int userId);
    }
}
