using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class POSMessagesResponseModel : POSMessageRequestModel
    {
        public int Id { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneDesc { get; set; }

        public string ReferenceType { get; set; }

        public string ReferenceOverrideType { get; set; }
        public string DisplayType { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ImageType { get; set; }
    }
}
