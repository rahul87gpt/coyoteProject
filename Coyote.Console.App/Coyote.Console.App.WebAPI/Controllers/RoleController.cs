using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
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
    /// RoleController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _iRoleService = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLogger = null;
        /// <summary>
        /// RoleController
        /// </summary>
        /// <param name="iRoleService"></param>
        /// <param name="iAutoMapper"></param>
        /// <param name="logger"></param>
        public RoleController(IRoleService iRoleService, IAutoMappingServices iAutoMapper, ILoggerManager logger)
        {
            _iLogger = logger;
            _iRoleService = iRoleService;
            _iAutoMapper = iAutoMapper;
        }

        // GET: api/Roles
        /// <summary>
        ///     Get all roles with the is_Deleted flag is Zero
        /// </summary>
        /// <returns>List of Role View Models </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RolesViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                PagedOutputModel<List<RolesViewModel>> roles = await _iRoleService.GetAllActiveRoles(inputModel).ConfigureAwait(false);

                return Ok(roles);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("RoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        // GET: api/Roles/{id}
        /// <summary>
        /// Get the detail of a role by its Id
        /// <param name="id">Role id</param>
        /// </summary>
        /// <returns>Role view model with specified Id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolesViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iRoleService.GetRoleById(id).ConfigureAwait(false));
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
                _iLogger.LogError("RoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        // POST: api/Roles
        /// <summary>
        ///   Add Role into the database
        /// </summary>
        /// <param name="roles">Role viewModel</param>
        /// <returns>Added new Role View Model</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RolesViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] RolesViewModel roles)
        {
            try
            {
                if (ModelState.IsValid && roles != null)
                {
                    var result = await _iRoleService.Insert(roles, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    roles.Id = result;

                    // Return 201
                    return new ObjectResult(roles) { StatusCode = StatusCodes.Status201Created };
                }
                return Conflict(ModelState);
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("RoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        // PUT: api/Roles/{id}
        /// <summary>
        /// Update Role by role_Id into the database
        /// </summary>
        /// <param name="roles">Role View Model</param>
        /// <param name="id">role Id</param>
        /// <returns>Updated Role View model with response</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RolesViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Put([FromBody]  RolesViewModel roles, int id)
        {
            try
            {
                if (!ModelState.IsValid || id == 0 || roles == null)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.RoleIdNotFound));
                }
                roles.Id = id;
                await _iRoleService.Update(roles, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return Ok(roles);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
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
                _iLogger.LogError("RoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }


        // Delete: api/Roles/{id}
        /// <summary>
        ///     Delete role by Id
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <returns>Success as response result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(RolesViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iRoleService.DeleteRole(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();

            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("RoleController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }
    }
}
