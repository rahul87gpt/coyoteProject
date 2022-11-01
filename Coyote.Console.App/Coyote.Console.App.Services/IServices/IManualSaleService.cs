using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    /// <summary>
    /// IManualSaleService
    /// </summary>
    public interface IManualSaleService
    {
        Task<int> GetManualSaleRefrenceNo();
        Task<PagedOutputModel<List<ManualSaleResponseModel>>> GetManualSale(PagedInputModel inputModel, SecurityViewModel securityViewModel);
        Task<ManualSaleResponseModel> GetManualSaleById(long Id);
        Task<ManualSaleResponseModel> AddManualSale(ManualSaleRequestModel requestModel, int userId);
        Task<ManualSaleResponseModel> EditManualSale(ManualSaleRequestModel requestModel, long Id, int userId);
        Task<bool> DeleteManualSale(long Id, int userId);
        Task<bool> DeleteManualSaleItem(long Id,int userId);
    }
}
