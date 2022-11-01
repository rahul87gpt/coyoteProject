using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IOutletProductService
    {
        Task<PagedOutputModel<List<OutletProductResponseViewModel>>> GetAllActiveOutletProducts(OutletProductFilter inputModel, SecurityViewModel securityViewModel);
        Task<List<OutletProductResponseViewModel>> Insert(OutletProductRequestModel viewModel, int userId);
        Task<bool> DeleteOutletProduct(long storeId, long productId, long? supplierId, int userId);
        Task<List<OutletProductResponseViewModel>> GetOutletProductsByStoreId(long storeId);
        Task<bool> Update(OutletProductRequestModel viewModel, long id, int userId);
        Task<bool> SetMinOnHandInOutletProduct(MinStockHandRequestModel viewModel);
        Task<bool> DeativateProducts(DeactivateProductListRequestModel inputModel, int id);
        Task<PagedOutputModel<List<OutletProductResponseViewModel>>> GetActiveOutletProducts(OutletProductFilter inputModel, SecurityViewModel securityViewModel);

        Task<PagedOutputModel<List<OutletProductResponseViewModel>>> GetActiveOutletProductsByProductId(OutletProductFilter inputModel, SecurityViewModel securityViewModel);
        
        Task<PagedOutputModel<List<DeactivateProductListResponseModel>>> GetDeactivateProductList(PagedInputModel filter, DeactivateProductListRequestModel inputModel, SecurityViewModel securityViewModel);
    }
}
