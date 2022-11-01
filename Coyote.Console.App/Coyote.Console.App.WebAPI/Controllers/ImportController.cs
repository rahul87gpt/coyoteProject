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
    /// Import Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[PermissionAuthorize]
    public class ImportController : Controller
    {

        private readonly IImportServices _import = null;
        private readonly ILoggerManager _iLogger = null;
        private IImageUploadHelper _iFileUploader = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="import"></param>
        /// <param name="logger"></param>
        /// <param name="iFileUploader"></param>
        public ImportController(IImportServices import, ILoggerManager logger, IImageUploadHelper iFileUploader)
        {
            _iLogger = logger;
            _import = import;
            _iFileUploader = iFileUploader;
        }

        /// <summary>
        /// Import Files
        /// </summary>
        /// <param name="tableFilter"></param>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ImportFile")]
        public async Task<IActionResult> ImportFiles([FromQuery] ExportFilterRequestModel tableFilter, [FromForm] ImportFilterRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    if (reportRequest != null)
                    {
                        // If Image to be saved, throw error before saving
                        if (reportRequest != null && reportRequest.ImportCSV.Length > 0)
                        {
                            string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;

                            //Saved to "Product/Id" folder
                            imagePath = await _iFileUploader.UploadCSVText(reportRequest.ImportCSV, reportRequest.ImportCSV.FileName).ConfigureAwait(false);

                            if (string.IsNullOrEmpty(imagePath))
                            {
                                throw new BadRequestException(ErrorMessages.ImageSizeError.ToString(CultureInfo.CurrentCulture));
                            }

                            //not forwarding csv, sending path only
                            reportRequest.ImportCSV = null;
                            var userId = 1;// Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture);
                            var result =await  _import.ImportCSVToTable(reportRequest, tableFilter, imagePath, userId).ConfigureAwait(false);

                            if (string.IsNullOrEmpty(imagePath))
                            {
                                throw new BadRequestException(ErrorMessages.PDEFileNotUpload.ToString(CultureInfo.CurrentCulture));
                            }
                            return Ok();
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.PDEFileNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
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
