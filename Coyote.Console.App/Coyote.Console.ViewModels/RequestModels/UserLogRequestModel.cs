using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class UserLogRequestModel<T>
    {
        public string Module { get; set; }
        public string Table { get; set; }
        public long TableId { get; set; }
        public string Action { get; set; }
        public int ActionBy { get; set; }
        public T NewData { get; set; }
        public T OldData { get; set; }
    }
}
