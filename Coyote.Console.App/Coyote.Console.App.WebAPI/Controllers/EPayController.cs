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
    /// EPayController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class EPayController : ControllerBase
    {
        private readonly IEPayService _iservices = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// EPayController
        /// </summary>
        /// <param name="iservices"></param>
        /// <param name="logger"></param>
        public EPayController(IEPayService iservices, ILoggerManager logger)
        {
            _iLogger = logger;
            _iservices = iservices;
        }

        /// <summary>
        /// Get the EPay details
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<EPayResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await _iservices.GetAllEPay(inputModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("EPayController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get the EPay details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EPayResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iservices.GetEPayById(id).ConfigureAwait(false));
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
                _iLogger.LogError("EPayController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}
