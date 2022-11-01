using Newtonsoft.Json;
using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class HttpResponseErrorModel
    {
        //public string Id { get; set; }
        //public string Code { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        //public string Path { get; set; }
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class HttpResponseModel<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class HttpListResponseModel<T> where T : class
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public HttpResponseValidationResult<T> DataList { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class HttpResponseValidationResult<T> where T : class
    {
        public int TotalRecordCount { get; set; }
        public List<T> Data { get; }

    }

    public class HttpResponseModel
    {
        public string Id { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

    }

    public class HttpAPIResponseModel
    {
        public string Message { get; set; }
     
    }



}
