using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface ISystemControlsService
    {
        Task<SystemControlsResponseModel>GetSystemControls();
        Task<SystemControlsResponseModel> AddUpdateSystemControls(SystemControlsRequestModel requestModel, int userId);
    }
}
