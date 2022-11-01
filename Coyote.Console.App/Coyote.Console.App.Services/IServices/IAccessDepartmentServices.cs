using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Coyote.Console.App.Services.IServices
{
    public interface IAccessDepartmentServices
    {

        Task<bool> DeleteAccessDepartment(int Id, int roleId);
        Task<PagedOutputModel<List<AccessDepartmentViewModel>>> GetAllAccessDepartment(PagedInputModel inputModel);
        Task<AccessDepartmentViewModel> GetAccessDepartmentById(int roleId);
        //Task<bool> Update(AccessDepartmentViewModel viewModel, int userId);
        Task<int> Insert(AccessDepartmentViewModel viewModel, int userId);




    }
}
