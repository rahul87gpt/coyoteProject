using Coyote.Console.App.Models;
using Coyote.Console.App.Repository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{

    /// <summary>
    /// UserController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _iUserService = null;
        private readonly IUserLoggerServices _iUserLogService = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ISendMailService _SendEmailService = null;
        private readonly IImageUploadHelper _iImageUploader = null;
        private readonly ILoggerManager _iLogger = null;
        /// <summary>
        /// UserController
        /// </summary>
        /// <param name="iUserService"></param>
        /// <param name="iAutoMapper"></param>
        /// <param name="iSendEmailService"></param>
        /// <param name="iImageHelper"></param>
        /// <param name="logger"></param>
        /// <param name="userLogger"></param>
        public UserController(IUserServices iUserService, IAutoMappingServices iAutoMapper, ISendMailService iSendEmailService, IImageUploadHelper iImageHelper, ILoggerManager logger, IUserLoggerServices userLogger)
        {
            _iLogger = logger;
            _iUserService = iUserService;
            _iAutoMapper = iAutoMapper;
            _SendEmailService = iSendEmailService;
            _iImageUploader = iImageHelper;
            _iUserLogService = userLogger;
        }

        // GET: api/user
        /// <summary>
        ///     Get all users with the is_Deleted flag is Zero
        /// </summary>
        /// <returns>List of User View Models </returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserSavedResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                var users = await _iUserService.GetAllActiveUsers(filter).ConfigureAwait(false);
                if (users.TotalCount == 0)
                {
                    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.UserDetailNotFound));
                }
                return Ok(users);
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
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // GET: api/user/{id}
        /// <summary>
        /// Get the detail of a user by its Id
        /// <param name="id">User id</param>
        /// </summary>
        /// <returns>User view model with specified Id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserSavedResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var users = await _iUserService.GetUserById(id).ConfigureAwait(false);
                if (users == null)
                {
                    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.UserNotFound));
                }
                return Ok(users);
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
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // POST: api/user
        /// <summary>
        ///   Add User into the database
        /// </summary>
        /// <param name="userViewModel">User viewModel</param>
        /// <returns>Added new User View Model</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserSavedResponseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] UserRequestModel userViewModel)
        {
            try
            {
                if (ModelState.IsValid && userViewModel != null)
                {
                    var result = await _iUserService.Insert(userViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    userViewModel.Id = result;
                    UserSavedResponseViewModel savedResponseViewModel = _iAutoMapper.Mapping<UserRequestModel, UserSavedResponseViewModel>(userViewModel);
                    if (userViewModel.Image != null && userViewModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            if (string.IsNullOrEmpty(userViewModel.ImageName))
                            {
                                userViewModel.ImageName = userViewModel.UserName;
                            }
                            string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;

                            imagePath = await _iImageUploader.UploadImage(userViewModel.Image, userViewModel.ImageName, result.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);

                            //to save path update record
                            await _iUserService.Update(userViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), imagePath).ConfigureAwait(false);

                            savedResponseViewModel.ImageUploadStatusCode = "Ok";
                            savedResponseViewModel.ImagePath = imagePath;
                        }
                        catch (Exception ex)
                        {
                            _iImageUploader.WriteErrorLog(ex);
                            savedResponseViewModel.ImageUploadStatusCode = ex.Message;
                        }
                        finally
                        {
                            userViewModel.Image = null;
                            savedResponseViewModel.Image = null;
                        }
                    }
                    return new ObjectResult(savedResponseViewModel) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (AlreadyExistsException br)
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
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        // PUT: api/User/5
        /// <summary>
        /// Update User by user_Id into the database
        /// </summary>
        /// <param name="userViewModel">User View Model</param>
        /// <param name="id">User Id</param>
        /// <returns>Updated Userview Model with response</returns>
        [ProducesResponseType(typeof(UserSavedResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] UserRequestModel userViewModel, int id)
        {
            try
            {
                if (!ModelState.IsValid || id == 0 || userViewModel == null)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.UserIdNotFound));
                };
                string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;
                if (userViewModel.Image != null && userViewModel.Image.Length > 0)
                {
                    //not throwing exception if image upload fails
                    try
                    {
                        if (string.IsNullOrEmpty(userViewModel.ImageName))
                        {
                            userViewModel.ImageName = userViewModel.UserName;
                        }

                        imagePath = await _iImageUploader.UploadImage(userViewModel.Image, userViewModel.ImageName, userViewModel.Id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                        imageUploadStatusCode = "Ok";
                    }
                    catch (Exception ex)
                    {
                        imageUploadStatusCode = ex.Message;
                    }
                    finally
                    {
                        userViewModel.Image = null;
                    }
                }
                if (userViewModel.Id == 0)
                {
                    userViewModel.Id = id;
                }
                await _iUserService.Update(userViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), imagePath).ConfigureAwait(false);
                UserSavedResponseViewModel savedResponseViewModel = _iAutoMapper.Mapping<UserRequestModel, UserSavedResponseViewModel>(userViewModel);
                savedResponseViewModel.ImagePath = imagePath;
                savedResponseViewModel.ImageUploadStatusCode = imageUploadStatusCode;
                return Ok(savedResponseViewModel);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (AlreadyExistsException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // DELETE: api/ApiWithActions/5
        /// <summary>
        ///     Delete user by Id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Success as response result</returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    await _iUserService.DeleteUser(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return NoContent();
                }
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.UserNotFound));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get User Activity Log
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet("UserActivity")]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserActivityResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserActivity([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var activity = await _iUserLogService.GetUserActivity(inputModel).ConfigureAwait(false);
                if (activity == null)
                {
                    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.UserNotFound));
                }
                return Ok(activity);
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
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get User By Access Store
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet("UserByAccess")]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserActivityResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserByAccess([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var activity = await _iUserService.GetUserByAccessStores(inputModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                if (activity == null)
                {
                    return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.UserNotFound));
                }
                return Ok(activity);
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
                _iLogger.LogError("UserController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}
