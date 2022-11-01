using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IRebateServices
    {
        Task<PagedOutputModel<List<RebateHeaderResponseModel>>> GetAllActiveHeaders(PagedInputModel inputModel,SecurityViewModel securityViewModel);

        Task<RebateResponseModel> GetRebateById(long Id);
        Task<RebateResponseModel> AddRebateAsync(RebateRequestModel requestModel, int UserId);
        Task<RebateResponseModel> UpdateRebate(RebateRequestModel requestModel, int id, int UserId);
        Task<bool> DeleteRebate(int Id, int userId);
    }
}