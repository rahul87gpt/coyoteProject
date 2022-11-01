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
    /// OptimalOrderController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class OptimalOrderController : ControllerBase
    {
        private readonly IOptimalOrderServices _optimalOrderServices = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// OptimalOrderController
        /// </summary>
        /// <param name="optimalOrder"></param>
        /// <param name="logger"></param>
        public OptimalOrderController(IOptimalOrderServices optimalOrder, ILoggerManager logger)
        {
            _iLogger = logger;
            _optimalOrderServices = optimalOrder;
        }

        /// <summary>
        /// GetAllAsync
        /// </summary>
        /// <param name="outletId"></param>
        /// <param name="orderNo"></param>
        /// <param name="orderDate"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<OptimalOrderBatchResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllAsync(int outletId,int orderNo,DateTime orderDate)
        {
            try
            {
                var result = _optimalOrderServices.GetOptimalBatch(outletId,orderNo,orderDate);

                return Ok(result);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OptimalOrderController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}