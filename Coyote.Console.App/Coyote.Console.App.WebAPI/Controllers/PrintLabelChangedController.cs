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
    /// PrintLabelChangedController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PrintLabelChangedController : ControllerBase
    {
        private readonly IPrintChangedLabelServices services;
        private readonly ILoggerManager _iLogger;
        private IImageUploadHelper _iImageUploader = null;

        /// <summary>
        /// PrintLabelChangedController
        /// </summary>
        /// <param name="iservices"></param>
        /// <param name="logger"></param>
        /// <param name="iImageUploader"></param>
        public PrintLabelChangedController(IPrintChangedLabelServices iservices, ILoggerManager logger, IImageUploadHelper iImageUploader)
        {
            _iLogger = logger;
            services = iservices;
            _iImageUploader = iImageUploader;
        }


        /// <summary>
        /// Get Details
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(PrintChangeLabelResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await services.GetPrintLabelChangedAsync().ConfigureAwait(false));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// GetPrintChangedLabel
        /// </summary>
        /// <returns></returns>
        [HttpGet("PrintChangedLabel")]
        [ProducesResponseType(typeof(PrintChangeLabelResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPrintChangedLabel([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await services.GetPrintChangedLabel(inputModel).ConfigureAwait(false));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// GetReprintChangedLabel
        /// </summary>
        /// <returns></returns>
        [HttpGet("Reprint")]
        [ProducesResponseType(typeof(RePrintChangeLabelResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReprintChangedLabel([FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await services.GetRePrintChangedLabel(inputModel).ConfigureAwait(false));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetSpecPrintChangedLabel
        /// </summary>
        /// <returns></returns>
        [HttpGet("SpecPrint")]
        [ProducesResponseType(typeof(SpecPrintChangeLabelResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpecPrintChangedLabel([FromQuery]PagedInputModel inputModel)
        {
            try
            {
                return Ok(await services.GetSpecPrintLabelChanged(inputModel).ConfigureAwait(false));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetPromotionPrintLabel
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("PromotionPrint")]
        [ProducesResponseType(typeof(PagedOutputModel<List<PromotionResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPromotionPrintLabel([FromQuery] PromotionFilter filter)
        {
            try
            {
                return Ok(await services.GetPromotionPrintLabel(filter, null).ConfigureAwait(false));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetPrintLabelFromTablet
        /// </summary>
        /// <returns></returns>
        [HttpGet("Tablet")]
        [ProducesResponseType(typeof(PagedOutputModel<List<PromotionResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult GetPrintLabelFromTablet()
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(services.GetPrintLabelFromTablet(securityViewModel));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetPrintLabelFromPDE
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [HttpGet("PDELoad")]
        [ProducesResponseType(typeof(PagedOutputModel<List<PromotionResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult GetPrintLabelFromPDE(int StoreId)
        {
            try
            {
                return Ok(services.GetPrintLabelFromPDE(StoreId));
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
                _iLogger.LogError("PrintLabelChangedController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetPrintLabelPDEImport
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PDEImport")]
        public async Task<IActionResult> GetPrintLabelPDEImport([FromForm] PrintLabelRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    if (reportRequest.PDEFile != null)
                    {
                        // If Image to be saved, throw error before saving
                        if (reportRequest.PDEFile != null && reportRequest.PDEFile.Length > 0)
                        {
                            string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;

                            //Saved to "Product/Id" folder
                            imagePath = await _iImageUploader.UploadPDEText(reportRequest.PDEFile).ConfigureAwait(false);
                            reportRequest.PDEFilePath = imagePath;

                            if (string.IsNullOrEmpty(imagePath))
                            {
                                throw new BadRequestException(ErrorMessages.PDEFileNotUpload.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.PDEFileNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = await services.GetPrintLabelPDEImport(reportRequest, mime).ConfigureAwait(false);
                    //Get the name of resulting report file with needed extension
                    var file = string.Concat("Report", ".", reportRequest.Format);
                    //if the inline parameter is true, open in browser
                    if (reportRequest.Inline)
                        return Ok(File(stream, mime));
                    else
                        //otherwise download report file
                        return File(stream, mime, file); // attachment
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (BadRequestException nf)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

    }
}