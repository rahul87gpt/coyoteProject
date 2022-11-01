using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
   public interface ISupplierOrderScheduleServices
    {
        Task<SupplierOrderScheduleResponseModel> Insert(SupplierOrderScheduleRequestModel viewModel, int userId);
        Task<SupplierOrderScheduleResponseModel> GetSupplierOrderScheduleById(long soId);
        Task<PagedOutputModel<List<SupplierOrderScheduleResponseModel>>> GetAllActiveSchedule(PagedInputModel inputModel);

        Task<SupplierOrderScheduleResponseModel> Update(SupplierOrderScheduleRequestModel viewModel, int glId, int userId);
        Task<bool> Delete(long soId, int userId);
    }
}
