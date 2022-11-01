using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class KeypadResponseModel : KeypadRequestModel
    {
        public int Id { get; set; }
        public string OutletName { get; set; }
        public string OutletCode { get; set; }

    }
}
