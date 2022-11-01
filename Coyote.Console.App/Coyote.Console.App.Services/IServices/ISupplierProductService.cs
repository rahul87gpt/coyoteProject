using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ISupplierProductService
    {
        Task<PagedOutputModel<List<SupplierProductResponseViewModel>>> GetAllActiveSupplierProducts(SupplierProductFilter inputModel);
        Task<List<SupplierProductResponseViewModel>> GetSupplierProductsById(long supplierId);
        Task<bool> DeleteSupplierProduct(long supplierId, int userId);
        Task<List<SupplierProductResponseViewModel>> Update(SupplierProductRequestModel viewModel, int id, int userId);
        Task<List<SupplierProductResponseViewModel>> Insert(SupplierProductRequestModel viewModel, int userId);
        Task<PagedOutputModel<List<SupplierProductResponseViewModel>>> GetAllSupplierProducts(SupplierProductFilter inputModel);
    }
}
