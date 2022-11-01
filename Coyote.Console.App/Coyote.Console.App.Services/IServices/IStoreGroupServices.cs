using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IStoreGroupServices
    {
        Task<bool> DeleteStoreGroup(int Id, int userId);
        Task<PagedOutputModel<List<StoreGroupResponseModel>>> GetAllActiveStoreGroups(PagedInputModel filter);
        Task<StoreGroupResponseModel> GetStoreGroupsById(int Id);
        Task<StoreGroupResponseModel> Update(StoreGroupRequestModel viewModel,int groupId, int userId);
        Task<StoreGroupResponseModel> Insert(StoreGroupRequestModel viewModel, int userId);
        Task<PagedOutputModel<List<CorporateTreeResponseModel>>> GetCorporateTree(PagedInputModel filter);
    }
}
