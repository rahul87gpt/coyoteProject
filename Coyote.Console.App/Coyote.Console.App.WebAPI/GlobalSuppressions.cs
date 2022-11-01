﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We are not supporting multilingal right now.", Scope = "namespaceanddescendants", Target = "Coyote.Console.App.WebAPI.Controllers")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "Coyote.Console.App.WebAPI.Controllers")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Helper.PermissionAuthorizeAttribute.OnAuthorization(Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)")]
[assembly: SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "<Pending>", Scope = "type", Target = "~T:Coyote.Console.App.WebAPI.Program")]
[assembly: SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.CashierController.GetImage(System.String)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportViewerController.GetPrintLabel(Coyote.Console.ViewModels.RequestModels.PrintLabelRequestModel)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportViewerController.#ctor(Microsoft.Extensions.Configuration.IConfiguration,Microsoft.AspNetCore.Hosting.IHostingEnvironment)")]
[assembly: SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportViewerController.GetReportData(System.String,System.Data.SqlClient.SqlParameter[])~System.Data.DataSet")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportViewerController.GetPrintLabel(Coyote.Console.ViewModels.RequestModels.PrintLabelRequestModel)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportsController.GetSalesHistoryChartByDepartment(Coyote.Console.ViewModels.RequestModels.SalesHistoryRequestModle)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportsController.GetSalesHistoryChartByCommodity(Coyote.Console.ViewModels.RequestModels.SalesHistoryRequestModle)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportsController.GetSalesHistoryChartBySupplier(Coyote.Console.ViewModels.RequestModels.SalesHistoryRequestModle)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportsController.GetSalesHistoryChartByCategory(Coyote.Console.ViewModels.RequestModels.SalesHistoryRequestModle)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportsController.GetSalesHistoryChartByGroup(Coyote.Console.ViewModels.RequestModels.SalesHistoryRequestModle)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Controllers.ReportsController.GetSalesHistoryChartByOutlet(Coyote.Console.ViewModels.RequestModels.SalesHistoryRequestModle)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Helper.ImageUploadHelper.SaveJson(System.String,System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Helper.ImageUploadHelper.CheckCreateLogDirectory(System.String)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Helper.ImageUploadHelper.WriteErrorLog(System.String)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Helper.ImageUploadHelper.WriteErrorLog(System.Exception)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>", Scope = "member", Target = "~M:Coyote.Console.App.WebAPI.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment,Coyote.Console.Common.Services.ILoggerManager,System.IServiceProvider,Microsoft.Extensions.Logging.ILoggerFactory)")]