using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface IHostSettingsService
    {
        Task<PagedOutputModel<List<HostSettingsResponseModel>>> GetAllHostSettings(PagedInputModel inputModel);
        Task<HostSettingsResponseModel> GetHostSettingsById(int Id);
        Task<HostSettingsResponseModel> AddHostSettings(HostSettingsRequestModel requestModel, int userId);
        Task<HostSettingsResponseModel> EditHostSettings(HostSettingsRequestModel requestModel, int Id, int userId);
        Task<bool> DeleteHostSettings(int Id, int userId);
    }
}
