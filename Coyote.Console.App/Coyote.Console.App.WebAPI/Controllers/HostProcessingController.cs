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
    /// HostProcessingController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class HostProcessingController : ControllerBase
    {
        private readonly IHostProcessing _iservices = null;
        private readonly ILoggerManager _ilogger = null;

        /// <summary>
        /// HostProcessingController
        /// </summary>
        /// <param name="iservices">IHostProcessing</param>
        /// <param name="logger">ILoggerManager</param>
        public HostProcessingController(IHostProcessing iservices, ILoggerManager logger)
        {
            _iservices = iservices;
            _ilogger = logger;
        }

        /// <summary>
        /// Get the HostProcessing details
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns>HostProcessingResponseModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<HostProcessingResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await _iservices.GetAllHostProcessing(inputModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _ilogger.LogError("HostProcessingController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// AddUpdate the HostProcessing details
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>HostProcessingResponseModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<HostProcessingResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] HostProcessingRequestModel requestModel)
        {
            try
            {

                if (ModelState.IsValid && requestModel != null)
                {
                    var result = await _iservices.AddUpdateHostProcessing(requestModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

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
                _ilogger.LogError("HostProcessingController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get User Activity Log
        /// </summary>
        /// <param name="HostSettingId"></param>
        /// <returns></returns>
        [HttpGet("HostChangesSheet")]
        [ProducesResponseType(typeof(PagedOutputModel<List<HOSTUPDUserActivityResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HostChangesSheet(long HostSettingId)
        {
            try
            {
                var activity = await _iservices.HostChangesSheet(HostSettingId).ConfigureAwait(false);
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
                _ilogger.LogError("HostProcessingController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}
