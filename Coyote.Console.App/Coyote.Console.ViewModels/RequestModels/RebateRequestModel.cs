using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class RebateRequestModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public RebateType Type { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        public int? ZoneId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> RebateOutletsList { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public List<RebateDetailsRequestModel> RebateDetailsList { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }

    public class RebateDetailsRequestModel
    {
        public long ProductId { get; set; }
        public float Amount { get; set; }
    }
}
