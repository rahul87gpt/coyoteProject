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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// PathsController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class PathsController : ControllerBase
    {
        private readonly IPathsService _iservices = null;
        private readonly ILoggerManager _iLogger = null;
        private IImageUploadHelper _iFileUploader = null;

        /// <summary>
        /// Path Controller
        /// </summary>
        /// <param name="iservices"></param>
        /// <param name="logger"></param>
        /// <param name="iFileUploader"></param>
        public PathsController(IPathsService iservices, ILoggerManager logger, IImageUploadHelper iFileUploader)
        {
            _iLogger = logger;
            _iservices = iservices;
            _iFileUploader = iFileUploader;
        }

        /// <summary>
        /// Get the path details
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<PathsResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await _iservices.GetAllPaths(inputModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("PathsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get the path details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PathsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iservices.GetPathById(id).ConfigureAwait(false));
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
                _iLogger.LogError("PathsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Add the path details
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<PathsResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromForm] PathsRequestModel requestModel)
        {
            try
            {

                if (ModelState.IsValid && requestModel != null)
                {
                    if (requestModel.File == null || requestModel.File.Length < 0)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidPathFile);
                    }

                    string filePath = string.Empty;
                    try
                    {

                        filePath = await _iFileUploader.UploadFileViaPath(requestModel.File).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new BadRequestException(ex.Message);
                    }

                    var result = await _iservices.InsertPath(requestModel, filePath, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

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
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("PathsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Edit path details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PathsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] PathsRequestModel requestModel)
        {
            try
            {
                if (!ModelState.IsValid || id == 0 || requestModel == null)
                {
                    return BadRequest(ModelState);
                }

                string filePath = string.Empty;
                try
                {

                    filePath = await _iFileUploader.UploadFileViaPath(requestModel.File).ConfigureAwait(false);
                    return Ok(await _iservices.UpdatePath(requestModel, id, filePath, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                }
                catch (Exception ex)
                {
                    throw new BadRequestException(ex.Message);
                }


            }
            catch (AlreadyExistsException br)
            {
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (BadRequestException br)
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
                _iLogger.LogError("PathsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete the path details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return Ok(await _iservices.DeletePath(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("PathsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}

