using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IProductService
    {
        Task<PagedOutputModel<List<ProductResponseModel>>> GetAllActiveProducts(ProductFilter inputModel, SecurityViewModel securityViewModel);
        Task<ProductResponseModel> GetProductById(long id);
        Task<ProductResponseModel> Insert(ProductRequestModel viewModel, int userId);
        Task<ProductResponseModel> Update(ProductRequestModel viewModel, long productId, int userId, string imagePath = null);
        Task<bool> Delete(long productId, int userId);
        Task<ProductNumberModel> GetNewProductNumber(string Extend = null);
        Task<PagedOutputModel<List<ProductResponseModel>>> GetAllProductsWithoutAPN(PagedInputModel filter = null);
        Task<ProductTabsHistoryResponseModel> GetProductTabsHistory(SecurityViewModel securityViewModel, ProductHistoryFilter inputFilter);
        Task<PagedOutputModel<List<ProductResponseModel>>> GetActiveProducts(ProductFilter inputModel, SecurityViewModel securityViewModel);
        Task<PagedOutputModel<List<ProductResponseModel>>> GetActiveReplicateProducts(ProductFilter inputModel, SecurityViewModel securityViewModel);
        Task<ProductResponseModel> GetByProductId(long id, PagedInputModel inputModel, bool fetchListItems = true);

        Task<ProductNumberModel> GetNewProductNo(PagedInputModel inputModel, string Extend = null);
        Task<PagedOutputModel<List<ProductResponseModel>>> GetProductsWithoutAPN(ProductFilter inputModel, SecurityViewModel securityViewModel);
        Task<ProductResponseModel> UpdateProduct(ProductRequestModel viewModel, long productId, int userId, SecurityViewModel securityViewModel, string imagePath = null);
        Task<ProductResponseModel> InsertProduct(ProductRequestModel viewModel, SecurityViewModel securityViewModel, int userId);

        Task<ProductTabsHistoryResponseModel> ProductTabsHistory(SecurityViewModel securityViewModel, ProductHistoryFilter inputFilter);
    }
}
