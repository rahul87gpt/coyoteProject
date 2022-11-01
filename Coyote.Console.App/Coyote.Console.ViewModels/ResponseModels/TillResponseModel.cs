using Coyote.Console.App.Models;
using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class TillResponseModel  :TillRequestModel
    {
        public int Id { get; set; }

        public string OutletName{ get; set; }
        public string OutletCode { get; set; }

        public string KeypadCode { get; set; }
        public string KeypadName { get; set; }

        public string TypeCode { get; set; }
        public string TypeName { get; set; }
    }
}
