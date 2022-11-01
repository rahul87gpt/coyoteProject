using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IWarehouseServices
    {
        Task<PagedOutputModel<List<WarehouseResponseViewModel>>> GetAllActiveWarehouse(PagedInputModel filter = null);
        Task<WarehouseResponseViewModel> GetWarehouseById(int wearehouseId);
        Task<bool> DeleteWarehouse(int wearehouseId, int userId);
        Task<WarehouseResponseViewModel> Update(WarehouseRequestModel viewModel,int warehouseId, int userId);
        Task<WarehouseResponseViewModel> Insert(WarehouseRequestModel viewModel, int userId);
    }
}
