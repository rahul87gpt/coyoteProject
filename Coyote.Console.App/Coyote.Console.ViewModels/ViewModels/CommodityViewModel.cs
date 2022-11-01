using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class CommodityViewModel
    {
        public int Id { get; set; }
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
        //[Required]

        //public DateTime CreatedAt { get; set; }
        //[Required]

        //public DateTime UpdatedAt { get; set; }
        //[Required]
        //public int CreatedById { get; set; }
        //[Required]
        //public int UpdatedById { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
