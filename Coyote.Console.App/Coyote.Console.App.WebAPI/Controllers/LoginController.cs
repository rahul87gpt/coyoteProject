using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// LoginController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _service = null;
        private readonly ILoginHelper _login;
        private readonly ISendMailService _SendEmailService = null;
        private readonly ILoggerManager _iLogger = null;


        /// <summary>
        /// LoginController
        /// </summary>
        /// <param name="loginHelper"></param>
        /// <param name="repository"></param>
        /// <param name="iSendEmailService"></param>
        /// <param name="logger"></param>
        public LoginController(ILoginHelper loginHelper, ILoginService repository, ISendMailService iSendEmailService,ILoggerManager logger)
        {
            _iLogger = logger;
            _login = loginHelper;
            _service = repository;
            _SendEmailService = iSendEmailService;

        }

        /// <summary>
        /// Provide login
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost(Name = "Login")]
        [ProducesResponseType(typeof(List<LoginViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.InValidEmailAddress));
            }
            //commented as UserEmail can have Username
            //bool isValidEmail = false;
            //if (!string.IsNullOrEmpty(loginViewModel.UserEmail))
            //{
            //    isValidEmail = _login.IsValidEmail(loginViewModel.UserEmail);
            //}
            //if (isValidEmail == false || string.IsNullOrEmpty(loginViewModel.UserEmail))
            //{
            //    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.InValidEmailAddress));
            //}
            if (string.IsNullOrEmpty(loginViewModel.Password))
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.InValidPassword));

            //LoginViewModel user = new LoginViewModel() { UserEmail = loginViewModel.UserEmail, Password = loginViewModel.Password };

            try
            {
                var response =await _login.Authenticate(loginViewModel).ConfigureAwait(false);
                if (string.IsNullOrEmpty(response.Token))
                {
                    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.LoginFailed));
                }

                response.MigrationName = _service.GetMigrationHistory().ToString();
                return Ok(response);

            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {

                return BadRequest(APIReponseBuilder.HandleResponse(ex.Message));
                // return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.InternalServerError));
            }
        }

        /// <summary>
        /// RevokeToken
        /// </summary>
        /// <param name="revokeToken"></param>
        /// <returns></returns>
        [HttpPost("RevokeToken")]
        [ProducesResponseType(typeof(List<LoginViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken([FromBody]RevokeTokenRequestModel revokeToken)
        {
            if (string.IsNullOrEmpty(revokeToken?.RefreshToken))
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.InValidRefreshTokens));
            }
            try
            {
                var response = await _login.AuthenticateRefreshToken(revokeToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(response.Token))
                {
                    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.InValidRefreshTokens));
                }
                return Ok(response);

            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("LoginController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // Post: api/Login/ResetPassword
        /// <summary>
        ///     Reset password by new password
        /// </summary>
        /// <param name="resetUser">ResetPasswordViewModel</param>
        /// <returns>Updated Password </returns>
        [HttpPost("ResetPassword")]
        [ProducesResponseType(typeof(List<ResetPasswordViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel resetUser)
        {
            try
            {
                if (ModelState.IsValid && resetUser != null)
                {
                    if (resetUser.NewPassword != resetUser.ConfirmPassword)
                    {

                        return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ConfirmPassNotMatch));
                    }

                    await _service.ResetPassword(resetUser).ConfigureAwait(false);
                    return Ok(resetUser);
                }
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.PasswordUpdateFailed));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("LoginController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        // Post: api/Login/ForgotPassword
        /// <summary>
        ///     ForgotPassword will send Email to user.
        /// </summary>
        /// <param name="useremail">User Email id</param>
        /// <returns>Email id on which email has been send </returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string useremail)
        {
            try
            {
                bool isValidEmail = false;
                if (!string.IsNullOrEmpty(useremail))
                {
                    isValidEmail = _login.IsValidEmail(useremail);
                }
                if (isValidEmail == false || string.IsNullOrEmpty(useremail))
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.InValidEmailAddress));

               var Id= await _service.ForgotPassword(useremail).ConfigureAwait(false);

                return Ok(APIReponseBuilder.HandleResponse(ErrorMessages.MailSendSeccessfully));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("LoginController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // Post: api/Login/ResetPassword
        /// <summary>
        ///     Reset password by new password
        /// </summary>
        /// <param name="resetUser">ResetPasswordViewModel</param>
        /// <returns>Updated Password </returns>
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangePasswrod([FromBody] ResetPasswordViewModel resetUser)
        {
            try
            {
                if (ModelState.IsValid && resetUser != null)
                {
                    if (resetUser.NewPassword != resetUser.ConfirmPassword)
                    {

                        return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ConfirmPassNotMatch));
                    }

                    await _service.ChangePasswrod(resetUser).ConfigureAwait(false);
                    return Ok(APIReponseBuilder.HandleResponse(ErrorMessages.PasswordUpdated));
                }
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.PasswordUpdateFailed));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("LoginController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }
    }
}