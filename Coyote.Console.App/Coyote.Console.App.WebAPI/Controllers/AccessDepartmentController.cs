using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// AccessDepartmentController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    
    public class AccessDepartmentController : Controller
    {


        private readonly IAccessDepartmentServices _iAccessDepartmentService = null;
        private readonly ILoggerManager _iLogger = null;


        /// <summary>
        /// Initialize the  IWarehouseRepositry,IAutoMappingServices
        /// </summary>
        /// <param name="iAccessDepartmentService"></param>
        /// <param name="iLogger"></param>
        public AccessDepartmentController(IAccessDepartmentServices iAccessDepartmentService, ILoggerManager iLogger)
        {
            _iAccessDepartmentService = iAccessDepartmentService;
            _iLogger = iLogger;
        }
        //Get: api/Access
        /// <summary>
        /// Get all Active AccessDepartment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<AccessDepartmentViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAccessDepartment([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                PagedOutputModel<List<AccessDepartmentViewModel>> apn = await _iAccessDepartmentService.GetAllAccessDepartment(inputModel).ConfigureAwait(false);

                return Ok(apn);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("AccessDepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        // GET: api/AccessDepartment/{id}
        /// <summary>
        /// Get the detail of a AccessDepartment by its Id
        /// <param name="id">Role id</param>
        /// </summary>
        /// <returns>Role view model with specified Id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccessDepartmentViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iAccessDepartmentService.GetAccessDepartmentById(id).ConfigureAwait(false));
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
                _iLogger.LogError("AccessDepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }




        // POST: api/AccessDepartment
        /// <summary>
        ///   Add Role into the database
        /// </summary>
        /// <param name="AccessDept">AccessDepartment viewModel</param>
        /// <returns>Added new AccessDepartment View Model</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AccessDepartmentViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] AccessDepartmentViewModel AccessDept)
        {
            try
            {
                if (ModelState.IsValid && AccessDept != null)
                {
                    var result = await _iAccessDepartmentService.Insert(AccessDept, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    AccessDept.Id = result;

                    // Return 201
                    return new ObjectResult(AccessDept) { StatusCode = StatusCodes.Status201Created };
                }
                return Conflict(ModelState);
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
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


        // Delete: api/AccessDepartment/{id}
        /// <summary>
        ///     Delete AccessDepartmentV by Id
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <returns>Success as response result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(AccessDepartmentViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iAccessDepartmentService.DeleteAccessDepartment(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
