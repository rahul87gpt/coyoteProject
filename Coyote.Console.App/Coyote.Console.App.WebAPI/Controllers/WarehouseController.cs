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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// WarehouseController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseServices _iWarehouseService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// WarehouseController
        /// </summary>
        /// <param name="iWarehouseService"></param>
        /// <param name="ilog"></param>
        public WarehouseController(IWarehouseServices iWarehouseService, ILoggerManager ilog)
        {
            _iLogger = ilog;
            _iWarehouseService = iWarehouseService;
        }

        //Get: api/Warehouse
        /// <summary>
        /// Get all active warehouses.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<WarehouseResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                return Ok(await _iWarehouseService.GetAllActiveWarehouse(filter).ConfigureAwait(false));
            }
            catch (NullReferenceException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.NoWarehouseFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("WarehouseController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        //Get: api/Warehouse/{id}
        /// <summary>
        /// Get a warehouse using specific Id
        /// </summary>
        /// <param name="id">warehouse id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WarehouseResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iWarehouseService.GetWarehouseById(id).ConfigureAwait(false));
            }
            catch (NotFoundException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.WarehouseIdNotFound));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("WarehouseController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        //POST: api/AddWarehouse   
        /// <summary>
        /// Add new warehouse
        /// </summary>
        /// <param name="warehouseViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(WarehouseResponseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddWarehouse([FromBody] WarehouseRequestModel warehouseViewModel)
        {
            try
            {
                if (ModelState.IsValid && warehouseViewModel != null)
                {
                    var result = await _iWarehouseService.Insert(warehouseViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };

                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
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
                _iLogger.LogError("WarehouseController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update existing warehouse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warehouseViewModel"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(WarehouseResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id,[FromBody]  WarehouseRequestModel warehouseViewModel)
        {
            try
            {
                if (ModelState.IsValid && warehouseViewModel != null)
                {
                   var result=  await _iWarehouseService.Update(warehouseViewModel,id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
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
                _iLogger.LogError("WarehouseController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        //Delete: api/DeleteWarehouse
        /// <summary>
        /// Delete a warehouse
        /// </summary>
        /// <param name="id">warehouse id</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            try
            {
                await _iWarehouseService.DeleteWarehouse(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException)
            {
                return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.WarehouseNotFound));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("WarehouseController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}