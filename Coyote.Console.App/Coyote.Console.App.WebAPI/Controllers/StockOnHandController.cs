using System;
using System.Collections.Generic;
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
    /// StockOnHandController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StockOnHandController : ControllerBase
    {
        private IStockOnHandService _istock = null;
        private ILoggerManager _iLogger = null;
        /// <summary>
        /// StockOnHandController
        /// </summary>
        /// <param name="handService"></param>
        /// <param name="logger"></param>
        public StockOnHandController(IStockOnHandService handService, ILoggerManager logger)
        {
            _iLogger = logger;
            _istock = handService;
        }

        /// <summary>
        /// Get Details
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<StockProductResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] StockProductsRequestModel inputModel)
        {
            try
            {
                return Ok(await _istock.GetStockOnHand(inputModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StockOnHandController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}