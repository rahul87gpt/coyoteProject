using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IAPNServices
    {
        Task<PagedOutputModel<List<APNResponseViewModel>>> GetAllActiveAPN(APNFilter filter = null);
        Task<bool> DeleteAPN(long apnId, int userId);
        Task<APNResponseViewModel> GetAPNById(long apnId);
        Task<APNResponseViewModel> Update(APNRequestModel viewModel, long id, int userId);
        Task<APNResponseViewModel> Insert(APNRequestModel viewModel, int userId);

    Task<PagedOutputModel<List<APNResponseViewModel>>> GetActiveAPN(APNFilter filter = null);
  }
}
