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
    /// StoreGroupController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class StoreGroupController : Controller
    {
        private readonly IStoreGroupServices _iStoreGroupService = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// StoreGroupController
        /// </summary>
        /// <param name="storeGroupService"></param>
        /// <param name="logger"></param>
        public StoreGroupController(IStoreGroupServices storeGroupService,ILoggerManager logger)
        {
            _iLogger = logger;
            this._iStoreGroupService = storeGroupService;
        }

        /// <summary>
        /// Get all active StoreGroups
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PagedOutputModel<List<StoreGroupResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                var storeList = await _iStoreGroupService.GetAllActiveStoreGroups(filter).ConfigureAwait(false);
                return Ok(storeList);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StoreGroupController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Get Store by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoreGroupResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStoreGroupsById(int id)
        {
            try
            {
                return Ok(await _iStoreGroupService.GetStoreGroupsById(id).ConfigureAwait(false));
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
                _iLogger.LogError("StoreGroupController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        // POST: api/Stores/AddStoreGroup
        /// <summary>
        /// Add new Store Group
        /// <param name="storeModel">StoreGroupViewModel</param>
        /// </summary>
        /// <returns>StoreGroupViewModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(StoreGroupResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddStoreGroup([FromBody] StoreGroupRequestModel storeModel)
        {
            try
            {
                if (ModelState.IsValid && storeModel != null)
                {
                    var result = await _iStoreGroupService.Insert(storeModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StoreGroupController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update Store Group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StoreGroupResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStoreGroup(int id,[FromBody]  StoreGroupRequestModel storeModel)
        {
            try
            {
                if (ModelState.IsValid && storeModel != null)
                {
                    var result = await _iStoreGroupService.Update(storeModel,id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
                _iLogger.LogError("StoreGroupController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        // Delete: api/Stores/DeleteStoreGroup/{id}
        /// <summary>
        /// Delete Store Group
        /// <param name="id">Store Group Id</param>
        /// </summary>
        /// <returns>StoreGroupViewModel</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(List<StoreGroupViewModel>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStoreGroup(int id)
        {
            try
            {
                await _iStoreGroupService.DeleteStoreGroup(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StoreGroupController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetCorporateTree
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("CorporateTree")]
        [ProducesResponseType(typeof(PagedOutputModel<List<StoreGroupResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCorporateTree([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                var storeList = await _iStoreGroupService.GetCorporateTree(filter).ConfigureAwait(false);
                return Ok(storeList);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("StoreGroupController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

    }

}
