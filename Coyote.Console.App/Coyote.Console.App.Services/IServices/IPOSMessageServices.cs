using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
  public interface IPOSMessageServices
  {
    Task<PagedOutputModel<List<POSMessagesResponseModel>>> GetAllActivePOSMessages(PagedInputModel inputModel,SecurityViewModel securityViewModel);
    Task<POSMessagesResponseModel> GetPOSMessageById(int id);
    Task<POSMessagesResponseModel> Insert(POSMessageRequestModel viewModel, int userId);

    Task<POSMessagesResponseModel> Update(POSMessageRequestModel viewModel, int posId, int userId);
    Task<bool> Delete(long msgId, int userId);
    Task<int> POSNumber();
    Task<PagedOutputModel<List<POSMessagesResponseModel>>> GetActivePOSMessages(PagedInputModel inputModel);
    Task<POSMessagesResponseModel> POSMessageById(int id);
  }
}
