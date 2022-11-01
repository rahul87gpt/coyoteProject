using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IMasterListService
    {
        Task<bool> DeleteMasterList(int id, int userId);
        Task<PagedOutputModel<List<MasterListResponseViewModel>>> GetAllActiveMasterLists(PagedInputModel inputModel);
        Task<MasterListResponseViewModel> GetMasterListById(int id);
        Task<MasterListResponseViewModel> GetMasterListByCode(string Code);
        Task<bool> Update(MasterListRequestModel viewModel,int id, int userId);
        Task<int> Insert(MasterListRequestModel viewModel,int userId);
        Task<List<string>> GetNationalLevelSalesPeriod();
    }
}
