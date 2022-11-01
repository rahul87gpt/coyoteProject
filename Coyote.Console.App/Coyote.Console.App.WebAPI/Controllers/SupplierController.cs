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
    /// SupplierController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _iSupplierService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// SupplierController
        /// </summary>
        /// <param name="iSupplier"></param>
        /// <param name="logger"></param>
        public SupplierController(ISupplierService iSupplier, ILoggerManager logger)
        {
            _iLogger = logger;
            this._iSupplierService = iSupplier;
        }

        /// <summary>
        /// Get all active suppliers.
        /// </summary>
        /// <returns>List of SupplierViewModel</returns>
        [HttpGet("GetActiveSuppliers")]
        [ProducesResponseType(typeof(List<SupplierResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                PagedOutputModel<List<SupplierResponseViewModel>> suppliers = await _iSupplierService.GetAllActiveSuppliers(inputModel).ConfigureAwait(false);
                return Ok(suppliers);
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoSupplierFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get all active suppliers.
        /// </summary>
        /// <returns>List of SupplierViewModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SupplierResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveSuppliers([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<SupplierResponseViewModel>> suppliers = await _iSupplierService.GetActiveSuppliers(inputModel,securityViewModel).ConfigureAwait(false);
                return Ok(suppliers);
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoSupplierFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get a supplier by specific Supplier Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SupplierResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSuppplierById(int id)
        {
            try
            {
                return Ok(await _iSupplierService.GetSuppliersById(id).ConfigureAwait(false));
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
                _iLogger.LogError("SupplierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Add a new Supplier.
        /// </summary>
        /// <param name="supplierModel">SupplierViewModel</param>
        /// <returns>SupplierViewModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SupplierResponseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierRequestModel supplierModel)
        {
            try
            {
                if (ModelState.IsValid && supplierModel != null)
                {
                    var result = await _iSupplierService.Insert(supplierModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update existing Supplier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplierModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SupplierResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSupplierAsync(int id, [FromBody] SupplierRequestModel supplierModel)
        {
            try
            {
                if (ModelState.IsValid && supplierModel != null && id != 0)
                {
                    var result = await _iSupplierService.Update(supplierModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (AlreadyExistsException br)
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
                _iLogger.LogError("SupplierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Delete an existing Supplier
        /// </summary>
        /// <param name="id">Supplier Id</param>
        /// <returns>bool</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                await _iSupplierService.DeleteSupplier(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (AlreadyExistsException ae)
            {
                return Conflict(APIReponseBuilder.HandleResponse(ae.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("SupplierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}