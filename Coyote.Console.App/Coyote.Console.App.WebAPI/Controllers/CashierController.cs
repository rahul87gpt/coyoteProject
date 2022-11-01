using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// CashierController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class CashierController : ControllerBase
    {
        private ICashierServices _cashierService;
        private IImageUploadHelper _iImageUploader = null;
        private readonly ILoggerManager _iLog = null;

        /// <summary>
        /// CashierController
        /// </summary>
        /// <param name="cashierService"></param>
        /// <param name="imageHelper"></param>
        /// <param name="logger"></param>
        public CashierController(ICashierServices cashierService, IImageUploadHelper imageHelper, ILoggerManager logger)
        {
            _cashierService = cashierService;
            _iImageUploader = imageHelper;
            _iLog = logger;
        }


        /// <summary>
        /// Add a new Cashier
        /// </summary>
        /// <param name="viewModel">CashierRequestModel</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CashierResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] CashierRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _cashierService.Insert(viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    if (viewModel.Image != null && viewModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;
                            imagePath = await _iImageUploader.UploadImage(viewModel.Image, ("Cashier").ToString(CultureInfo.CurrentCulture), result.Id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                            //to save path update record
                          result=   await _cashierService.Update(viewModel, result.Id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), imagePath).ConfigureAwait(false);

                            result.ImageUploadStatusCode = "Ok";
                            result.ImagePath = imagePath;
                        }
                        catch (Exception ex)
                        {
                            result.ImageUploadStatusCode = ex.Message;
                        }
                    }
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
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
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (NotFoundException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLog.LogError("CashierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update an existing Cashier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CashierResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody]  CashierRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null && id > 0)
                {
                    string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;
                    if (viewModel.Image != null && viewModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            imagePath = await _iImageUploader.UploadImage(viewModel.Image, id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                            imageUploadStatusCode = "Ok";
                        }
                        catch (Exception ex)
                        {
                            imageUploadStatusCode = ex.Message;
                        }
                    }
                    var result = await _cashierService.Update(viewModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), imagePath).ConfigureAwait(false);
                    result.ImageUploadStatusCode = imageUploadStatusCode;
                    result.ImagePath = imagePath;
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (NotFoundException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (AlreadyExistsException nr)
            {
                return Conflict(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLog.LogError("CashierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Get all active cashier
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<CashierResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<CashierResponseModel>> cashiers = await _cashierService.GetAllActiveCashier(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(cashiers);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLog.LogError("CashierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get cashier by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CashierRequestModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _cashierService.GetCashierById(id).ConfigureAwait(false));
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
                _iLog.LogError("CashierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCashier(long id)
        {
            try
            {
                if (id != 0)
                {
                    var result = await _cashierService.Delete(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return NoContent();
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
                _iLog.LogError("CashierController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}