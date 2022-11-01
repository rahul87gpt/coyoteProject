using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IDepartmentService
    {
        Task<PagedOutputModel<List<DepartmentResponseModel>>> GetAllActiveDepartments(PagedInputModel filter = null);
        Task<PagedOutputModel<List<DepartmentResponseModel>>> GetActiveDepartments(PagedInputModel inputModel = null);
        Task<DepartmentResponseModel> GetDepartmentById(int roleId);
        Task<bool> DeleteDepartment(int Id, int userId);
        Task<DepartmentResponseModel> Update(DepartmentRequestModel viewModel, int deptId, int userId);
        Task<DepartmentResponseModel> Insert(DepartmentRequestModel viewModel, int userId);
    }
}
