using System;
using System.Globalization;
using System.Net.Mail;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Coyote.Console.App.Services
{
    public class LoginService : ILoginService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IConfiguration _config = null;
        private ILoggerManager _iLoggerManager = null;
        private ISendMailService _iSendEmailService = null;
        private IConfiguration _configuration;
        public LoginService(IUnitOfWork unitOfWork, IConfiguration config, ILoggerManager iLoggerManager, ISendMailService iSendEmailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _iLoggerManager = iLoggerManager;
            _iSendEmailService = iSendEmailService;
            _configuration = configuration;
        }
 
        private async Task<bool> Update(Users viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Users>();
                    viewModel.UpdatedAt = DateTime.UtcNow;
                    //viewModel.UpdatedById = userId;
                    repository.Update(viewModel);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    return true;
                }
                throw new NullReferenceException(ErrorMessages.PasswordUpdateFailed.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    bool isExpired = false;
                    var repository = _unitOfWork?.GetRepository<Users>();
                    int passwordExpiryInMinute = int.Parse(_config.GetValue<string>("TempPassword:ExpiryTimeInMinute"), CultureInfo.CurrentCulture);

                    var user = await repository.GetAll(x => x.TemporaryPassword == viewModel.TempPassword && x.Id == viewModel.UserId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (user != null && user.TempPasswordCreatedAt.HasValue)
                    {
                        int minuteLeft = (DateTime.UtcNow - user.TempPasswordCreatedAt.Value).Minutes;
                        if (minuteLeft >= passwordExpiryInMinute)
                            isExpired = true;
                    }
                    if (isExpired)
                    {
                        throw new NotFoundException(ErrorMessages.PasswordExpired.ToString(CultureInfo.CurrentCulture));
                    }
                    var userExist = await repository.GetAll(x => x.TemporaryPassword == viewModel.TempPassword && x.Id == viewModel.UserId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (userExist == null)
                    {
                        throw new NullReferenceException(ErrorMessages.PasswordUpdateFailed.ToString(CultureInfo.CurrentCulture));
                    }
                    userExist.IsResetPassword = false;
                    userExist.TemporaryPassword = null;
                    userExist.TempPasswordCreatedAt = null;
                    userExist.Password = EncryptDecryptAlgorithm.EncryptString(viewModel.NewPassword);

                    return await Update(userExist).ConfigureAwait(false);
                }
                throw new NullReferenceException(ErrorMessages.PasswordUpdateFailed.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        public async Task<bool> ChangePasswrod(ResetPasswordViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Users>();
                    var userExist = await repository.GetAll(x => x.PlainPassword == viewModel.TempPassword && x.Id == viewModel.UserId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (userExist == null)
                    {
                        throw new NullReferenceException(ErrorMessages.PasswordUpdateFailed.ToString(CultureInfo.CurrentCulture));
                    }
                    userExist.PlainPassword = viewModel.NewPassword;
                    userExist.Password = EncryptDecryptAlgorithm.EncryptString(viewModel.NewPassword);

                    return await Update(userExist).ConfigureAwait(false);
                }
                throw new NullReferenceException(ErrorMessages.PasswordUpdateFailed.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<int> ForgotPassword(string useremail)
        {
            try
            {
                var result = 0;
                bool isForgotPassword = true;
                var repository = _unitOfWork?.GetRepository<Users>();
                var userExist  = await repository.GetAll(x => x.Email == useremail).FirstOrDefaultAsync().ConfigureAwait(false);
                if (userExist == null)
                {
                    throw new NullReferenceException(ErrorMessages.UserNotExistWIthEmail.ToString(CultureInfo.CurrentCulture));
                }

                string tempPassword = Guid.NewGuid().ToString();
                userExist.TemporaryPassword = tempPassword;
                userExist.IsResetPassword = true;
                userExist.TempPasswordCreatedAt = DateTime.UtcNow;
                DeliveryNotificationOptions mailStatus = await _iSendEmailService.SendMail(userExist, isForgotPassword).ConfigureAwait(false);

                if (DeliveryNotificationOptions.OnSuccess != mailStatus)
                {
                    throw new BadRequestException(ErrorMessages.UnableToSendMail.ToString(CultureInfo.CurrentCulture));
                }
                await Update(userExist).ConfigureAwait(false);

                result = userExist.Id;
                return result;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public  int GetMigrationHistory()
        {
            try 
            { 
            int migrationcount =0 ;
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection sqlConn = new SqlConnection(_configuration["ConnectionStrings:DBConnection"]))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("select * from __EFMigrationsHistory order by migrationid desc", sqlConn))
                    {
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                           
                                    migrationcount = dt.Rows.Count;
                            
                        }
                    }
                }

            }
            return migrationcount;

            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }
    }
}
