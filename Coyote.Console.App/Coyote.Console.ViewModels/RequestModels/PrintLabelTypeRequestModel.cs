using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class PrintLabelTypeRequestModel
    {
        
        [Required]
        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength, ErrorMessage = ErrorMessages.PrintLabelTypeCodeLength)]
        public String Code { get; set; }

        [Required(ErrorMessage=ErrorMessages.DescRequired)]
        [MaxLength(MaxLengthConstants.MaxPrintLabelDescCodeLength, ErrorMessage = ErrorMessages.PrintLabelTypeDescLength)]
        public String Desc { get; set; }

        [Required]
        public int LablesPerPage { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength)]
        public string PrintBarCodeType { get; set; }
        public bool Status { get; set; } = true;

    }
}
