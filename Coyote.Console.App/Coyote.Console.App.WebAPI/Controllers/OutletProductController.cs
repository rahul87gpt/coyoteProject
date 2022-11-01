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
    /// OutletProductController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class OutletProductController : Controller
    {
        private readonly IOutletProductService _iOutletProductService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// OutletProductController
        /// </summary>
        /// <param name="iOutletProductService"></param>
        /// <param name="logger"></param>
        public OutletProductController(IOutletProductService iOutletProductService, ILoggerManager logger)
        {
            _iLogger = logger;
            this._iOutletProductService = iOutletProductService;
        }

        /// <summary>
        /// Get all active Outlet Products.
        /// </summary>
        /// <returns>List of OutletProductViewModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OutletProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] OutletProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iOutletProductService.GetActiveOutletProducts(inputModel, securityViewModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nre)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get all active Outlet Products.
        /// </summary>
        /// <returns>List of OutletProductViewModel</returns>
        [HttpGet("GetActiveOutletProducts")]
        [ProducesResponseType(typeof(List<OutletProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveOutletProducts([FromQuery] OutletProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iOutletProductService.GetActiveOutletProducts(inputModel, securityViewModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nre)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get all active Outlet Products by productId.
        /// </summary>
        /// <returns>List of OutletProductViewModel</returns>
        [HttpGet("GetActiveOutletProductsByProductId")]
        [ProducesResponseType(typeof(List<OutletProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveOutletProductsBYProductId([FromQuery] OutletProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iOutletProductService.GetActiveOutletProductsByProductId(inputModel, securityViewModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nre)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Add a new Outlet Product.
        /// </summary>
        /// <param name="outletProductModel">OutletProductViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<OutletProductResponseViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddOutletProduct([FromBody] OutletProductRequestModel outletProductModel)
        {
            try
            {
                if (ModelState.IsValid && outletProductModel != null)
                {
                    var Id = await _iOutletProductService.Insert(outletProductModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    var result = await _iOutletProductService.GetOutletProductsByStoreId(outletProductModel.StoreId).ConfigureAwait(false);

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException aee)
            {
                return Conflict(APIReponseBuilder.HandleResponse(aee.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete an existing Outlet Product
        /// </summary>
        /// <param name="storeId">Outlet Id</param>
        /// <param name="productId">Product Id</param>
        /// <param name="supplierId">Supplier Id</param>
        /// <returns>bool</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOutletProduct(long storeId, long productId, long? supplierId)
        {
            try
            {
                await _iOutletProductService.DeleteOutletProduct(storeId, productId, supplierId, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceCustomException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get a Outlet Products by specific Store Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<OutletProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOutletProductsByStoreId(long id)
        {
            try
            {
                return Ok(await _iOutletProductService.GetOutletProductsByStoreId(id).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update existing Outlet Product
        /// </summary>
        /// <param name="outletProductModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(List<OutletProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSupplierProductAsync([FromBody] OutletProductRequestModel outletProductModel, int id)
        {
            try
            {
                if (ModelState.IsValid && outletProductModel != null)
                {
                    await _iOutletProductService.Update(outletProductModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(await _iOutletProductService.GetOutletProductsByStoreId(outletProductModel.StoreId).ConfigureAwait(false));
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
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// SetMinOnHandInOutletProduct
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPut("SetMinOnHand")]
        [ProducesResponseType(typeof(List<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetMinOnHandInOutletProduct([FromBody] MinStockHandRequestModel viewModel)
        {
            try
            {
                return Ok(await _iOutletProductService.SetMinOnHandInOutletProduct(viewModel).ConfigureAwait(false));
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
                _iLogger.LogError("SetMinOnHandInOutletProduct" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

      /// <summary>
      /// Fetch list of Products to deactivate
      /// </summary>
      /// <param name="filter"></param>
      /// <param name="inputModel"></param>
      /// <returns></returns>
        [HttpPost("DeactivateList")]
        [ProducesResponseType(typeof(List<ProductResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDeactivateList([FromQuery] PagedInputModel filter, [FromBody] DeactivateProductListRequestModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<DeactivateProductListResponseModel>> products = await _iOutletProductService.GetDeactivateProductList(filter, inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(products);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (BadRequestException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// DeactivateProducts
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost("DeactivateProducts")]
        [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateProducts([FromBody] DeactivateProductListRequestModel inputModel)
        {
            try
            {

                await _iOutletProductService.DeativateProducts(inputModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                //return Ok(await _iOutletProductService.GetAllDeactivateList(inputModel).ConfigureAwait(false));
                return Ok();
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("OutletProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }
    }
}