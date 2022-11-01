using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class CommodityRequestModel
    {
        [Required]
        [MaxLength(MaxLengthConstants.MinCommodityLength, ErrorMessage = ErrorMessages.CommodityCode)]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxCommodityLength, ErrorMessage = ErrorMessages.CommodityName)]
        [Required]
        public string Desc { get; set; }
        public int? CoverDays { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public double? GPPcntLevel1 { get; set; }
        public double? GPPcntLevel2 { get; set; }
        public double? GPPcntLevel3 { get; set; }
        public double? GPPcntLevel4 { get; set; }
    }
}
