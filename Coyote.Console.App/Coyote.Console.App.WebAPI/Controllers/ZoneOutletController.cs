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
    /// ZoneOutletController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class ZoneOutletController : Controller
    {
        private readonly IZoneOutletServices _iZoneOutletService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// ZoneOutletController
        /// </summary>
        /// <param name="iZoneOutletService"></param>
        /// <param name="logger"></param>
        public ZoneOutletController(IZoneOutletServices iZoneOutletService, ILoggerManager logger)
        {
            _iLogger = logger;
            this._iZoneOutletService = iZoneOutletService;
        }


        /// <summary>
        /// Get all active Zone Outlets.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ZoneOutletGetAllResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] ZoneOutletInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iZoneOutletService.GetActiveZoneOutlet(inputModel, securityViewModel).ConfigureAwait(false));
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoZoneOutletFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ZoneOutletController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get Zone Outlet with the given Id
        /// </summary>
        /// <param name="id">Zone Outlet Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ZoneOutletResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetZoneOutletById(int id)
        {
            try
            {
                return Ok(await _iZoneOutletService.GetZoneOutletById(id).ConfigureAwait(false));
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoZoneOutletFound));
            }
            catch (NullReferenceException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ZoneOutletId));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ZoneOutletController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Add a new Zone Outlet
        /// </summary>
        /// <param name="zoneOutletModel">ZoneOutletViewModel</param>
        /// <returns>ZoneOutletViewModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ZoneOutletResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddZoneOutlet([FromBody] ZoneOutletRequestModel zoneOutletModel)
        {
            try
            {
                if (ModelState.IsValid && zoneOutletModel != null)
                {
                    var result = await _iZoneOutletService.Insert(zoneOutletModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };

                }
                return BadRequest(ModelState);
            }
            catch (NullReferenceException nre)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (AlreadyExistsException nre)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ZoneOutletController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }


        /// <summary>
        /// Update the existing Zone Outlet.
        /// </summary>
        /// <param name="zoneOutletModel">ZoneOutletViewModel</param>
        /// <returns>ZoneOutletViewModel</returns>
        [HttpPut()]
        [ProducesResponseType(typeof(ZoneOutletResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateZoneOutlet([FromBody]  ZoneOutletRequestModel zoneOutletModel)
        {
            try
            {
                if (ModelState.IsValid && zoneOutletModel != null)
                {
                    var result = await _iZoneOutletService.Update(zoneOutletModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ZoneOutletNotFound));
            }
            catch (AlreadyExistsException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ZoneOutletNotFound));
            }
            catch (NullReferenceException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ZoneOutletId));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ZoneOutletController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }


        /// <summary>
        /// To delete a Zone Outlet.
        /// </summary>
        /// <param name="id">Zone Outlet Id</param>
        /// <returns>HttpResponseErrorModel</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteZoneOutlet(int id)
        {
            try
            {
                await _iZoneOutletService.DeleteZoneOutlet(id, 1).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ZoneOutletNotFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ZoneOutletController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        [HttpGet("store/{id}")]
        [ProducesResponseType(typeof(ZoneOutletResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public async Task<IActionResult> GetZoneOutletByStoreId(int id)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                return Ok(await _iZoneOutletService.GetZoneOutletByStoreId(id).ConfigureAwait(false));
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoZoneOutletFound));
            }
            catch (NullReferenceException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ZoneOutletId));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ZoneOutletController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}