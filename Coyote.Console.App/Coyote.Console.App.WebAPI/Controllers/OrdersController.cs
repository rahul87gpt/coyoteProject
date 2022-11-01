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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// OrdersController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _iOrderService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// OrdersController
        /// </summary>
        /// <param name="iOrderService"></param>
        /// <param name="logger"></param>
        public OrdersController(IOrdersService iOrderService, ILoggerManager logger)
        {
            _iLogger = logger;
            _iOrderService = iOrderService;
        }


        /// <summary>
        /// Get All Active Order Headers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<OrderHeaderResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] OrderInvoiceRequestModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iOrderService.GetAllActiveOrderHeaders(inputModel, securityViewModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }

            catch (BadRequestException ex)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Order and Order Details by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDetailResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                return Ok(await _iOrderService.GetOrderById(id).ConfigureAwait(false));
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update Order and Order details 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(OrderDetailResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] OrderRequestModel viewModel, int id)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    return Ok(await _iOrderService.Update(viewModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException br)
            {
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Add new order with order details
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDetailRequestModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] OrderRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _iOrderService.Insert(viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (AlreadyExistsException br)
            {
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete Order and Order details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _iOrderService.DeleteOrder(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="detailid"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}/details/{detailid}")]
        public async Task<IActionResult> Delete(long id, long detailid)
        {
            try
            {
                await _iOrderService.DeleteOrderDetailItem(id, detailid, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// EDI for Lion Supplier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deliveryInstructions"></param>
        /// <returns></returns>
        [HttpPost("EDIForLionSupplier/{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EDIForLionSupplier(long id, string deliveryInstructions)
        {
            try
            {
                if (id > 0)
                {
                    await _iOrderService.EDIForLionSupplier(id, deliveryInstructions).ConfigureAwait(false);
                    return NoContent();
                }
                throw new NotFoundException(ErrorMessages.OrderNotFound);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// EDI for Coca Cola Supplier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deliveryInstructions"></param>
        /// <returns></returns>
        [HttpPost("EDIForCocaColaSupplier/{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EDIForCocaColaSupplier(long id, string deliveryInstructions)
        {
            try
            {
                if (id > 0)
                {
                    await _iOrderService.EDIForCocaColaSupplier(id, deliveryInstructions).ConfigureAwait(false);
                    return NoContent();
                }
                throw new NotFoundException(ErrorMessages.OrderNotFound);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// EDI for Distributor Supplier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deliveryInstructions"></param>
        /// <returns></returns>
        [HttpPost("EDIForDistributorSupplier/{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EDIForDistributorSupplier(long id, string deliveryInstructions)
        {
            try
            {
                if (id > 0)
                {
                    await _iOrderService.EDIForDistributorSupplier(id, deliveryInstructions).ConfigureAwait(false);
                    return NoContent();
                }
                throw new NotFoundException(ErrorMessages.OrderNotFound);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetOrderNumber
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/number")]
        [ProducesResponseType(typeof(OrderDetailResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderNumber(int id)
        {
            try
            {
                return Ok(await _iOrderService.GetNewOrderNumber(id).ConfigureAwait(false));
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetSupplierItem
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(OrderDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("SupplierProduct/{id}")]
        public async Task<IActionResult> GetSupplierItem([FromBody] List<OrderDetailRequestModel> viewModel, int id)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    return Ok(await _iOrderService.UpdateSupplierProductAsync(viewModel, id).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// SendOrder
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("SendOrder")]
        [ProducesResponseType(typeof(OrderDetailRequestModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOrder([FromBody] OrderSendRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    return Ok(await _iOrderService.SendOrder(requestModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
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
            catch (NotModifiedException ne)
            {
                return Conflict(APIReponseBuilder.HandleResponse(ne.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// PostOrder
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("PostOrder")]
        [ProducesResponseType(typeof(OrderDetailRequestModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostOrder([FromBody] OrderPostRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    //return Ok(await _iOrderService.PostOrder(requestModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                    return Ok(await _iOrderService.PostOrderClick(requestModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));

                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// FinishOrder
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("FinishOrder")]
        [ProducesResponseType(typeof(OrderDetailRequestModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FinishOrder([FromBody] OrderFinishRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    return Ok(await _iOrderService.FinishOrder(requestModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// RefreshOrder
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="outletId"></param>
        /// <param name="orderNo"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(OrderDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("RefreshOrder/{outletId}/{orderNo}")]
        public async Task<IActionResult> RefreshOrder([FromBody] List<OrderDetailRefreshRequestModel> viewModel, int outletId, long orderNo, int? supplierId)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    return Ok(await _iOrderService.RefreshOrder(viewModel, outletId, orderNo, supplierId).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// TabletLoad
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<OrderTabletLoadRequestModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpGet("TabletLoad")]
        public async Task<IActionResult> TabletLoad([FromQuery] OrderTabletLoadRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    return Ok(await _iOrderService.TabletLoad(requestModel).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// UpliftRecalcOrder
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("UpliftRecalcOrder")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpliftRecalcOrder([FromBody] OrderUpliftRecalcRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    return Ok(await _iOrderService.UpliftRecalcOrder(requestModel).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// AutomaticOrder
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("AutomaticOrder")]
        [ProducesResponseType(typeof(AutomaticOrderResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AutomaticOrder([FromBody] AutomaticOrderRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    return Ok(await _iOrderService.AutomaticOrder(requestModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get All Active Order Headers
        /// </summary>
        /// <returns></returns>
        [HttpPost("orders")]
        [ProducesResponseType(typeof(PagedOutputModel<List<OrderHeaderResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrders([FromBody] OrderInvoiceRequestModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iOrderService.GetActiveOrderHeaders(inputModel, securityViewModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }

            catch (BadRequestException ex)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetCoverDaysForOutletSupplier
        /// </summary>
        /// <param name="outletId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [HttpGet("GetCoverDaysForOutletSupplier/{outletId:int}/{supplierId:int}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCoverDaysForOutletSupplier(int outletId, int supplierId)
        {
            try
            {
                return Ok(await _iOrderService.GetCoverDaysForOutletSupplier(outletId, supplierId).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }

            catch (BadRequestException ex)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetDefaultSettingsForOutlet
        /// </summary>
        /// <param name="outletId"></param>
        /// <param name="supplierId"></param>
        /// <returns>AutoOrderDefaultSettingsResponseViewModel</returns>
        [HttpGet("GetDefaultSettingsForOutlet/{outletId:int}/{supplierId:int}")]
        [ProducesResponseType(typeof(AutoOrderDefaultSettingsResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDefaultSettingsForOutlet(int outletId, int supplierId)
        {
            try
            {
                return Ok(await _iOrderService.GetDefaultSettingsForOutlet(outletId,supplierId).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }

            catch (BadRequestException ex)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}