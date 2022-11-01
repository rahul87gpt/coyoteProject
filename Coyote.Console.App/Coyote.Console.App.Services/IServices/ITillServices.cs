using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ITillServices
    {
        Task<TillResponseModel> Insert(TillRequestModel viewModel, int userId);
        Task<TillResponseModel> Update(TillRequestModel viewModel, int tillId, int userId);
        Task<TillResponseModel> GetTillById(int tillId);
        Task<PagedOutputModel<List<TillResponseModel>>> GetAllActiveTillAsync(PagedInputModel inputModel, SecurityViewModel securityViewModel);
        Task<bool> Delete(int tillId, int userId);
        Task<bool> DeleteMultiple(string tills, int userId);
        Task<PagedOutputModel<List<TillSyncResponseModel>>> GetAllTillSync(PagedInputModel inputModel, SecurityViewModel securityViewModel);
        Task<PagedOutputModel<List<TillSyncResponseModel>>> AddSyncTills(TillSyncRequestModel requestModel, int userId);
        Task<PagedOutputModel<List<TillJournalResponseModel>>> GetTillJournal(TillJournalInputModel inputModel, TillJournalRequestModel requestModel = null, SecurityViewModel securityViewModel = null);
        Task<PagedOutputModel<List<TillResponseModel>>> GetAllTills(PagedInputModel filter, SecurityViewModel securityViewModel);
    }
}
