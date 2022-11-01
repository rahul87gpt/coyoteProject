using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// UserRoleController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _iUserRoleService = null;
        private readonly IUserServices _iUserService = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ISendMailService _iSendMailService = null;
        private readonly ILoggerManager _iLogger = null;
        private readonly ILoginHelper _login;

        /// <summary>
        /// UserRoleController
        /// </summary>
        /// <param name="loginHelper"></param>
        /// <param name="iUserRoleService"></param>
        /// <param name="iUserService"></param>
        /// <param name="iAutoMapper"></param>
        /// <param name="iSendMailService"></param>
        /// <param name="logger"></param>
        public UserRoleController(ILoginHelper loginHelper,IUserRoleService iUserRoleService, IUserServices iUserService, IAutoMappingServices iAutoMapper, ISendMailService iSendMailService, ILoggerManager logger)
        {
            _iLogger = logger;
            _iUserRoleService = iUserRoleService;
            _iUserService = iUserService;
            _iAutoMapper = iAutoMapper;
            _iSendMailService = iSendMailService;
            _login = loginHelper;
        }

        // GET: api/Roles
        /// <summary>
        /// Get all user_roles with the is_Deleted flag is Zero
        /// </summary>
        /// <returns>List of User_Role View Models</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserRoleResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                return Ok(await _iUserRoleService.GetAllActiveUserRoles(filter).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        // GET: api/Roles
        /// <summary>
        /// Get user roles by userid
        /// </summary>
        /// <returns>List of User_Role View Models</returns>
        [HttpGet("Roles/{UserId}")]
        [ProducesResponseType(typeof(UserRoleListResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserIdAsync([FromRoute] int UserId)
        {
            try
            {
                var model = await _iUserRoleService.GetAllActiveUserRoles(null, UserId).ConfigureAwait(false);
                var rModel = new UserRoleListResponseModel
                {
                    Roles = model.Data.Select(x => x.Role).ToList(),
                    User = model.Data.Select(x => x.User).FirstOrDefault(),
                };
                return Ok(rModel);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        // GET: api/Roles/{id}
        /// <summary>
        /// Get the detail of a user role by its Id
        /// <param name="id">User role id</param>
        /// </summary>
        /// <returns>User role view model with specified Id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<UserRoleViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iUserRoleService.GetUserRoleById(id).ConfigureAwait(false));
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
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        // POST: api/Roles
        /// <summary>
        ///   Add User into the database
        /// </summary>
        /// <param name="userRolesViewModel">User role viewModel</param>
        /// <returns>Added new User role View Model</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserRoleViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] UserRoleViewModel userRolesViewModel)
        {
            try
            {
                if (ModelState.IsValid && userRolesViewModel != null)
                {
                    var result = await _iUserRoleService.Insert(userRolesViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    userRolesViewModel.Id = result;
                    return Ok(userRolesViewModel);
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        // POST: api/Roles
        /// <summary>
        ///   Add User into the database
        /// </summary>
        /// <param name="userRolesViewModel">User role viewModel</param>
        /// <returns>Added new User role View Model</returns>
        [HttpPost("Roles")]
        [ProducesResponseType(typeof(UserResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostRoles([FromBody] UserRolesRequestModel userRolesViewModel)
        {
            try
            {
                if (ModelState.IsValid && userRolesViewModel != null)
                {
                    var userViewModel = _iAutoMapper.Mapping<UserRolesRequestModel, UserRequestModel>(userRolesViewModel);
                    if (userViewModel.Id <= 0)
                    {
                        var result = await _iUserService.Insert(userViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                        if (result > 0)
                        {
                            userViewModel.Id = result;
                            var roles = new UserRolesPutRequestModel
                            {
                                Roles = userRolesViewModel.Roles,
                                DefaultRoleId = userRolesViewModel.DefaultRoleId
                            };
                            var rolesResult = await _iUserService.UpdateUserRoles(roles, userViewModel.Id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                            if (rolesResult)
                            {
                                return Ok(new UserResponseViewModel
                                {
                                    User = _iAutoMapper.Mapping<UserRequestModel,UserSavedResponseViewModel>(userViewModel),
                                    Roles = (await _iUserRoleService.GetAllActiveUserRoles(UserId: userViewModel.Id).ConfigureAwait(false)).Data.Select(x => x.Role).ToList()
                                });
                            }
                            else
                                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.InternalError));
                        }
                        return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.InternalError));
                    }
                    else
                    {
                        await _iUserService.Update(userViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                        return Ok(new UserResponseViewModel
                        {
                            User = _iAutoMapper.Mapping<UserRequestModel, UserSavedResponseViewModel>(userViewModel),
                            Roles = (await _iUserRoleService.GetAllActiveUserRoles(UserId: userViewModel.Id).ConfigureAwait(false)).Data.Select(x => x.Role).ToList()
                        });
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// PutRoles
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPut("Roles/{UserId}")]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserRoleResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutRoles([FromBody] UserRolesPutRequestModel viewModel, int UserId)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null && UserId > 0)
                {
                    await _iUserService.UpdateUserRoles(viewModel, UserId, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(await _iUserRoleService.GetAllActiveUserRoles(UserId: UserId).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SwitchRole
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <param name="revokeToken"></param>
        /// <returns></returns>
        [HttpPut("SwitchRole/{UserId:int}/{RoleId:int}")]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserRoleResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SwitchRole(int UserId, int RoleId,[FromBody] SwitchRoleRequestModel revokeToken)
        {
            try
            {
                if (UserId > 0 && RoleId>0)
                {
                    await _iUserService.SwitchDefaultRole(RoleId, UserId, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    //return Ok(await _iUserRoleService.GetAllActiveUserRoles(UserId: UserId).ConfigureAwait(false));
                    return Ok(await _login.AuthenticateRefreshToken(new RevokeTokenRequestModel {userId=UserId,RefreshToken= revokeToken?.RefreshToken }).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update User by user_Id into the database
        /// </summary>
        /// <param name="userRoleViewModel"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(UserRoleViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]  UserRoleViewModel userRoleViewModel)
        {
            try
            {
                if (ModelState.IsValid && userRoleViewModel != null)
                {
                    await _iUserRoleService.Update(userRoleViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(userRoleViewModel);
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }


        // DELETE: api/ApiWithActions/5
        /// <summary>
        ///     Delete user role by Id
        /// </summary>
        /// <param name="id">User role Id</param>
        /// <returns>Success as response result</returns>
        [ProducesResponseType(typeof(UserRoleViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iUserRoleService.DeleteUserRole(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.UserRoleNotFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("UserRoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }
    }
}