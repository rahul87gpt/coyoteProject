using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.Helper
{
    public static class ResponseBuilder<T> where T : class
    {
        public static HttpResponseModel HandleResponse(int status, string detail, string stackTrace,string id)
        {
            return new HttpResponseModel()
            {
                Id=id,
                Status = status,
                Message = detail,
                StackTrace = stackTrace
            };
        }

        public static HttpResponseModel<T> HandleResponse(int status, string detail, T tObject)
        {
            return new HttpResponseModel<T>()
            {
                Status = status,
                Message = detail,
                Data = tObject
            };
        }
        public static HttpListResponseModel<T> HandleResponse(int status, string detail, HttpResponseValidationResult<T> tObject)
        {
            return new HttpListResponseModel<T>()
            {
                Status = status,
                Message = detail,
                DataList = tObject
            };
        }

    }
}
