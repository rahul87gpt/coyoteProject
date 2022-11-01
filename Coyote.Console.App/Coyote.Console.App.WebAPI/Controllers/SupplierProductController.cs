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
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// SupplierProductController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class SupplierProductController : Controller
    {
        private readonly ISupplierProductService _iSupplierProductService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// SupplierProductController
        /// </summary>
        /// <param name="iSupplier"></param>
        /// <param name="logger"></param>
        public SupplierProductController(ISupplierProductService iSupplier,ILoggerManager logger)
        {
            _iLogger = logger;
            this._iSupplierProductService = iSupplier;
        }

        /// <summary>
        /// Get all active supplier products.
        /// </summary>
        /// <returns>List of SupplierProductViewModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SupplierProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GetAllAsync([FromQuery] SupplierProductFilter inputModel)
        {
            try
            {
                return Ok(await _iSupplierProductService.GetAllSupplierProducts(inputModel).ConfigureAwait(false));
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoSupplierProductFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get a supplier product by specific Supplier Id and Product Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<SupplierProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GetSupplierProductsById(long id)
        {
            try
            {
                var result = await _iSupplierProductService.GetSupplierProductsById(id).ConfigureAwait(false);
                if (result.Count>0) {
                    return Ok(result);
                }
                throw  new NotFoundException();
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.SupplierProductNotFound));
            }
            catch (NullReferenceCustomException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SupplierId));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete an existing Supplier Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSupplierProduct(long id)
        {
            try
            {
                await _iSupplierProductService.DeleteSupplierProduct(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceCustomException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SupplierProductNotFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Add a new Supplier Product.
        /// </summary>
        /// <param name="supplierProductModel">SupplierProductViewModel</param>
        /// <returns>SupplierProductViewModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<SupplierProductResponseViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddSupplierProduct([FromBody] SupplierProductRequestModel supplierProductModel)
        {
            try
            {
                if (ModelState.IsValid && supplierProductModel != null)
                {
                    var result = await _iSupplierProductService.Insert(supplierProductModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SupplierProductDuplicate));
            }
            catch (NullReferenceCustomException nre)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (NotFoundException nre)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update existing Supplier Product
        /// </summary>
        /// <param name="supplierProductModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(List<SupplierProductResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSupplierProductAsync([FromBody]  SupplierProductRequestModel supplierProductModel, int id)
        {
            try
            {
                if (ModelState.IsValid && supplierProductModel != null)
                {
                    return Ok(await _iSupplierProductService.Update(supplierProductModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false));
                }
                return BadRequest(ModelState);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceCustomException nre)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nre.Message));
            }
            catch (BadRequestException bre)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(bre.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}