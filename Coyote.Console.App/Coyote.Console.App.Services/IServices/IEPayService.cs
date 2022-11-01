using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface IEPayService
    {
        Task<PagedOutputModel<List<EPayResponseModel>>> GetAllEPay(PagedInputModel inputModel);
        Task<EPayResponseModel> GetEPayById(int Id);      
    }
}
