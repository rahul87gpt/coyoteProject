using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class KeypadRequestModel
    {
        [Required]
        public int OutletId { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadCodeLength, ErrorMessage = ErrorMessages.KeypadCodeLength)]
        public String Code { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadDescLength, ErrorMessage = ErrorMessages.KeypadDescLength)]
        public string Desc { get; set; }
        public int? KeypadCopyId { get; set; }
        public bool Status { get; set; }
        public string KeyPadButtonJSONData { get; set; }
    }
}
