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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// ReportScheduler
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class ReportScheduler : Controller
    {

        private readonly IReportSchedulerServices _rtScheduler = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// /Report Scheduler
        /// </summary>
        /// <param name="ireportScheduler"></param>
        /// <param name="logger"></param>
        public ReportScheduler(IReportSchedulerServices ireportScheduler, ILoggerManager logger)
        {
            _iLogger = logger;
            _rtScheduler = ireportScheduler;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ReportSchedulerResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public async Task<IActionResult> Get([FromQuery] PagedInputModel filter)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {

                var result = await _rtScheduler.GetAllScheduler(filter).ConfigureAwait(false);
                return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
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
                _iLogger.LogError("ReportScheduler" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Scheduler by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReportSchedulerResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                if (id > 0)
                {
                    var result = await _rtScheduler.GetSchedulerById(id).ConfigureAwait(false);
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("ReportScheduler" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Report Scheduler
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ReportSchedulerResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] ReportSchedulerResponseModel scheduler)
        {
            try
            {
                if (ModelState.IsValid && scheduler != null)
                {
                    var result = await _rtScheduler.InsertReportSchedulers(scheduler, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
                _iLogger.LogError("ReportScheduler" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Report Scheduler
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ReportSchedulerResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public async Task<IActionResult> Put(int id, [FromBody] ReportSchedulerResponseModel scheduler)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            try
            {
                if (ModelState.IsValid && scheduler != null)
                {
                    var result = await _rtScheduler.UpdateReportSchedulers(scheduler, id, 1).ConfigureAwait(false);
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
                _iLogger.LogError("ReportScheduler" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Report Scheduler
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ReportSchedulerResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public async Task<IActionResult> Delete(int id)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            try
            {
                if (id>0)
                {
                    var result = await _rtScheduler.DeleteScheduler(id, 1).ConfigureAwait(false);
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
                _iLogger.LogError("ReportScheduler" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


    }
}
