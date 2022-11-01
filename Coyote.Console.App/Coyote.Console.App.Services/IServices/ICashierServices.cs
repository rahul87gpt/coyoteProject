using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ICashierServices
    {
        Task<CashierResponseModel> Insert(CashierRequestModel viewModel, int userId);
        Task<CashierResponseModel> Update(CashierRequestModel viewModel, int cashierId, int userId, string imagePath = null);
        Task<CashierResponseModel> GetCashierById(long cashierId);
        Task<PagedOutputModel<List<CashierResponseModel>>> GetAllActiveCashier(PagedInputModel inputModel, SecurityViewModel securityViewModel);

        Task<bool> Delete(long cashierId, int userId);
    }
}
