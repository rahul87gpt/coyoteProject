using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IStockAdjustService
    {
        Task<PagedOutputModel<List<StockAdjustHeaderResponseViewModel>>> GetAllActiveHeaders(StockAdjustFilter input, SecurityViewModel securityViewModel);
        Task<StockAdjustHeaderResponseViewModel> Insert(StockAdjustHeaderRequestModel viewModel, int userId);
        Task<StockAdjustHeaderResponseViewModel> GetStockAdjustById(long id);
        Task<bool> DeleteHeader(long id, int userId);
        Task<bool> DeleteHeaderDetailItem(long id, long itemId, int userId);
        Task<StockAdjustHeaderResponseViewModel> Update(StockAdjustHeaderRequestModel viewModel, long id, int userId);
        Task<PagedOutputModel<List<StockAdjustHeaderResponseViewModel>>> GetActiveHeaders(StockAdjustFilter input);
        Task<int> GetReferenceNo();
  }
}
