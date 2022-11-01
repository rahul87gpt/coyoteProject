using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// API for anonymous usage ex. Digital Signage
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AnonymousController : ControllerBase
    {
        private readonly IStoreServices _iStoreService = null;
        private readonly ILoggerManager _iLogger = null;
        //private HttpResponseValidationResult<StoreViewModel> _objHttpResponseValidationResult = null;

        /// <summary>
        /// StoreController
        /// </summary>
        /// <param name="iStoreService"></param>
        /// <param name="logger"></param>
        public AnonymousController(IStoreServices iStoreService, ILoggerManager logger)
        {
            _iLogger = logger;
            this._iStoreService = iStoreService;
        }

        /// <summary>
        ///  Get all active stores
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("Stores")]
        [ProducesResponseType(typeof(PagedOutputModel<List<AnonymousStoreResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStores([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                var stores = await _iStoreService.GetActiveStoresForDigS(filter).ConfigureAwait(false);
                return Ok(stores);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StoreController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        ///  Get active store by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Store/{Id}")]
        [ProducesResponseType(typeof(PagedOutputModel<List<AnonymousStoreResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStoreyId(int Id)
        {
            try
            {
                var stores = await _iStoreService.GetActiveStoreByIdForDigS(Id).ConfigureAwait(false);
                return Ok(stores);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StoreController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}
