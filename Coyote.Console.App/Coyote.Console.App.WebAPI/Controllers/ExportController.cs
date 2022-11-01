using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    /// Export Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[PermissionAuthorize]
    public class ExportController : Controller
    {
        private readonly IExportServices _export = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        ///  ExportController
        /// </summary>
        /// <param name="iExport"></param>
        /// <param name="logger"></param>
        public ExportController(IExportServices iExport, ILoggerManager logger)
        {
            _iLogger = logger;
            _export = iExport;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFile([FromQuery] ExportFilterRequestModel inputfilter)
        {
            try
            {
                var stream = await _export.GetExportedFiles(inputfilter).ConfigureAwait(false);

                var mime = "application/zip";
                var file = string.Concat("Export", ".zip"); 
                
                //download export file
                return File(stream, mime, file); // attachment
                
            }
            catch (AlreadyExistsException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (BadRequestException br)
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
                _iLogger.LogError("ExportController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ex.InnerException + ex.Message + ex.StackTrace));
                //return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }


        
    }
}
