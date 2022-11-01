using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ICommodityService
    {
        Task<PagedOutputModel<List<CommodityResponseModel>>> GetAllActiveCommodities(PagedInputModel inputModel);
        Task<CommodityResponseModel> GetCommodityById(int id);
        Task<bool> DeleteCommodity(int Id, int userId);
        Task<CommodityResponseModel> Update(CommodityRequestModel viewModel,int comId,int userId);
        Task<CommodityResponseModel> Insert(CommodityRequestModel viewModel, int userId);
        Task<PagedOutputModel<List<CommodityResponseModel>>> GetActiveCommodities(SecurityViewModel securityViewModel,PagedInputModel inputModel);
  }
}
