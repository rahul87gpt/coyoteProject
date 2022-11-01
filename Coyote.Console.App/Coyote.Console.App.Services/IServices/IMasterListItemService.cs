using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IMasterListItemService
    {
        Task<bool> DeleteMasterListItem(int id, string code, int userId);
        Task<PagedOutputModel<List<MasterListItemResponseViewModel>>> GetAllActiveMasterListItems(string code, SecurityViewModel securityViewModel, PagedInputModel filter = null);
        Task<MasterListItemResponseViewModel> GetMasterListItemsById(int id, string code);
        Task<bool> Update(MasterListItemRequestModel viewModel, string code, int id, int userId);
        Task<MasterListItemResponseViewModel> AddMasterListItem(MasterListItemRequestModel viewModel, string code, int userId);
        Task<PagedOutputModel<List<MasterListItemResponseViewModel>>> GetActiveMasterListItems(string code, SecurityViewModel securityViewModel, MasterListFilterModel filter = null);
    }
}
