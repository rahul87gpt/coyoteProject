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
    /// MasterListItemController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class MasterListItemController : Controller
    {
        private readonly IMasterListItemService _iMasterListItemService = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// MasterListItemController
        /// </summary>
        /// <param name="iMasterListItemService"></param>
        /// <param name="iAutoMapper"></param>
        /// <param name="logger"></param>
        public MasterListItemController(IMasterListItemService iMasterListItemService, IAutoMappingServices iAutoMapper, ILoggerManager logger)
        {
            _iLogger = logger;
            _iMasterListItemService = iMasterListItemService;
            _iAutoMapper = iAutoMapper;
        }

        /// <summary>
        /// Get all MasterListItem with the is_Deleted flag is Zero
        /// </summary>
        /// <param name="code"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("code")]
        [ProducesResponseType(typeof(List<MasterListItemResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(string code,[FromQuery] MasterListFilterModel filter = null)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<MasterListItemResponseViewModel>> masterList = await _iMasterListItemService.GetActiveMasterListItems(code, securityViewModel, filter).ConfigureAwait(false);
                return Ok(masterList);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("MasterListItemController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        ///  Get the detail of a MasterList by its Id
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{code}/{id}")]
        [ProducesResponseType(typeof(MasterListItemResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string code, int id)
        {
            try
            {
                var masterListItem = await _iMasterListItemService.GetMasterListItemsById(id, code).ConfigureAwait(false);
                return Ok(masterListItem);
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
                _iLogger.LogError("MasterListItemController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Add MasterListItem into the database
        /// </summary>
        /// <param name="masterListItem"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("{code}")]
        [ProducesResponseType(typeof(MasterListItemResponseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] MasterListItemRequestModel masterListItem, string code)
        {
            try
            {
                if (ModelState.IsValid && masterListItem != null && !string.IsNullOrEmpty(code))
                {
                    var result = await _iMasterListItemService.AddMasterListItem(masterListItem, code, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
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
                _iLogger.LogError("MasterListItemController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Update MasterListItem by MasterList_Id into the database
        /// </summary>
        /// <param name="masterListItem"></param>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MasterListItemResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPut("{code}/{id}")]
        public async Task<IActionResult> Put([FromBody]  MasterListItemRequestModel masterListItem, int id, string code)
        {
            try
            {
                if (!ModelState.IsValid || id == 0 || masterListItem == null)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.ListItemIdNotFound));
                } 
                await _iMasterListItemService.Update(masterListItem, code, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return Ok(await _iMasterListItemService.GetMasterListItemsById(id,code).ConfigureAwait(false));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (AlreadyExistsException br)
            {
                return Conflict(APIReponseBuilder.HandleResponse(br.Message));
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
                _iLogger.LogError("MasterListItemController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Delete MasterListItem by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpDelete("{code}/{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, string code)
        {
            try
            {
                await _iMasterListItemService.DeleteMasterListItem(id, code, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("MasterListItemController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
    }
}
