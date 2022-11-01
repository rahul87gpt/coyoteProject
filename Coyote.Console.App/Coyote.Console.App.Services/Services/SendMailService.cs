using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
namespace Coyote.Console.App.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _autoMappingServices = null;
        private ILoggerManager _iLoggerManager = null;
        private EDIEmailSettings _ediEmailSetting { get; }
        public SendMailService(IOptions<AppSettings> appsetting, IUnitOfWork repository, IAutoMappingServices autoMappingServices, ILoggerManager iLoggerManager, IOptions<EDIEmailSettings> ediEmailSettings)
        {
            _appsetting = appsetting?.Value;
            _unitOfWork = repository;
            _autoMappingServices = autoMappingServices;
            _iLoggerManager = iLoggerManager;
            _ediEmailSetting = ediEmailSettings?.Value;
        }
        private AppSettings _appsetting
        {
            get; set;
        }
        public async Task<DeliveryNotificationOptions> SendMail(UserRequestModel user, bool isForgotPassword)
        {
            var userViewModel = _autoMappingServices.Mapping<UserRequestModel, Users>(user);

            return await SendMail(userViewModel, isForgotPassword).ConfigureAwait(false);
        }
        public async Task<DeliveryNotificationOptions> SendMail(Users user, bool isForgotPassword)
        {
            string subject, body;
            SendEmailViewModel sendEmailsViewModel = new SendEmailViewModel();
            Uri resetUrl = _appsetting.ResetUrl;

            string resetLink = $"{resetUrl}?UserId={user?.Id}&TempPassword={user.TemporaryPassword}";
            resetLink = string.Format(CultureInfo.CurrentCulture, "<a href='{0}'>{1}</a>", resetLink, "Click here");

            var repository = _unitOfWork?.GetRepository<EmailTemplate>();
            if (isForgotPassword)
            {
                var emailTemplate = await repository.GetAll(x => x.Name == EmailTemplateNameConstants.ForgotPassword).FirstOrDefaultAsync().ConfigureAwait(true);
                body = emailTemplate.Body;

                body = body
                            .Replace(EmailTemplateNameConstants.UserFirstNameMergeField, user.FirstName, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.UserLastNameMergeField, user.LastName, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.UserNameMergeField, user.Email, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.TemporaryPasswordURLMergeField, resetLink, StringComparison.OrdinalIgnoreCase);
                subject = emailTemplate.Subject;
                sendEmailsViewModel.EmailSubject = subject;
                sendEmailsViewModel.TemplateId = emailTemplate.Id;
                sendEmailsViewModel.TemplateName = emailTemplate.Name;
            }
            else
            {
                Uri newPasUrl = _appsetting.LoginUrl;

                string newPassLink = $"{newPasUrl}";
                newPassLink = string.Format(CultureInfo.CurrentCulture, "<a href='{0}'>{1}</a>", newPassLink, "click here");

                var emailTemplate = await repository.GetAll(x => x.Name == EmailTemplateNameConstants.NewUserCreation).FirstOrDefaultAsync().ConfigureAwait(true);
                body = emailTemplate.Body;

                body = body
                            .Replace(EmailTemplateNameConstants.UserFirstNameMergeField, user.FirstName, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.UserLastNameMergeField, user.LastName, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.UserNameMergeField, user.Email, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.PasswordMergeField, user.PlainPassword, StringComparison.OrdinalIgnoreCase)
                            .Replace(EmailTemplateNameConstants.TemporaryPasswordURLMergeField, newPassLink, StringComparison.OrdinalIgnoreCase);
                subject = emailTemplate.Subject;
                sendEmailsViewModel.EmailSubject = subject;
                sendEmailsViewModel.MsgBody = body;
                sendEmailsViewModel.TemplateId = emailTemplate.Id;
                sendEmailsViewModel.TemplateName = emailTemplate.Name;
            }


            // Mail message
            var mail = new MailMessage
            {
                From = new MailAddress(_appsetting.FromEmail),
                Subject = subject,
                Body = body
            };

            mail.IsBodyHtml = true;
            mail.To.Add(new MailAddress(user.Email));
            sendEmailsViewModel.FromAddress = _appsetting.FromEmail;
            sendEmailsViewModel.ToAddress = user.Email;
            sendEmailsViewModel.CreatedById = Convert.ToInt32(user.Id);
            sendEmailsViewModel.UpdatedById = Convert.ToInt32(user.Id);
            sendEmailsViewModel.CreatedAt = DateTime.UtcNow;
            sendEmailsViewModel.UpdatedAt = DateTime.UtcNow;

            // Credentials
            var credentials = new NetworkCredential(_appsetting.FromEmail, _appsetting.Password);

            // Smtp client
            var client = new SmtpClient
            {
                Port = Convert.ToInt32(_appsetting.SMTPPort, CultureInfo.CurrentCulture),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = _appsetting.Host,
                Credentials = credentials,
                EnableSsl = false
            };


            try
            {
                await client.SendMailAsync(mail).ConfigureAwait(false);
                sendEmailsViewModel.IsSendEmail = true;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            }
            catch (Exception ex)
            {
                sendEmailsViewModel.IsSendEmail = false;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
                _iLoggerManager.WriteErrorLog(ex);
            }
            finally
            {
                client.Dispose();
                mail.Dispose();
            }

            //Savin sent email
            await SendEmailsLogAsync(sendEmailsViewModel, user.Id).ConfigureAwait(false);
            return mail.DeliveryNotificationOptions;

        }

        public async Task SendEmailsLogAsync(SendEmailViewModel sendEmailsViewModel, int userId)
        {
            try
            {
                if (sendEmailsViewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<SendEmail>();
                    //var sendEmailsModel = _autoMappingServices.Mapping<SendEmailViewModel, SendEmail>(sendEmailsViewModel);
                    //await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@FromAddress",sendEmailsViewModel.FromAddress),
                        new SqlParameter("@ToAddress",sendEmailsViewModel.ToAddress),
                        new SqlParameter("@CC",sendEmailsViewModel.CC),
                        new SqlParameter("@BCC",sendEmailsViewModel.BCC),
                        new SqlParameter("@MsgBody",sendEmailsViewModel.MsgBody),
                        new SqlParameter("@TemplateId",sendEmailsViewModel.TemplateId),
                        new SqlParameter("@TemplateName",sendEmailsViewModel.TemplateName),
                        new SqlParameter("@TemplateContent",sendEmailsViewModel.TemplateContent),
                        new SqlParameter("@EmailSubject",sendEmailsViewModel.EmailSubject),
                        new SqlParameter("@CreatedById",sendEmailsViewModel.CreatedById),
                        new SqlParameter("@IsSendEmail",sendEmailsViewModel.IsSendEmail)
                    };
                    await repository.ExecuteStoredProcedure(StoredProcedures.SaveEmails, dbParams.ToArray()).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
                _iLoggerManager.WriteErrorLog(ex);
                throw;
            }
        }

        public async Task<DeliveryNotificationOptions> SendMailForEDI(string attachmentName)
        {
            // Credentials
            var credentials = new NetworkCredential(_ediEmailSetting.Username, _ediEmailSetting.Password);
            // Mail message
            var mail = new MailMessage
            {
                From = new MailAddress(_ediEmailSetting.FromAddress),
                Subject = _ediEmailSetting.EmailSubject,
                Body = _ediEmailSetting.EmailBody
            };

            mail.IsBodyHtml = true;
            mail.To.Add(new MailAddress("abhishektomar@cdnsol.com")); //(new MailAddress(_ediEmailSetting.ToAddress));
            mail.Attachments.Add(new Attachment(attachmentName));
            // Smtp client
            var client = new SmtpClient
            {
                Port = Convert.ToInt32(_ediEmailSetting.Port, CultureInfo.CurrentCulture),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = _ediEmailSetting.Host,
                EnableSsl = true,
                Credentials = credentials
            };
            try
            {
                await client.SendMailAsync(mail).ConfigureAwait(false);
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            }
            catch (SmtpFailedRecipientException ex)
            {
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
            }
            finally
            {
                client.Dispose();
                mail.Dispose();
            }
            return mail.DeliveryNotificationOptions;
        }

        public async Task<bool> SendReportMail(List<ReportMailUserModel> userList, byte[] pdf,string mailBody = null ,string reportName = null)
        {
            string subject, body;
            SendEmailViewModel sendEmailsViewModel = new SendEmailViewModel();
            Uri resetUrl = _appsetting.ResetUrl;

            var repository = _unitOfWork?.GetRepository<EmailTemplate>();

            if (userList?.Count > 0)
            {

                //var emailTemplate =  get data from scheduler
                body = $"{mailBody} </br>Please go through attachment for requested Reports.";
                subject = "Coyote Scheduled Report";

                if (pdf == null)
                {
                    body = $"{mailBody} </br></br>No reports were generated for above selections.";
                }

                // Mail message
                var mail = new MailMessage
                {
                    From = new MailAddress(_appsetting.FromEmail),
                    Subject = subject,
                    Body = body
                };
                mail.IsBodyHtml = true;

                if (pdf != null)
                {
                    mail.Attachments.Add(new Attachment(new MemoryStream(pdf), "Report.pdf"));
                }
               


                foreach (var user in userList)
                {
                    mail.To.Add(new MailAddress(user.Email));
                }

                // Credentials
                var credentials = new NetworkCredential(_appsetting.FromEmail, _appsetting.Password);

                // Smtp client
                var client = new SmtpClient
                {
                    Port = Convert.ToInt32(_appsetting.SMTPPort, CultureInfo.CurrentCulture),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _appsetting.Host,
                    Credentials = credentials,
                    EnableSsl = false
                };


                try
                {
                    await client.SendMailAsync(mail).ConfigureAwait(false);
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

                }
                catch (SmtpFailedRecipientException ex)
                {
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
                    //Email Send failed, log it in table, try again in 5 min-next scheduler run
                }
                finally
                {
                    client.Dispose();
                    mail.Dispose();
                }


            }
            return true;

        }


        public async Task<bool> SendReportFailMail(SendEmailViewModel mailModel)
        {
            string subject, body;
            SendEmailViewModel sendEmailsViewModel = new SendEmailViewModel();
            Uri resetUrl = _appsetting.ResetUrl;

            if (mailModel != null)
            {
                body = mailModel.MsgBody;
                subject = mailModel.EmailSubject;

                // Mail message
                var mail = new MailMessage
                {
                    From = new MailAddress(_appsetting.FromEmail),
                    Subject = subject,
                    Body = body
                };
                mail.IsBodyHtml = true;

                mail.To.Add(new MailAddress(mailModel.ToAddress));

                // Credentials
                var credentials = new NetworkCredential(_appsetting.FromEmail, _appsetting.Password);

                // Smtp client
                var client = new SmtpClient
                {
                    Port = Convert.ToInt32(_appsetting.SMTPPort, CultureInfo.CurrentCulture),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _appsetting.Host,
                    Credentials = credentials,
                    EnableSsl = true
                };


                try
                {
                    await client.SendMailAsync(mail).ConfigureAwait(false);
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

                }
                catch (SmtpFailedRecipientException ex)
                {
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
                    //Email Send failed, log it in table, try again in 5 min-next scheduler run
                }
                finally
                {
                    client.Dispose();
                    mail.Dispose();
                }
            }
            return true;

        }


    }
}

