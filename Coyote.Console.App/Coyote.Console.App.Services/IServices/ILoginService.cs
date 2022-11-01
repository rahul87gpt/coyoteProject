using Coyote.Console.App.Models;
using Coyote.Console.ViewModels.ViewModels;
using System.Data;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ILoginService
    { 
        //Task<bool> Update(Users viewModel, int userId);
        Task<bool> ResetPassword(ResetPasswordViewModel viewModel);
        Task<int> ForgotPassword(string useremail);
        Task<bool> ChangePasswrod(ResetPasswordViewModel viewModel);
        int GetMigrationHistory();

    }
}
