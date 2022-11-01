using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class TaxRequestModel
    {
        [MaxLength(MaxLengthConstants.MinTaxCodeLength, ErrorMessage = ErrorMessages.TaxCode)]
        [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.MinTaxDescLength, ErrorMessage = ErrorMessages.TaxDesc)]
        [Required]
        public string Desc { get; set; }

        public float Factor { get; set; }

        public bool Status { get; set; }
    }
}
