using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
   public interface IXeroAccountServices
    {
        Task<XeroAccountReponseModel> Insert(XeroAccountRequestModel viewModel, int userId);
        Task<XeroAccountReponseModel> Update(XeroAccountRequestModel viewModel, int xeroId, int userId);
        Task<PagedOutputModel<List<XeroAccountReponseModel>>> GetAllActiveXeroAccount(PagedInputModel inputModel, SecurityViewModel securityViewModel);
        Task<XeroAccountReponseModel> GetXeroAccountById(long xeroId);
        Task<bool> Delete(long xeroId, int userId);
    }
}
