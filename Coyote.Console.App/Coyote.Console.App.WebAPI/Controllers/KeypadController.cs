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
    /// KeypadController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class KeypadController : ControllerBase
    {
        private IKeypadServices _keypadServices;
        private readonly ILoggerManager _iLogger = null;
        private IImageUploadHelper _iFileUploader = null;
        /// <summary>
        /// KeypadController
        /// </summary>
        /// <param name="keypadServices"></param>
        /// <param name="logger"></param>
        /// <param name="iFileUploader"></param>
        public KeypadController(IKeypadServices keypadServices, ILoggerManager logger, IImageUploadHelper iFileUploader)
        {
            _iLogger = logger;
            _keypadServices = keypadServices;
            _iFileUploader = iFileUploader;
        }

        /// <summary>
        /// Add new keypad
        /// </summary>
        /// <param name="viewModel">KeypadRequestModel</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(KeypadResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] KeypadRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _keypadServices.Insert(viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    if (viewModel.KeypadCopyId > 0)
                    {
                        if (result != null && viewModel.KeyPadButtonJSONData != null)
                        {
                            var jsonPath = _iFileUploader.SaveKeypadJson(viewModel.KeyPadButtonJSONData, viewModel.Code).ConfigureAwait(false);
                        }
                    }

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException aee)
            {
                return Conflict(APIReponseBuilder.HandleResponse(aee.Message));
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
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// update keypad
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel">KeypadRequestModel</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(KeypadRequestModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody] KeypadRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _keypadServices.Update(id, viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    return Ok(viewModel);
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException aee)
            {
                return Conflict(APIReponseBuilder.HandleResponse(aee.Message));
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
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Get keypad by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(KeypadResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _keypadServices.GetKeypadById(id).ConfigureAwait(false));
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
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get all active keypads
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedOutputModel<List<KeypadResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<KeypadResponseModel>> keypads = await _keypadServices.GetAllActiveKeypads(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(keypads);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Delete a keypad using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteKeypad(long id)
        {
            try
            {
                if (id != 0)
                {
                    var result = await _keypadServices.Delete(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
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
            catch (NotFoundException nr)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get details of Keypad Design Using keypad Id
        /// </summary>
        /// <param name="keypadId"></param>
        /// <returns></returns>
        [HttpGet("KeypadDesign/{keypadId}")]
        [ProducesResponseType(typeof(KeypadDesignResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetKeypadDesign(int keypadId)
        {
            try
            {
                var result = await _keypadServices.GetKeypadDesign(keypadId).ConfigureAwait(false);
                return Ok(result);
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
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Add and Update keypad designs
        /// </summary>
        /// <param name="keypadId"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPut("KeypadDesign/{keypadId}")]
        [ProducesResponseType(typeof(KeypadDesignResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateKeypadDesign(int keypadId, [FromBody] KeypadDesignRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _keypadServices.UpdateKeypadDesignUsingSP(viewModel, keypadId, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    if (result != null && viewModel.KeyPadButtonJSONData != null)
                    {
                        var jsonPath = _iFileUploader.SaveKeypadJson(viewModel.KeyPadButtonJSONData, viewModel.Code).ConfigureAwait(false);
                    }

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest();
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
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
                // return BadRequest(APIReponseBuilder.HandleResponse($"{ (ex.InnerException + ex.Message + ex.StackTrace) }"));
            }
        }


        /// <summary>
        /// Add and Update keypad designs
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("KeypadDesign")]
        [ProducesResponseType(typeof(KeypadDesignResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddKeypadDesign([FromBody] KeypadDesignRequestModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel != null)
                {
                    var result = await _keypadServices.Insert(viewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest();
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
                _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.InternalServerError));
            }
        }

        //[HttpDelete]
        //[ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        //[ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> DeleteKeypadMultiple([FromQuery] string Ids)
        //{
        //    try
        //    {
        //        if (Ids != null)
        //        {
        //            var result = await _keypadServices.DeleteMultiple(Ids, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
        //            return NoContent();
        //        }
        //        return BadRequest(ModelState);
        //    }
        //    catch (AlreadyExistsException aee)
        //    {
        //        return Conflict(APIReponseBuilder.HandleResponse(aee.Message));
        //    }
        //    catch (NullReferenceCustomException nr)
        //    {
        //        return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _iLogger.LogError("KeypadController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
        //        return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
        //    }
        //}



    }
}