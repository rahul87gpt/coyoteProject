using Coyote.Console.App.Models;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplate> GetEmailTemplateByName(string templateName);
    }
}
