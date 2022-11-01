using Coyote.Console.App.Models;
using Coyote.Console.App.Repository;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// ILoginHelper
    /// </summary>
    public interface ILoginHelper
    {
        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<LoginViewModel> Authenticate(LoginRequestViewModel user);
        /// <summary>
        /// IsValidEmail
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        bool IsValidEmail(string emailAddress);
        /// <summary>
        /// AuthenticateRefreshToken
        /// </summary>
        /// <param name="revokeToken"></param>
        /// <returns></returns>
        Task<LoginViewModel> AuthenticateRefreshToken(RevokeTokenRequestModel revokeToken);
    }

    /// <summary>
    /// LoginHelper
    /// </summary>
    public class LoginHelper : ILoginHelper
    {
        //private IConfiguration _configuration;
        private readonly IUserRepository _repository = null;
        private readonly AppSettings _appSettings;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// LoginHelper
        /// </summary>
        /// <param name="appsetting"></param>
        /// <param name="repository"></param>
        /// <param name="roleService"></param>
          /// <param name="unitOfWork"></param>
        public LoginHelper(IOptions<AppSettings> appsetting, IUserRepository repository, IRoleService roleService, IUnitOfWork unitOfWork)
        {
            _appSettings = appsetting?.Value;
            _repository = repository;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public async Task<LoginViewModel> Authenticate(LoginRequestViewModel userLogin)
        {
            var responseModel = new LoginViewModel();
            // return null 
            if (userLogin == null)
            {
                return responseModel;
            }

            
            var seretKey = _appSettings.Secret;
            var passwordEncypted = EncryptDecryptAlgorithm.EncryptString(userLogin.Password);
            var user = await _repository.GetAll(x => (x.Email == userLogin.UserEmail || x.UserName == userLogin.UserEmail) && x.Password == passwordEncypted && !x.IsDeleted).SingleOrDefaultAsync().ConfigureAwait(false);
            // return null if user not found
            if (user == null)
            {
                //check plain password //to be removed in near future
                user = await _repository.GetAll(x => (x.Email == userLogin.UserEmail || x.UserName == userLogin.UserEmail) && x.PlainPassword == userLogin.Password && !x.IsDeleted).SingleOrDefaultAsync().ConfigureAwait(false);
                if (user == null)
                {
                    return responseModel;
                }
            }
            var userRole = _repository.GetUserRole(user.Email) ?? DBRoles.User.ToString(CultureInfo.CurrentCulture);

            //cc-  //Need to send roles and isdefault 
            responseModel.Roles = _repository.GetUserRoles(user.Id);
            responseModel.DefaultRoleId = responseModel.Roles.Where(x => x.IsDefualt).Select(x => x.Id).FirstOrDefault();
            responseModel.DefaultRolePermissions = responseModel.Roles.Where(x => x.IsDefualt).Select(x => x.Permissions).FirstOrDefault();

            // authentication successful so generate jwt token
            responseModel.Token = GenerateToken(user, userRole, responseModel.DefaultRoleId);
            responseModel.RefreshToken = GenerateRefreshToken(user.Id);
            responseModel.TokenTimeOut = _appSettings.TokenExpiryMinute;
            responseModel.TokenExpiration = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiryMinute);
            responseModel.FirstLogin = user.LastLogin != null ? false : true;
            responseModel.UserId = user.Id;
            responseModel.Password = user.PlainPassword;
            responseModel.UserName = user.UserName;
            responseModel.UserEmail = user.Email;
            responseModel.FirstName = user.FirstName;
            responseModel.LastName = user.LastName;
            //responseModel.Image = user.ImagePath;      

            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                Byte[] imageBytes;
                string imageFolderPath = Directory.GetCurrentDirectory() + user.ImagePath;
                if (File.Exists(imageFolderPath))
                {
                    imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                    responseModel.Image = imageBytes;
                }
            }

            //Get DB name 
            
            var dbDetails = _repository.GetDBName();
            responseModel.Database = dbDetails.Database;
            responseModel.DataSource = dbDetails.DataSource;

           //migration history 
         
            //responseModel.MigrationName = dp.MigrationId;

#pragma warning disable CS0618 // Type or member is obsolete
            TimeZoneInfo localZone = TimeZoneInfo.Local;
#pragma warning restore CS0618 // Type or member is obsolete

            DateTimeOffset localServerTime = DateTimeOffset.Now;

            DateTimeOffset usersTime = TimeZoneInfo.ConvertTime(localServerTime, localZone);

            DateTimeOffset utc = usersTime.ToUniversalTime();



#pragma warning disable CS0618 // Type or member is obsolete
        //    DateTime curUTC = localZone.u(DateTime.Now);
#pragma warning restore CS0618 // Type or member is obsolete

            user.LastLogin = utc.LocalDateTime;
            user.RefreshToken = responseModel.RefreshToken;
            user.UpdatedAt = DateTime.Now;
            //Detaching tracked entry - exists
            _repository.DetachLocal(_ => _.Id == user.Id);
            _repository.Update(user);
            await (_repository?.SaveChangesAsync()).ConfigureAwait(false);

            //Log in UserLog UserActivity LogIn

            return responseModel;
        }
        /// <summary>
        /// AuthenticateRefreshToken
        /// </summary>
        /// <param name="revokeToken"></param>
        /// <returns></returns>
        public async Task<LoginViewModel> AuthenticateRefreshToken(RevokeTokenRequestModel revokeToken)
        {
            if (string.IsNullOrEmpty(revokeToken?.RefreshToken))
            {
                throw new BadRequestException();
            }
            var seretKey = _appSettings.Secret;

            var user = await _repository.GetAll(x => x.Id == revokeToken.userId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

            if (user != null)
            {
                if (user.RefreshToken == revokeToken.RefreshToken)
                {
                    var userRole = _repository.GetUserRole(user.Email) ?? DBRoles.User.ToString(CultureInfo.CurrentCulture);

                    var userLogin = new LoginViewModel();

                    //cc-  //Need to send roles and isdefault 
                    userLogin.Roles = _repository.GetUserRoles(user.Id);
                    userLogin.DefaultRoleId = userLogin.Roles.Where(x => x.IsDefualt).Select(x => x.Id).FirstOrDefault();
                    userLogin.DefaultRolePermissions = userLogin.Roles.Where(x => x.IsDefualt).Select(x => x.Permissions).FirstOrDefault();

                    // authentication successful so generate jwt token
                    userLogin.Token = GenerateToken(user, userRole, userLogin.DefaultRoleId);
                    userLogin.RefreshToken = GenerateRefreshToken(revokeToken.userId);
                    userLogin.TokenExpiration = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiryMinute);
                    userLogin.TokenTimeOut = _appSettings.TokenExpiryMinute;
                    userLogin.FirstLogin = user.LastLogin != null ? false : true;
                    userLogin.UserId = user.Id;
                    userLogin.Password = user.PlainPassword;
                    userLogin.UserName = user.UserName;
                    userLogin.UserEmail = user.Email;

                    if (!string.IsNullOrEmpty(user.ImagePath))
                    {
                        Byte[] imageBytes;
                        string imageFolderPath = Directory.GetCurrentDirectory() + user.ImagePath;
                        if (File.Exists(imageFolderPath))
                        {
                            imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                            userLogin.Image = imageBytes;
                        }
                    }

                    user.LastLogin = DateTime.UtcNow;
                    user.RefreshToken = userLogin.RefreshToken;
                    //Detaching tracked entry - exists
                    _repository.DetachLocal(_ => _.Id == user.Id);
                    _repository.Update(user);
                    await _repository.SaveChangesAsync().ConfigureAwait(false);
                    return userLogin;
                }
            }

            //var user = _repository.GetAll(x => x.RefreshToken == revokeToken.RefreshToken && x.Id ==revokeToken.userId && !x.IsDeleted).FirstOrDefault();
            //// return null if user not found
            //if (user == null)
            //{
            //    throw new SecurityTokenException(ErrorMessages.InValidRefreshTokens.ToString(CultureInfo.CurrentCulture));
            //}

            throw new SecurityTokenException(ErrorMessages.InValidRefreshTokens.ToString(CultureInfo.CurrentCulture));

        }

        /// <summary>
        /// IsValidEmail
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        private string GenerateToken(Users user, string userRole, int roleId)
        {
            var seretKey = _appSettings.Secret;
            var jwtIssuer = _appSettings.Issuer;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(seretKey);
            var issuer = jwtIssuer;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(CustomClaimTypes.Id, user.Id.ToString(CultureInfo.CurrentCulture)),
                    new Claim(CustomClaimTypes.UserEmail, user.Email),
                    new Claim(CustomClaimTypes.AddUnlockProduct, user.AddUnlockProduct.ToString()),
                    new Claim(ClaimTypes.Role,userRole),
                    new Claim(CustomClaimTypes.RoleId,roleId.ToString(CultureInfo.CurrentCulture)),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiryMinute).ToString(CultureInfo.CurrentCulture))
                }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiryMinute),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var Token = tokenHandler.WriteToken(token);
            return Token;

        }

        private static string GenerateRefreshToken(int userId)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                if (userId > 0)
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
            return "";
        }
    }
}

