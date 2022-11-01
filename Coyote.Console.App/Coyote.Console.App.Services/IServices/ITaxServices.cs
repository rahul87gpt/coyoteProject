using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ITaxServices
    {
        Task<PagedOutputModel<List<TaxResponseModel>>> GetAllActiveTaxes(PagedInputModel inputModel);
        Task<TaxResponseModel> GetTaxById(long taxId);
        Task<bool> DeleteTax(long taxId, int userId);
        Task<TaxResponseModel> Update(TaxRequestModel viewModel,long taxId, int userId);
        Task<TaxResponseModel> Insert(TaxRequestModel viewModel, int userId);
    }
}
