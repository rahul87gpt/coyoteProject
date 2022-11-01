using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IPromotionService
    {
        Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetAllActivePromotionHeaders(SecurityViewModel securityViewModel, PromotionFilter inputModel);
        Task<PromotionResponseViewModel> GetPromotionById(int id);
        Task<PromotionResponseViewModel> Insert(PromotionRequestModel viewModel, int userId);
        Task<bool> DeletePromotion(int id, int userId);
        Task<PromotionResponseViewModel> Update(PromotionRequestModel viewModel, int id, int userId);
        Task<PromotionDetailResponseViewModel> GetPromotionDetailsById(int id);
        Task<int> AddPromotionDetail(PromotionDetailRequestModel viewModel, int userId);
        Task<bool> DeletePromotionDetails(int id, int detailId, int userId);
        Task<bool> UpdatePromotionDetails(int promotionId, PromotionDetailRequestModel viewModel, int userId);

        Task<PromotionDetailResponseViewModel> CopyPromotion(PromotionCloneFilter inputModel, int userId);
        Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetActivePromotionHeaders(SecurityViewModel securityViewModel, PromotionFilter inputModel);

        Task<int> ImportPromotionDetail(PromotionImportRequestModel importRequestModel, string path, int userId, int?id);
    }
}
