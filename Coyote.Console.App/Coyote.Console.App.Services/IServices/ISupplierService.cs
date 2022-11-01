using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
  public interface ISupplierService
  {
    Task<PagedOutputModel<List<SupplierResponseViewModel>>> GetAllActiveSuppliers(PagedInputModel inputModel);
    Task<SupplierResponseViewModel> GetSuppliersById(int supplierId);
    Task<bool> DeleteSupplier(int supplierId, int userId);
    Task<SupplierResponseViewModel> Update(SupplierRequestModel viewModel, int id, int userId);
    Task<SupplierResponseViewModel> Insert(SupplierRequestModel viewModel, int userId);
    Task<PagedOutputModel<List<SupplierResponseViewModel>>> GetActiveSuppliers(PagedInputModel inputModel,SecurityViewModel securityViewModel);
  }
}
