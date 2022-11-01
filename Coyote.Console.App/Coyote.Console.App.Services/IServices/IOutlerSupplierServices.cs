using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IOutlerSupplierServices
    {
        Task<PagedOutputModel<List<OutletSupplierResponseModel>>> GetAllActiveOutletSupplierAsync(PagedInputModel inputModel, SecurityViewModel securityViewModel=null);
        Task<OutletSupplierResponseModel> GetOuletSupplierById(int outletSupplierId);
        Task<OutletSupplierResponseModel> Insert(OutletSupplierRequestModel viewModel, int userId);
        Task<OutletSupplierResponseModel> Update(OutletSupplierRequestModel viewModel, int outletSupplierId, int userId);
        Task<bool> Delete(int outletSupplierId, int userId);
        Task<PagedOutputModel<List<OutletSupplierResponseModel>>> GetActiveOutletSupplier(PagedInputModel inputModel, SecurityViewModel securityViewModel);
    }
}
