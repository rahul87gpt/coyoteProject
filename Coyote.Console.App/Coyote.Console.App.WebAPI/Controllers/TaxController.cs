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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// TaxController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class TaxController : ControllerBase
    {
        private readonly ITaxServices _iTaxService = null;
        private readonly ILoggerManager _iLogger = null;
        /// <summary>
        /// TaxController
        /// </summary>
        /// <param name="iTax"></param>
        /// <param name="logger"></param>
        public TaxController(ITaxServices iTax, ILoggerManager logger)
        {
            _iTaxService = iTax;
            _iLogger = logger;
        }

        /// <summary>
        /// Get all Active Taxes.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TaxResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await _iTaxService.GetAllActiveTaxes(inputModel).ConfigureAwait(false));
            }
            catch (NullReferenceException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TaxController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // GET: api/Tax/{id}
        /// <summary>
        /// Get a specific Tax using Id
        /// </summary>
        /// <param name="id">Tax Id</param>
        /// <returns>TaxViewModel</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaxResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                return Ok(await _iTaxService.GetTaxById(id).ConfigureAwait(false));
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
                _iLogger.LogError("TaxController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Add New Tax.
        /// </summary>
        /// <param name="taxViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaxResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTax([FromBody] TaxRequestModel taxViewModel)
        {
            try
            {
                if (ModelState.IsValid && taxViewModel != null)
                {
                    var result = await _iTaxService.Insert(taxViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
                _iLogger.LogError("TaxController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        ///  Updated an existing tax.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxViewModel"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(TaxResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTax(long id, [FromBody] TaxRequestModel taxViewModel)
        {
            try
            {
                if (ModelState.IsValid && taxViewModel != null)
                {
                    var result = await _iTaxService.Update(taxViewModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (NotFoundException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (AlreadyExistsException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TaxController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// DeleteTax
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTax(long id)
        {
            try
            {
                await _iTaxService.DeleteTax(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TaxController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}