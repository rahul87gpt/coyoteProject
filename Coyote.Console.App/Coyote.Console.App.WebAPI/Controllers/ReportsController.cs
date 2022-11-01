using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.DataSources;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// ReportsController
    /// </summary>
    [Route("api")]
    //[PermissionAuthorize]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILoggerManager _iLogger;
        private IConfiguration _configuration;
        /// <summary>
        /// ReportsController
        /// </summary>
        /// <param name="reportService"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public ReportsController(IReportService reportService, ILoggerManager logger, IConfiguration configuration)
        {
            _iLogger = logger;
            _reportService = reportService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get Item Sales By Department
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesDepartment")]
        public IActionResult GetItemSalesByDepartment([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.Department.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get Item NIL Transaction
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesNilTransaction")]
        public IActionResult GetSalesNilTransaction([FromBody] SalesNilTransaction reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesNilTransaction(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get Item Sales By Commodityitems
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesCommodity")]
        public IActionResult GetItemSalesByCommodity([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.Commodity.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Items With Hourly SalesReport
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("HourlySales")]
        public IActionResult GetItemsWithHourlySalesReport([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemsWithHourlySales(reportRequest, ReportType.Commodity.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Item Sales By Category
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesCategory")]
        public IActionResult GetItemSalesByCategory([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.Category.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Item Sales By Group
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesGroup")]
        public IActionResult GetItemSalesByGroup([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.Group.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Item Sales By Supplier
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesSupplier")]
        public IActionResult GetItemSalesBySupplier([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.Supplier.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Item Sales By Outlet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesOutlet")]
        public IActionResult GetItemSalesByOutlet([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.Outlet.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }



        /// <summary>
        /// Get Item Sales By Outlet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpGet("SalesOutlet2")]

        public IActionResult GetItemSalesByOutlet2([FromQuery] ReportRequestModel reportRequest)
        {
            /* try
             {
                 if (reportRequest != null)
                 {
                     string mime = "application/" + reportRequest.Format; // MIME header with default value
                     DataSet ds = _reportService.GetItemSales2(reportRequest, ReportType.Outlet.ToString(), mime);

                     setWebreport(ds, "RPT_ITEMSALESReport.frx");
                     return View();
                 }
                 else
                     return NoContent();
             }
             catch (NullReferenceException nf)
             {
                 return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
             }
             catch (Exception ex)
             {
                 _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                 return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
             }*/
            SalesReport.SetModal(reportRequest);
            return View();
        }

       
        [HttpGet("Viewer")]
        public IActionResult Viewer([FromQuery] ReportRequestModel reportRequest)
        {
            //SalesReport sr = new SalesReport();
            //SessionHelper.SetObjectInSession(HttpContext.Session, "userObject", reportRequest);
            SalesReport.SetModal(reportRequest);
            return View();
        }


        /// <summary>
        /// Get Item With No Sales
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("NoSales")]
        public IActionResult GetItemWithNoSales([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSales(reportRequest, ReportType.NoSales.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Stock Variance
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockVariance")]
        public IActionResult GetStockVariance([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStockVariance(reportRequest, mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get Stock OnHand
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockOnHandReport")]
        public IActionResult GetStockOnHand([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStockOnHand(reportRequest, mime, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture));

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Stock Adjustment
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockAdjustment")]
        public IActionResult GetStockAdjustment([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStockAdjustment(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Stock Wastage Product Wise
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockWastage")]
        public IActionResult GetStockWastageProductWise([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStockWastageProductWise(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Cost Varience
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CostVarience")]
        public IActionResult GetCostVarience([FromBody] CostVarianceFilters reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.CostVarience(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetStockPurchase
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockPurchase")]
        public IActionResult GetStockPurchase([FromBody] StockPurchaseReportRequest reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStockPurchase(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }

                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetSalesTrxSheet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SaleTrxSheet")]
        public IActionResult GetSalesTrxSheet([FromBody] StockTrxSheetRequestModel reportRequest, [FromQuery] PagedInputModel filter = null)
        {
            try
            {
                if (reportRequest != null)
                {
                    var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                    return Ok(JsonConvert.SerializeObject(_reportService.GetSalesAndStockTrxSheet(reportRequest, securityViewModel, ReportType.Sales.ToString(), filter), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetStockTrxSheet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockTrxSheet")]
        public IActionResult GetStockTrxSheet([FromBody] StockTrxSheetRequestModel reportRequest, [FromQuery] PagedInputModel filter = null)
        {
            try
            {
                if (reportRequest != null)
                {
                    var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                    return Ok(JsonConvert.SerializeObject(_reportService.GetSalesAndStockTrxSheet(reportRequest, securityViewModel, ReportType.Stock.ToString(), filter), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetJournalSalesFinancialSummary
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("JournalSalesFinancialSummary")]
        public IActionResult GetJournalSalesFinancialSummary([FromBody] JournalSalesRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetJournalSalesFinancialSummary(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetJournalSalesRoyaltySummary
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("JournalSalesRoyaltySummary")]
        public IActionResult GetJournalSalesRoyaltySummary([FromBody] JournalSalesRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetJournalSalesRoyaltyAndAdvertisingSummary(reportRequest, ReportType.Royalty.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetJournalSalesAdvertisingSummary
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("JournalSalesAdvertisingSummary")]
        public IActionResult GetJournalSalesAdvertisingSummary([FromBody] JournalSalesRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetJournalSalesRoyaltyAndAdvertisingSummary(reportRequest, ReportType.Advertising.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// DepartmentSalesChart
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("DepartmentSalesChart")]
        public IActionResult DepartmentSalesChart([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChart(reportRequest, ReportType.Department.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// DepartmentSalesChartById
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("DepartmentSalesChartById")]
        public IActionResult DepartmentSalesChartById([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartById(reportRequest, ReportType.Department.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// DepartmentSalesChartDetailedReport
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("DepartmentSalesChartDetailed")]
        public IActionResult DepartmentSalesChartDetailedReport([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartDetailed(reportRequest, ReportType.Department.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// CommoditySalesChart
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CommoditySalesChart")]
        public IActionResult CommoditySalesChart([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChart(reportRequest, ReportType.Commodity.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// CommoditySalesChartById
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CommoditySalesChartById")]
        public IActionResult CommoditySalesChartById([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartById(reportRequest, ReportType.Commodity.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// CommoditySalesChartDetailed
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CommoditySalesChartDetailed")]
        public IActionResult CommoditySalesChartDetailed([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartDetailed(reportRequest, ReportType.Commodity.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// CategorySalesChart
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CategorySalesChart")]
        public IActionResult CategorySalesChart([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChart(reportRequest, ReportType.Category.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// CategorySalesChartById
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CategorySalesChartById")]
        public IActionResult CategorySalesChartById([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartById(reportRequest, ReportType.Category.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// CategorySalesChartDetailed
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("CategorySalesChartDetailed")]
        public IActionResult CategorySalesChartDetailed([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartDetailed(reportRequest, ReportType.Category.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GroupSalesChart
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("GroupSalesChart")]
        public IActionResult GroupSalesChart([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChart(reportRequest, ReportType.Group.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GroupSalesChartById
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("GroupSalesChartById")]
        public IActionResult GroupSalesChartById([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartById(reportRequest, ReportType.Group.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GroupSalesChartDetailed
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("GroupSalesChartDetailed")]
        public IActionResult GroupSalesChartDetailed([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartDetailed(reportRequest, ReportType.Group.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// OutletSalesChart
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("OutletSalesChart")]
        public IActionResult OutletSalesChart([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChart(reportRequest, ReportType.Outlet.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// OutletSalesChartById
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("OutletSalesChartById")]
        public IActionResult OutletSalesChartById([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartById(reportRequest, ReportType.Outlet.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// OutletSalesChartDetailed
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("OutletSalesChartDetailed")]
        public IActionResult OutletSalesChartDetailed([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartDetailed(reportRequest, ReportType.Outlet.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SupplierSalesChart
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SupplierSalesChart")]
        public IActionResult SupplierSalesChart([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChart(reportRequest, ReportType.Supplier.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SupplierSalesChartById
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SupplierSalesChartById")]
        public IActionResult SupplierSalesChartById([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartById(reportRequest, ReportType.Supplier.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SupplierSalesChartDetailed
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SupplierSalesChartDetailed")]
        public IActionResult SupplierSalesChartDetailed([FromBody] ReportRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(JsonConvert.SerializeObject(_reportService.SalesChartDetailed(reportRequest, ReportType.Supplier.ToString()), Formatting.Indented));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByDepartment
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesDepartmentSummary")]
        public IActionResult GetItemSalesSummaryByDepartment([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Department.ToString(), mime);

                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByCommodity
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesCommoditySummary")]
        public IActionResult GetItemSalesSummaryByCommodity([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Commodity.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByCategory
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesCategorySummary")]
        public IActionResult GetItemSalesSummaryByCategory([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Category.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByGroup
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesGroupSummary")]
        public IActionResult GetItemSalesSummaryByGroup([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Group.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryBySupplier
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesSupplierSummary")]
        public IActionResult GetItemSalesSummaryBySupplier([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Supplier.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByOutlet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesOutletSummary")]
        public IActionResult GetItemSalesSummaryByOutlet([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Outlet.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByMember
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesMemberSummary")]
        public IActionResult GetItemSalesSummaryByMember([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.Member.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetRankingByOutlet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("RankingByOutlet")]
        public IActionResult GetRankingByOutlet([FromBody] RankingOutletRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetRankingByOutlet(reportRequest, mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(String.Format(ErrorMessages.RankingByOutletSalesNotFound.ToString(CultureInfo.CurrentCulture), reportRequest.StartDate?.ToString("dd MMM yyyy"), reportRequest.EndDate?.ToString("dd MMM yyyy"))));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemSalesSummaryByNoSales
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ItemWithNoSalesSummary")]
        public IActionResult GetItemSalesSummaryByNoSales([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    reportRequest.stockSOHButNoSales = true;
                    reportRequest.SalesSOH = GeneralFieldFilter.Equals;
                    reportRequest.salesSOHRange = 0;
                    reportRequest.OrderByAmt = true;
                    reportRequest.stockSOHLevel = true;
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.NoSales.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemPurchaseSummaryByDepartment
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PurchaseDepartmentSummary")]
        public IActionResult GetItemPurchaseSummaryByDepartment([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemPurchaseSummary(reportRequest, ReportType.Department.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemPurchaseSummaryByCommodity
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PurchaseCommoditySummary")]
        public IActionResult GetItemPurchaseSummaryByCommodity([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemPurchaseSummary(reportRequest, ReportType.Commodity.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemPurchaseSummaryByCategory
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PurchaseCategorySummary")]
        public IActionResult GetItemPurchaseSummaryByCategory([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemPurchaseSummary(reportRequest, ReportType.Category.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemPurchaseSummaryByGroup
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PurchaseGroupSummary")]
        public IActionResult GetItemPurchaseSummaryByGroup([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemPurchaseSummary(reportRequest, ReportType.Group.ToString(), mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemPurchaseSummaryBySupplier
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PurchaseSupplierSummary")]
        public IActionResult GetItemPurchaseSummaryBySupplier([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemPurchaseSummary(reportRequest, ReportType.Supplier.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemPurchaseSummaryByOutlet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PurchaseOutletSummary")]
        public IActionResult GetItemPurchaseSummaryByOutlet([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemPurchaseSummary(reportRequest, ReportType.Outlet.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get all Report filteration dropdownlist Items
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ReportDropdownListItems")]
        public async Task<IActionResult> GetReportDropdownListItems([FromBody] PagedInputModel filter = null)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                var reportDropdownlistItems = await _reportService.ReportFilterationDropdownList(securityViewModel, filter).ConfigureAwait(false);
                return Ok(reportDropdownlistItems);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetPrintLabel
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PrintLabel")]
        public IActionResult GetPrintLabel([FromBody] PrintLabelRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetPrintLabel(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
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

        /// <summary>
        /// GetPrintLabelPDELoad
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("PrintLabel/PDELoad")]
        public IActionResult GetPrintLabelPDELoad([FromBody] PrintLabelRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetPrintLabel(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
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

        /// <summary>
        /// GetRanging
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("Ranging")]
        public IActionResult GetRanging([FromBody] RangingRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetRanging(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetNationalRanging
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("NationalRanging")]
        public IActionResult GetNationalRanging([FromBody] NationalRangingRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetNationalRanging(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemWithNoSalesProduct
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ItemWithNoSalesProduct")]
        public IActionResult GetItemWithNoSalesProduct([FromBody] NoSalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetItemWithNoSalesProduct(reportRequest, ReportType.ItemNoSales.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetItemWithLessthenXdaysStock
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("LessthenXdaysStock")]
        public IActionResult GetItemWithLessthenXdaysStock([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    reportRequest.stockSOHLevel = true;
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.LessthenXdays.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// ItemWithNegativeSOH
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ItemWithNegativeSOH")]
        public IActionResult ItemWithNegativeSOH([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    reportRequest.stockSOHLevel = true;
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.ItemWithNegativeSOH.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// ItemWithZeroSOH
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ItemWithZeroSOH")]
        public IActionResult ItemWithZeroSOH([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    reportRequest.stockSOHLevel = true;
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.ItemWithZeroSOH.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// ItemWithSlowMovingStock
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ItemWithSlowMovingStock")]
        public IActionResult ItemWithSlowMovingStock([FromBody] SalesSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    reportRequest.stockSOHLevel = true;
                    var stream = _reportService.GetItemSalesSummary(reportRequest, ReportType.ItemWithSlowMovingStock.ToString(), mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetProductPriceDeviation
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ProductPriceDeviation")]
        public IActionResult GetProductPriceDeviation([FromBody] NationalRangingRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetProductPriceDeviation(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetNationalLevelSalesSummary
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("NationalLevelSalesSummary")]
        public IActionResult GetNationalLevelSalesSummary([FromBody] NationalLevelRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetNationalLevelSalesSummary(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetSelectedPrintLabel
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SelectedPrintLabel")]
        public IActionResult GetSelectedPrintLabel([FromBody] PrintLabelRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    //Set PrintLable as SELECTED
                    reportRequest.PrintType = "selected";

                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetPrintLabel(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
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

        /// <summary>
        /// GetKPIRanking
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("KPIRanking")]
        public IActionResult GetKPIRanking([FromBody] KPIRankingRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetKPIRanking(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SalesHistoryChartByDepartment
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesHistoryChartByDepartment")]
        public IActionResult GetSalesHistoryChartByDepartment([FromBody] SalesHistoryRequestModle reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesHistoryChart(reportRequest, mime, ReportType.Department.ToString());
                    if (stream != null)
                    {
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
                    {
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInWeekRange));
                        }
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInMonthRange));
                        }
                        else
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                        }
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SalesHistoryChartByCommodity
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesHistoryChartByCommodity")]
        public IActionResult GetSalesHistoryChartByCommodity([FromBody] SalesHistoryRequestModle reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesHistoryChart(reportRequest, mime, ReportType.Commodity.ToString());
                    if (stream != null)
                    {
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
                    {
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInWeekRange));
                        }
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInMonthRange));
                        }
                        else
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                        }
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetSalesHistoryChartByOutlet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesHistoryChartByOutlet")]
        public IActionResult GetSalesHistoryChartByOutlet([FromBody] SalesHistoryRequestModle reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesHistoryChart(reportRequest, mime, ReportType.Outlet.ToString());
                    if (stream != null)
                    {
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
                    {
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInWeekRange));
                        }
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInMonthRange));
                        }
                        else
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                        }
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SalesHistoryChartBySupplier
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesHistoryChartBySupplier")]
        public IActionResult GetSalesHistoryChartBySupplier([FromBody] SalesHistoryRequestModle reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesHistoryChart(reportRequest, mime, ReportType.Supplier.ToString());
                    if (stream != null)
                    {
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
                    {
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInWeekRange));
                        }
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInMonthRange));
                        }
                        else
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                        }
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SalesHistoryChartByCategory
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesHistoryChartByCategory")]
        public IActionResult GetSalesHistoryChartByCategory([FromBody] SalesHistoryRequestModle reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesHistoryChart(reportRequest, mime, ReportType.Category.ToString());
                    if (stream != null)
                    {
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
                    {
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInWeekRange));
                        }
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInMonthRange));
                        }
                        else
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                        }
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// SalesHistoryChartByGroup
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("SalesHistoryChartByGroup")]
        public IActionResult GetSalesHistoryChartByGroup([FromBody] SalesHistoryRequestModle reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetSalesHistoryChart(reportRequest, mime, ReportType.Group.ToString());
                    if (stream != null)
                    {
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
                    {
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInWeekRange));
                        }
                        if (Convert.ToString(reportRequest?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesDatesAreNotInMonthRange));
                        }
                        else
                        {
                            return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                        }
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetFinancialSummary
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("Financial")]
        public IActionResult GetFinancialSummary([FromBody] FinancialSummaryRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetFinancialSummary(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetStockTakePrint
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StockTakePrint")]
        public IActionResult GetStockTakePrint([FromBody] StockTakePrintRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStockTakePrint(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// StoreDashboard
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("StoreDashboard")]
        public IActionResult StoreDashboard([FromBody] StoreDashboardRequest reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetStoreDashboard(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// ReporterStoreKPIGet
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("ReporterStoreKPI")]
        public IActionResult GetReporterStoreKPI([FromBody] ReporterStoreKPIReport reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    return Ok(_reportService.GetReporterStoreKPI(reportRequest));
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// GetTillJournal
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("TillJournal")]
        public IActionResult GetTillJournal([FromBody] TillJournalReportModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetTillJournal(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [HttpPost("BasketIncident")]
        public IActionResult GetBasketIncidentReport([FromBody] BasketIncidentFilters reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.GetBasketIncidentReport(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// AutomaticOrder
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("AutomaticOrder")]
        [ProducesResponseType(typeof(AutomaticOrderResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult AutomaticOrder([FromBody] AutomaticOrderRequestModel requestModel)
        {
            try
            {
                if (ModelState.IsValid && requestModel != null)
                {
                    string mime = "application/pdf"; // MIME header with default value

                    var orderResult = _reportService.AutomaticOrder(requestModel, mime, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture));

                    return Ok(orderResult);
                }
                return BadRequest(ModelState);
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }



        /// <summary>
        /// AutomaticOrderPrint
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [HttpPost("AutomaticOrderPrint")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult AutomaticOrderPrint([FromBody] OrderPrintRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null && ModelState.IsValid)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.AutomaticOrderPrint(reportRequest, mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
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
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// NormalOrderPrint
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [HttpPost("NormalOrderPrint")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult NormalOrderPrint([FromBody] OrderPrintRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null && ModelState.IsValid)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.NormalOrderPrint(reportRequest, mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
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
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
 /// <summary>
        /// WeeklySalesWorkBook
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("WeeklySalesWorkBook")]
        public IActionResult WeeklySalesWorkBook([FromBody] WeeklySalesWorkBookRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.WeeklySalesWorkBook(reportRequest, mime);
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }

                }
                else
                    return NoContent();
            }
            catch (NullReferenceException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ReportsController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// StockAdjustmentPrint
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        [HttpPost("StockAdjustmentPrint")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public IActionResult StockAdjustmentPrint([FromBody] StockAdjustPrintRequestModel reportRequest)
        {
            try
            {
                if (reportRequest != null && ModelState.IsValid)
                {
                    string mime = "application/" + reportRequest.Format; // MIME header with default value
                    var stream = _reportService.StockAdjustmentPrint(reportRequest, mime);
                    //Get the name of resulting report file with needed extension
                    if (stream != null)
                    {
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
                    {
                        return NotFound(APIReponseBuilder.HandleResponse(ErrorMessages.ItemSalesNotFound));
                    }
                }
                else
                    return NoContent();
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
                _iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        private void setWebreport(DataSet dataSet1, string reportName)
        {
            var webReport = new WebReport();
            string reportPath = _configuration["FastReportSettings:ReportPath"];
            reportPath = reportPath + reportName;
            try
            {
                if (dataSet1 == null || dataSet1.Tables.Count == 0)
                    throw new BadRequestException(ErrorMessages.NoLabelAvailable.ToString(CultureInfo.CurrentCulture));
                webReport.Report.Load(reportPath);
                webReport.Report.RegisterData(dataSet1, "Data");
                // webReport.pag = 50;

                //ReportPage page = webReport.Report.FindObject("Page1") as ReportPage;
                //webReport.Toolbar.ShowPrevButton = true;
                //webReport.Toolbar.ShowNextButton = true;
                //webReport.Toolbar.ShowFirstButton = true;
                //webReport.Toolbar.ShowLastButton = true;
                webReport.SinglePage = true;
                //webReport.Toolbar.Exports.ShowPreparedReport=true;
                //webReport.Report.MaxPages = 2000;
                ViewBag.WebReport = webReport;

                // return webReport;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message, ex);
                }
                if (ex is NotFoundException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        
    }
}
