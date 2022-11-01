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
    /// DepartmentController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _iDepartmentService = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// DepartmentController
        /// </summary>
        /// <param name="iDepartment"></param>
        /// <param name="iAutoMapper"></param>
        /// <param name="logger"></param>
        public DepartmentController(IDepartmentService iDepartment, IAutoMappingServices iAutoMapper, ILoggerManager logger)
        {
            _iLogger = logger;
            _iDepartmentService = iDepartment;
            _iAutoMapper = iAutoMapper;
        }


        // GET: api/Department
        /// <summary>
        /// Get all Department with the is_Deleted flag is Zero
        /// </summary>
        /// <returns>List of Department View Models </returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<DepartmentResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                var department = await _iDepartmentService.GetActiveDepartments(filter).ConfigureAwait(false);
                return Ok(department);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("DepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));}

        }

        // GET: api/Department/{id}
        /// <summary>
        /// Get the detail of a Department by its Id
        /// <param name="id">Department id</param>
        /// </summary>
        /// <returns>Department view model with specified Id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var department = await _iDepartmentService.GetDepartmentById(id).ConfigureAwait(false);
                return Ok(department);
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
                _iLogger.LogError("DepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // POST: api/Department
        /// <summary>
        ///   Add Department into the database
        /// </summary>
        /// <param name="department">DepartmentViewModel</param>
        /// <returns>Added new Department View Model</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] DepartmentRequestModel department)
        {
            try
            {
                if (ModelState.IsValid && department != null)
                {
                    var result = await _iDepartmentService.Insert(department, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException br)
            {
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
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
                _iLogger.LogError("DepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // PUT: api/Department/{id}
        /// <summary>
        /// Update Department by dept_Id into the database
        /// </summary>
        /// <param name="department">Department View Model</param>
        /// <param name="id">Dept Id</param>
        /// <returns>Updated Department View model with response</returns>
        [ProducesResponseType(typeof(DepartmentResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]  DepartmentRequestModel department, int id)
        {
            try
            {
                if (!ModelState.IsValid || id == 0 || department == null)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.DepartmentIdNotFound));
                }
                var result = await _iDepartmentService.Update(department, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return Ok(result);
            }
            catch (AlreadyExistsException br)
            {
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
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
                _iLogger.LogError("DepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }


        // Delete: api/Department/{id}
        /// <summary>
        ///     Delete department by Id
        /// </summary>
        /// <param name="id">department Id</param>
        /// <returns>Success as response result</returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iDepartmentService.DeleteDepartment(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("DepartmentController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}
