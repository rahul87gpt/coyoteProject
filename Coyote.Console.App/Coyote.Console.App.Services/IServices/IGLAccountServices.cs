using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
   public interface IGLAccountServices
    {
        Task<bool> Delete(long glId, int userId);
        Task<GLAccountResponseModel> Update(GLAccountRequestModel viewModel, int glId, int userId);
        Task<GLAccountResponseModel> Insert(GLAccountRequestModel viewModel, int userId);
        Task<GLAccountResponseModel> GetGLAccountById(long glId);
        Task<PagedOutputModel<List<GLAccountResponseModel>>> GetAllActiveGLAccount(GLAccountFilters inputModel);

    }
}
