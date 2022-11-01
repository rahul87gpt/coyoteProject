using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IZoneOutletServices
    {
        Task<PagedOutputModel<List<ZoneOutletGetAllResponseModel>>> GetAllActiveZoneOutlet(PagedInputModel inputModel, SecurityViewModel securityViewModel);
        Task<ZoneOutletResponseModel> GetZoneOutletById(int zoneOutletId);
        Task<bool> DeleteZoneOutlet(int zoneOutletId, int userId);
        Task<ZoneOutletResponseModel> Update(ZoneOutletRequestModel viewModel, int userId);
        Task<ZoneOutletResponseModel> Insert(ZoneOutletRequestModel viewModel, int userId);
        Task<List<MasterListItemResponseViewModel>> GetZoneOutletByStoreId(int storeId);
        Task<PagedOutputModel<List<ZoneOutletGetAllResponseModel>>> GetActiveZoneOutlet(ZoneOutletInputModel inputModel, SecurityViewModel securityViewModel);
    }
}
