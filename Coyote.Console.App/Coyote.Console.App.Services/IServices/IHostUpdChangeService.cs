using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
   public interface IHostUpdChangeService
    {
        Task<PagedOutputModel<List<HOSTUPDChangeResponseModel>>> GetAllHostUpdChange(PagedInputModel inputModel,long HostId);
       
     
    }
}
