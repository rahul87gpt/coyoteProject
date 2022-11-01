using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.RequestModels;
using FastReport;
using FastReport.Export.Html;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    ///     ReportViewerController
    /// </summary>
    [Route("/Api/[controller]")]
    public class ReportViewerController : Controller
    {
        /// <summary>
        ///    IConfiguration 
        /// </summary>
        public IConfiguration Configuration { get; }
        private string ConnectionString { get; set; }

        IReportService _reportService;
        /// <summary>
        ///   IHostingEnvironment  
        /// </summary>
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// ReportViewerController
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="reportService"></param>
#pragma warning disable CA1041 // Provide ObsoleteAttribute message
        [Obsolete]
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
        public ReportViewerController(IConfiguration configuration, IHostingEnvironment hostingEnvironment,IReportService reportService)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetConnectionString("DBConnection");
            _hostingEnvironment = hostingEnvironment;
            _reportService = reportService;
        }
        //GET: api/ReportViewer
        ///// <summary>
        /////     Get PrintLabel
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Open file in online designer</returns>
        //[HttpGet("PrintLabelFormat")]
        //// [Obsolete]
        //public IActionResult GetPrintLabelFormat([FromQuery] PrintLabelRequestModel request)
        //{
        //    var webReport = new WebReport();
        //    webReport.Width = "1000";
        //    webReport.Height = "1000";
        //    string webRootPath = _hostingEnvironment.WebRootPath;



        //    string reportPath = webRootPath + "/reports/" + "PRINTLABEL_STD_9OWL.frx";
        //    webReport.Report.Load(reportPath); // Load the report into a WebReport object
        //    if (request == null)
        //        throw new Exception();


        //    if (string.IsNullOrEmpty(request.Format) || request.Format.ToLower(CultureInfo.CurrentCulture) != "pdf")
        //        throw new Exception();


        //    if (request.StoreId <= 0)
        //    {
        //        throw new Exception();
        //    }
        //    string reportType = "PRINTLABEL_STD_9OWL_New.fr";


        //    if (string.IsNullOrEmpty(reportType))
        //        throw new Exception();

        //    reportPath = reportPath + reportType;

        //    var APNLow = "999999999999999";
        //    if (request?.BarCodeType?.ToLower(CultureInfo.CurrentCulture) == "apn")
        //    {
        //        APNLow = "9999999";
        //    }

        //    var dataSet = new DataSet();


        //    List<SqlParameter> dbParams = new List<SqlParameter>
        //           {
        //           new SqlParameter("@Outlet", request?.StoreId),
        //           new SqlParameter("@PromoId", request.PromoId.HasValue?request?.PromoId : null),
        //           new SqlParameter("@PrintType", request?.PrintType.ToLower(CultureInfo.CurrentCulture)),
        //           new SqlParameter("@APNLow", APNLow),
        //           new SqlParameter("@PriceLevel", request?.PriceLevel),
        //           new SqlParameter("@SpecDateFrom", request?.SpecDateFrom),
        //           new SqlParameter("@SpecDateTo", request?.SpecDateTo),
        //           new SqlParameter("@ReprintDateTime", request?.ReprintDateTime),
        //           new SqlParameter("@CommodityIds", request?.CommodityIds),
        //           new SqlParameter("@DepartmentIds", request?.DepartmentIds),
        //           new SqlParameter("@GroupIds", request?.GroupIds),
        //           new SqlParameter("@SupplierIds", request?.SupplierIds),
        //           new SqlParameter("@ManufacturerIds ", request?.ManufacturerIds),
        //           new SqlParameter("@CategoryIds", request?.CategoryIds),
        //           new SqlParameter("@ProductRangeFrom", request?.ProductRangeFrom),
        //           new SqlParameter("@ProductRangeTo", request?.ProductRangeTo),
        //           new SqlParameter("@MemberIds",request?.MemberIds)
        //           };

        //    dataSet = GetReportData("RPT_GetPrintChangeLabels", dbParams.ToArray());
        //    webReport.Report.RegisterData(dataSet, "Data");
        //    webReport.Mode = WebReportMode.Designer; // Set the mode of the web report object - display of the designer
        //    //webReport.ShowPrint = true;
        //    webReport.ShowToolbar = true;
        //    ReportPage page = webReport.Report.FindObject("Page1") as ReportPage;
        //    page.Landscape = false;
        //    string designerPath = Configuration["FastReportDesignerPath:DesignerPath"];
        //    webReport.DesignerPath = designerPath + "index.html";
        //    webReport.DesignerSaveCallBack = "ReportViewer/SaveReport";
        //    return View(webReport);
        //}


        //GET: api/ReportViewer
        /// <summary>
        ///     Get PrintLabel
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Open file in online designer</returns>
        [HttpGet("PrintLabel")]
        [Obsolete]
        public IActionResult GetPrintLabel([FromQuery] PrintLabelRequestModel request)
        {
            var webReport = new WebReport();
           
            string webRootPath = _hostingEnvironment.WebRootPath;
            string reportPath = webRootPath + "/reports/";

            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.LabelType))
                throw new BadRequestException(ErrorMessages.LabelTypeRequired.ToString(CultureInfo.CurrentCulture));

#pragma warning disable CA1304 // Specify CultureInfo
            if (string.IsNullOrEmpty(request.Format) || request.Format.ToLower() != "pdf")
#pragma warning restore CA1304 // Specify CultureInfo
                throw new BadRequestException(ErrorMessages.FormatRequired.ToString(CultureInfo.CurrentCulture));

            if (request.PriceLevel <= 0 || request.PriceLevel > 5)
                throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));


            if (request.StoreId <= 0)
            {
                throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
            }

            request.PrintType = "Change";
            try
            {

                // string reportPath = Configuration["FastReportSettings:ReportPath"];
                string reportType = "";
                switch (request.LabelType?.ToUpper(CultureInfo.CurrentCulture))
                {
                    case PrintLabelTypes.SHELF1UP: reportType = PrintLabelPaths.SHELF1UP; break;
                    case PrintLabelTypes.SLADEPOINT33: reportType = PrintLabelPaths.SLADEPOINT33; break;
                    case PrintLabelTypes.Shelf24: reportType = PrintLabelPaths.Shelf24; break;
                    case PrintLabelTypes.Potrait21: reportType = PrintLabelPaths.Potrait21; break;
                    case PrintLabelTypes.BurleighAPN: reportType = PrintLabelPaths.BurleighAPN; break;
                    case PrintLabelTypes.Fresh9UP: reportType = PrintLabelPaths.Fresh9UP; break;
                    case PrintLabelTypes.Helensvale24: reportType = PrintLabelPaths.Helensvale24; break;
                    case PrintLabelTypes.Offer9UP: reportType = PrintLabelPaths.Offer9UP; break;
                    case PrintLabelTypes.OfferA4OWL: reportType = PrintLabelPaths.OfferA4OWL; break;
                    case PrintLabelTypes.OwlAPN: reportType = PrintLabelPaths.OwlAPN; break;
                    case PrintLabelTypes.OwlMSI: reportType = PrintLabelPaths.OwlMSI; break;
                    case PrintLabelTypes.OWL24: reportType = PrintLabelPaths.OWL24; break;
                    case PrintLabelTypes.Promo9OWL: reportType = PrintLabelPaths.Promo9OWL; break;
                    case PrintLabelTypes.Promo9UP: reportType = PrintLabelPaths.Promo9UP; break;
                    case PrintLabelTypes.Promo4UP: reportType = PrintLabelPaths.Promo4UP; break;
                    case PrintLabelTypes.PromoA4OWL: reportType = PrintLabelPaths.PromoA4OWL; break;
                    case PrintLabelTypes.SHORT1: reportType = PrintLabelPaths.SHORT1; break;
                    case PrintLabelTypes.STARLINE: reportType = PrintLabelPaths.STARLINE; break;
                    case PrintLabelTypes.StarlineAPN: reportType = PrintLabelPaths.StarlineAPN; break;
                    case PrintLabelTypes.Std4OWL: reportType = PrintLabelPaths.Std4OWL; break;
                    case PrintLabelTypes.Std9BGOLD: reportType = PrintLabelPaths.Std9BGOLD; break;
                    case PrintLabelTypes.Std9FBLS: reportType = PrintLabelPaths.Std9FBLS; break;
                    case PrintLabelTypes.Std9OWL: reportType = PrintLabelPaths.Std9OWL; break;
                    case PrintLabelTypes.StdA4OWL: reportType = PrintLabelPaths.StdA4OWL; break;
                    case PrintLabelTypes.StdAPN: reportType = PrintLabelPaths.StdAPN; break;
                    default: reportType = ""; break;
                }

                if (string.IsNullOrEmpty(reportType))
                    throw new BadRequestException(ErrorMessages.LabelTypeUnavailable.ToString(CultureInfo.CurrentCulture));

                reportPath = reportPath + reportType;

                var APNLow = "999999999999999";
                if (request?.BarCodeType?.ToLower(CultureInfo.CurrentCulture) == "apn")
                {
                    APNLow = "9999999";
                }

#pragma warning disable CA2000 // Dispose objects before losing scope
                var dataSet = new DataSet();
#pragma warning restore CA2000 // Dispose objects before losing scope


                List<SqlParameter> dbParams = new List<SqlParameter>
                   {
                   new SqlParameter("@Outlet", request?.StoreId),
                   new SqlParameter("@PromoId", request.PromoId.HasValue?request?.PromoId : null),
                   new SqlParameter("@PrintType", request?.PrintType.ToLower(CultureInfo.CurrentCulture)),
                   new SqlParameter("@APNLow", APNLow),
                   new SqlParameter("@PriceLevel", request?.PriceLevel),
                   new SqlParameter("@SpecDateFrom", request?.SpecDateFrom),
                   new SqlParameter("@SpecDateTo", request?.SpecDateTo),
                   new SqlParameter("@ReprintDateTime", request?.ReprintDateTime),
                   new SqlParameter("@CommodityIds", request?.CommodityIds),
                   new SqlParameter("@DepartmentIds", request?.DepartmentIds),
                   new SqlParameter("@GroupIds", request?.GroupIds),
                   new SqlParameter("@SupplierIds", request?.SupplierIds),
                   new SqlParameter("@ManufacturerIds ", request?.ManufacturerIds),
                   new SqlParameter("@CategoryIds", request?.CategoryIds),
                   new SqlParameter("@ProductRangeFrom", request?.ProductRangeFrom),
                   new SqlParameter("@ProductRangeTo", request?.ProductRangeTo),
                   new SqlParameter("@MemberIds",request?.MemberIds)
                   };

#pragma warning disable CA2000 // Dispose objects before losing scope
                //dataSet = GetReportData(StoredProcedures.GetPrintChangeLabels, dbParams.ToArray());
#pragma warning restore CA2000 // Dispose objects before losing scope
                 dataSet = _reportService.GetReportData(StoredProcedures.GetPrintChangeLabels, dbParams.ToArray());

                if (dataSet == null || dataSet.Tables.Count == 0)
                    throw new BadRequestException(ErrorMessages.NoLabelAvailable.ToString(CultureInfo.CurrentCulture));


                webReport.Report.RegisterData(dataSet, "Data");
                webReport.Mode = WebReportMode.Designer; // Set the mode of the web report object - display of the designer
                                                         //webReport.ShowPrint = true;
                webReport.ShowToolbar = true;
                ReportPage page = webReport.Report.FindObject("Page1") as ReportPage;
                //   page.Landscape = false;
                string designerPath = Configuration["FastReportDesignerPath:DesignerPath"];
                webReport.DesignerPath = designerPath + "index.html";
                webReport.DesignerSaveCallBack = "ReportViewer/SaveReport";
                return View(webReport);
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


        

        //private DataSet GetReportData(string commandText, SqlParameter[] sqlParameters)
        //{
        //    DataSet dset = new DataSet();
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(commandText))
        //        {
        //            cmd.Connection = con;
        //            cmd.CommandTimeout = 180;
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddRange(sqlParameters);
        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //            {
        //                sda.Fill(dset);
        //                dset.Tables[0].TableName = "DataSet";
        //                //to avoid error on others
        //                if (dset.Tables.Count > 1)
        //                    dset.Tables[1].TableName = "FilterDataSet";
        //                if (dset.Tables.Count > 2)
        //                    dset.Tables[2].TableName = "StoreDataSet";
        //                if (dset.Tables.Count > 3)
        //                    dset.Tables[3].TableName = "DeptDataSet";
        //                if (dset.Tables.Count > 4)
        //                    dset.Tables[4].TableName = "CommDataSet";
        //                if (dset.Tables.Count > 5)
        //                    dset.Tables[5].TableName = "CatDataSet";
        //                if (dset.Tables.Count > 6)
        //                    dset.Tables[6].TableName = "GroupsDataSet";
        //                if (dset.Tables.Count > 7)
        //                    dset.Tables[7].TableName = "SupplierDataSet";
        //                if (dset.Tables.Count > 8)
        //                    dset.Tables[8].TableName = "ManufacturerDataSet";
        //                if (dset.Tables.Count > 9)
        //                    dset.Tables[9].TableName = "CashierDataSet";
        //                return dset;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        ///     Get PrintLabeltest
        /// </summary>
        [HttpGet("PrintLabel2")]
#pragma warning disable CA1041 // Provide ObsoleteAttribute message
        [Obsolete]
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
        public IActionResult GetPrintLabel2([FromQuery] ReportRequestModel reportRequest)
        {
            var webReport = new WebReport();
            webReport.Width = "1200";
            webReport.SplitReportPagesInTabs = false;
            string webRootPath = _hostingEnvironment.WebRootPath;
            string reportPath = webRootPath + "/reports/";

            try
            {

                // string reportPath = Configuration["FastReportSettings:ReportPath"];
                string reportType = "RPT_ITEMSALESReport.frx";
                reportPath = reportPath + reportType;
                DataSet dataSet1 = null;
                dataSet1 = GetItemSalesds();

                if (dataSet1 == null || dataSet1.Tables.Count == 0)
                    throw new BadRequestException(ErrorMessages.NoLabelAvailable.ToString(CultureInfo.CurrentCulture));

                string mimeType = string.Empty;
                //Config.WebMode = true;
                //Report report = new Report(); // Create a new report
                //ReportPage page = new ReportPage(); // Create a new report page
                //report.Pages.Add(page); // Add a page to the report
                //webReport.Report = report; // Assign a blank report to the report in the program
                webReport.ShowPdfExport = true;
                webReport.PrintPdf();
                webReport.Report.Load(reportPath);
                webReport.Report.RegisterData(dataSet1, "Data");

                //webReport.Report.Prepare();
                //webReport.Mode = WebReportMode.Designer; // Set the mode of the web report object - display of the designer
                webReport.Mode = WebReportMode.Designer;
                webReport.DesignerPath = "/WebReportDesigner/index.html";
                webReport.DesignerSaveCallBack = "/ReportViewer/SaveDesignedReport";
                
                ViewBag.WebReport = webReport;
                return View();
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




        private DataSet GetItemSalesds()
        {
            try
            {
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", "2021-12-01T14:41:45"),
                new SqlParameter("@endDate", "2021-12-11T14:41:45"),
               
                new SqlParameter("@storeIds", "124"),
                new SqlParameter("@reportType", "outlet"),

                //List<SqlParameter> dbParams = new List<SqlParameter>{
                //new SqlParameter("@startDate", request?.StartDate),
                //new SqlParameter("@endDate", request?.EndDate),
                //new SqlParameter("@productStartId", request?.ProductStartId),
                //new SqlParameter("@productEndId", request?.ProductEndId),
                //new SqlParameter("@promoSales",request.IsPromoSale),
                //new SqlParameter("@promoCode",request.PromoCode),
                //new SqlParameter("@summary",request.Summary),
                //new SqlParameter("@drillDown", request?.DrillDown),
                //new SqlParameter("@contineous",request?.Continuous),
                //new SqlParameter("@variance",request?.Variance),
                //new SqlParameter("@wastage", request?.Wastage),
                //new SqlParameter("@merge",request?.Merge),
                //new SqlParameter("@tillId", request?.TillId),
                //new SqlParameter("@storeIds", request?.StoreIds),
                //new SqlParameter("@zoneIds", request?.ZoneIds),
                //new SqlParameter("@dayRange", request?.DayRange),
                //new SqlParameter("@departmentIds", request?.DepartmentIds),
                //new SqlParameter("@commodityIds", request?.CommodityIds),
                //new SqlParameter("@categoryIds", request?.CategoryIds),
                //new SqlParameter("@groupIds", request?.GroupIds),
                //new SqlParameter("@suppliersIds", request?.SupplierIds),
                //new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                //new SqlParameter("@productIds", request?.ProductIds),
                //new SqlParameter("@memberIds", request?.MemberIds),
                //new SqlParameter("@reportType", "department"),
                //new SqlParameter("@IsMember", request?.IsMember),
                //new SqlParameter("@Quantity", request?.Quantity),
                //new SqlParameter("@Amount", request?.Amount),
                //new SqlParameter("@GP", request?.GP),
                //new SqlParameter("@Margin", request?.Margin)
            };
              var dataSet = _reportService.GetReportData(StoredProcedures.GetItemsSalesReportNew, dbParams.ToArray());
                return dataSet;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [HttpPost]
#pragma warning disable CA1041 // Provide ObsoleteAttribute message
        [Obsolete]
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
                              // call-back for save the designed report
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ActionResult SaveDesignedReport(string reportID, string reportUUID)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string reportPath = webRootPath + "/reports/";
            string reportType = "RPT_ITEMSALESReport.frx";
            reportPath = reportPath + reportType;

            Stream reportForSave = Request.Body; // Write the result of the Post query to the stream
            string pathToSave = reportPath; // get the path to save the file
            using (FileStream file = new FileStream(pathToSave, FileMode.Create)) // Create a file stream 
            {
                reportForSave.CopyToAsync(file).Wait(); // Save the result of the query to a file
            }
            return View();
        }
 

    }
}