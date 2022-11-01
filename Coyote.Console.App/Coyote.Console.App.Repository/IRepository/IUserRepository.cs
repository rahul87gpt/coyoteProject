using Coyote.Console.App.Models;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Repository
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        bool UserExistsCheckByUserEmail(string emailAddress);

        bool DeleteUser(int Id);
        Task<PagedOutputModel<List<UserSavedResponseViewModel>>> GetAllActiveUsers(PagedInputModel filter = null);

        Task<UserSavedResponseViewModel> GetUserById(int roleId);
        string GetUserRole(string email);
        Task<int> SaveChangesAsync();
        List<RoleResponseViewModel> GetUserRoles(int userId);
    }
}
