using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;

namespace Coyote.Console.App.Services.Helper
{
    public static class APIReponseBuilder
    {
        public static HttpAPIResponseModel HandleResponse(string message)
        {
            return new HttpAPIResponseModel()
            {
                Message = message,
            };
        }
        //public static HttpResponseErrorModel HandleResponse(int status, string message, string title = null, string stackTrace = null)
        //{
        //    return new HttpResponseErrorModel()
        //    {
        //        Status = status,
        //        Title = title,
        //        Message = message,
        //        StackTrace=stackTrace
        //    };
        //}
    }
}
