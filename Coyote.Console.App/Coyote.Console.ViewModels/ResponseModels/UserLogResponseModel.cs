using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class UserLogResponseModel<T> where T : class
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public string Table { get; set; }
        public long TableId { get; set; }
        public string Action { get; set; }
        //public string DataLog { get; set; }
        public DataLog<T> DataLogs { get; set; }
        public int ActionBy { get; set; }       
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public DateTime ActionAt { get; set; }
        public string Activity { get; set; }

    }

    public class DataLog<T> where T : class
    {
        public T NewData { get; set; }
        public T OldData { get; set; }
    }
}
