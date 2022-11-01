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
namespace Coyote.Console.App.WebAPI
  {       /// <summary>
          /// HostUpdChangeController
          /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
   
    public class HostUpdChangeController : ControllerBase
    {

        private readonly IHostUpdChangeService _iservices = null;
        private readonly ILoggerManager _ilogger = null;

        /// <summary>
        /// HostUpdChangeController
        /// </summary>
        /// <param name="iservices">IHostUpdChangeService</param>
        /// <param name="logger">ILoggerManager</param>
        public HostUpdChangeController(IHostUpdChangeService iservices, ILoggerManager logger)
        {
            _iservices = iservices;
            _ilogger = logger;
        }
        /// <summary>
        /// Get the HostSettings details
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="hostId"></param>
        /// <returns>HOSTUPDChangeResponseModel</returns>

        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<HOSTUPDChangeResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel,long hostId)
        {
            try
            {
                return Ok(await _iservices.GetAllHostUpdChange(inputModel, hostId).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
           
        }


    }
}
