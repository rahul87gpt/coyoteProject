using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.IServices
{
    public interface IPathsService
    {
        Task<PagedOutputModel<List<PathsResponseModel>>> GetAllPaths(PagedInputModel inputModel);
        Task<PathsResponseModel> GetPathById(int Id);
        Task<PathsResponseModel> InsertPath(PathsRequestModel viewModel, string path, int userId);
        Task<PathsResponseModel> UpdatePath(PathsRequestModel viewModel, int pathId, string path, int userId);
        Task<bool> DeletePath(int Id, int userId);
    }
}
