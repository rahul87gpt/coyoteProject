using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
   public class CommodityResponseModel : CommodityRequestModel
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public string DepartmentDesc { get; set; }
        public string DepartmentCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
