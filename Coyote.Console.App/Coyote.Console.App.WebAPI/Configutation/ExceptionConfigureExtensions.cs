using Coyote.Console.Common;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Coyote.Console.App.WebAPI.Configutation
{
    /// <summary>
    /// This class used for global exception handler
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExceptionConfigureExtensions
    {
        /// <summary>
        /// Use exception configure extensions method
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="logger">ILoggerManager</param>
        public static void UseExceptionConfigure(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = ErrorMessages.ApplicationJsonContentType;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError(ErrorMessages.GlobalUnhandledException + $"{ GetExceptionMessage(contextFeature.Error) }");
                        await context.Response.WriteAsync(new HttpResponseErrorModel()
                        {

                            Status = (int)HttpStatusCode.InternalServerError,
                          
                            Message = contextFeature.Error.StackTrace,
                        }.ToString()).ConfigureAwait(false);
                    }
                });
            });
        }
        private static string GetExceptionMessage(Exception ex)
        {
            return ex == null ? string.Empty : $"{Environment.NewLine}{ ex.Message}{Environment.NewLine}" + $"{ ex.StackTrace}{Environment.NewLine}" + GetExceptionMessage(ex.InnerException);
        }
    }
}