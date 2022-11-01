using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class DepartmentRequestModel
    {
        [MaxLength(MaxLengthConstants.MinDepartmentCodeLen, ErrorMessage = ErrorMessages.DepartmentCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxDepartmentLength, ErrorMessage = ErrorMessages.DepartmentName)]
        [Required]
        public string Desc { get; set; }
        public int? MapTypeId { get; set; }
        public double? BudgetGrowthFactor { get; set; }
        public double? RoyaltyDisc { get; set; }
        public double? AdvertisingDisc { get; set; }
        public bool? AllowSaleDisc { get; set; }
        public bool? ExcludeWastageOptimalOrdering { get; set; }
        public bool? IsDefault { get; set; }
    }
}
