using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class TaxResponseModel : TaxRequestModel
    {
        public long Id { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
