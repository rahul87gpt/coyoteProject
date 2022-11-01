using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface ICostPriceZonesService
    {
        Task<PagedOutputModel<List<CostPriceZonesResponseModel>>> GetAllCostPriceZones(PagedInputModel inputModel, int Types);
        Task<CostPriceZonesResponseModel> GetCostPriceZonesById(int Id);
        Task<CostPriceZonesResponseModel> AddCostPriceZones(CostPriceZonesRequestModel requestModel, int userId,int Type);
        Task<CostPriceZonesResponseModel> EditCostPriceZones(CostPriceZonesRequestModel requestModel, int Id, int userId,int Type);
        Task<bool> DeleteCostPriceZones(int Id, int userId);
    }
}
