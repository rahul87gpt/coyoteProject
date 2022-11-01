using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IStockTakeService
    {
        Task<PagedOutputModel<List<StockTakeHeaderResponseViewModel>>> GetAllActiveHeaders(PagedInputModel input, SecurityViewModel securityViewModel);
        Task<StockTakeHeaderResponseViewModel> Insert(StockTakeHeaderRequestModel viewModel, int userId);
        Task<StockTakeHeaderResponseViewModel> Update(StockTakeHeaderRequestModel viewModel, long id, int userId);
        Task<StockTakeHeaderResponseViewModel> GetStockTakeById(long id);
        Task<bool> DeleteHeader(long id, int userId);
        Task<bool> DeleteHeaderDetailItem(long id, long itemId, int userId);
        Task<PagedOutputModel<List<StockTakeHeaderResponseViewModel>>> GetActiveHeaders(PagedInputModel input, SecurityViewModel securityViewModel);
        Task<byte[]> GetExternalStockTakeFile(int storeId);
        Task<PagedOutputModel<List<StockTakeTabletResponseModel>>> TabletLoad(StockTakeTabletRequestModel requestModel);
        Task<StockTakeHeaderResponseViewModel> LoadProductRange(StockTakeLoadProdRangeRequestModel requestModel);
        Task<StockTakeHeaderResponseViewModel> Refresh(long id,int userId);

    }
}
