using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class WarehouseRequestModel
    {
        [Required]
        [MaxLength(MaxLengthConstants.MinWarehouseCodeLength, ErrorMessage = ErrorMessages.WarehouseCode)]

        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MinWarehouseDescLength, ErrorMessage = ErrorMessages.WarehouseDesc)]
        [Required]
        public string Desc { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int HostFormatId { get; set; }
        public bool Status { get; set; }
    }
}
