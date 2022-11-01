using Coyote.Console.ViewModels.ViewModels;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IModuleActionsService
    {
        Task<ModuleActionsViewModel> GetById(int id);
    }
}
