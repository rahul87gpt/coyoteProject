using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface IHostProcessing
    {
        Task<PagedOutputModel<List<HostProcessingResponseModel>>> GetAllHostProcessing(PagedInputModel inputModel);
        Task<HostProcessingResponseModel> AddUpdateHostProcessing(HostProcessingRequestModel requestModel, int userId);
        Task<HOSTUPDUserActivityResponseModel> HostChangesSheet(long HostSettingId);
        //Task<PagedOutputModel<List<UserLogResponseModel<T>>>> GetHOSTUPDUserLog<T>(long HostSettingId) where T : class;
    }
}
