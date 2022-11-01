using Coyote.Console.App.Models;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Coyote.Console.App.Services.IServices
{
    public interface ISendMailService
    {
        Task SendEmailsLogAsync(SendEmailViewModel sendEmailsViewModel, int userId);
        Task<DeliveryNotificationOptions> SendMail(Users user, bool isForgotPassword);
        Task<DeliveryNotificationOptions> SendMail(UserRequestModel user, bool isForgotPassword);
        Task<DeliveryNotificationOptions> SendMailForEDI(string attachmentName);
        Task<bool> SendReportMail(List<ReportMailUserModel> userList, byte[] pdf,string body = null, string reportName = null);
        Task<bool> SendReportFailMail(SendEmailViewModel mailModel);
    }
}
