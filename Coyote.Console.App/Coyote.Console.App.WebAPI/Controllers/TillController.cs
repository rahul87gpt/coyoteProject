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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// TillController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class TillController : ControllerBase
    {
        private ITillServices _tillService;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// TillController
        /// </summary>
        /// <param name="tillService"></param>
        /// <param name="logger"></param>
        public TillController(ITillServices tillService,ILoggerManager logger)
        {
            _tillService = tillService;
            _iLogger = logger;
        }

       /// <summary>
       /// Get all active tills
       /// </summary>
       /// <param name="inputModel"></param>
       /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<TillResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<TillResponseModel>> cashiers = await _tillService.GetAllTills(inputModel, securityViewModel).ConfigureAwait(false);
                
                return Ok(cashiers);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

       
        /// <summary>
        /// Get active tll by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TillResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _tillService.GetTillById(id).ConfigureAwait(false));
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
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Add new Till
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TillResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] TillRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _tillService.Insert(viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    // Return 201
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
                return BadRequest(APIReponseBuilder.HandleResponse( br.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (NotFoundException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update Existing Till
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TillResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody]  TillRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _tillService.Update(viewModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (NotFoundException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// delete a till
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteTill(int id)
        {
            try
            {
                if (id != 0)
                {
                    var result = await _tillService.Delete(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return NoContent();
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException aee)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(aee.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get all Sync Till Details
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet("GetTillSync")]
        [ProducesResponseType(typeof(PagedOutputModel<List<TillSyncResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTillSync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<TillSyncResponseModel>> cashiers = await _tillService.GetAllTillSync(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(cashiers);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Update Sync-d Tills
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("TillSync")]
        [ProducesResponseType(typeof(TillSyncResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTillSync([FromBody] TillSyncRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _tillService.AddSyncTills(viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    // Return 201
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
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get Till Journal within specified Range
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet("GetTillJournal")]
        [ProducesResponseType(typeof(PagedOutputModel<List<TillJournalResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTillJournal([FromQuery] TillJournalInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<TillJournalResponseModel>> cashiers = await _tillService.GetTillJournal(inputModel, null, securityViewModel).ConfigureAwait(false);
                return Ok(cashiers);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Till Journal within specified Range
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("GetTillJournal")]
        [ProducesResponseType(typeof(PagedOutputModel<List<TillJournalResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTillJournal([FromQuery]TillJournalInputModel inputModel,TillJournalRequestModel requestModel=null)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<TillJournalResponseModel>> cashiers = await _tillService.GetTillJournal(inputModel, requestModel, securityViewModel).ConfigureAwait(false);
                return Ok(cashiers);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("TillController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}