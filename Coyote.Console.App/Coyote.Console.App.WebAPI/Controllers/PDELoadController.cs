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
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// PDELoadController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class PDELoadController : ControllerBase
    {
        private IUserLoggerServices _services;
        private readonly ILoggerManager _iLog = null;

        /// <summary>
        /// PDELoadController
        /// </summary>
        /// <param name="services"></param>
        /// <param name="logger"></param>
        public PDELoadController(IUserLoggerServices services, ILoggerManager logger)
        {
            _services = services;
            _iLog = logger;
        }

        /// <summary>
        /// PDE Load History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserLogResponseModel<PDELoadDataLogResponseModel>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await _services.GetPDELoadHistory().ConfigureAwait(false);
                return Ok(response);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLog.LogError("PDELoad" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// PDE Load History by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(PagedOutputModel<List<UserLogResponseModel<PDELoadDataLogResponseModel>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(int Id)
        {
            try
            {
                var response = await _services.GetPDELoadHistoryGetById(Id).ConfigureAwait(false);
                return Ok(response);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLog.LogError("PDELoad" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}
