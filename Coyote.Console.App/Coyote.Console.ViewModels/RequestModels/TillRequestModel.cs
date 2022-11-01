using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class TillRequestModel
    {
        

        [Required]
        [MaxLength(MaxLengthConstants.MaxTillCodeLength, ErrorMessage = ErrorMessages.TillCodeLength)]
        public String Code { get; set; }

        [Required]
        public int OutletId { get; set; }

        [Required]
        public int KeypadId { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxTillDescLength, ErrorMessage = ErrorMessages.TillDescLength)]
        public string Desc { get; set; }

        [Required]
        public bool Status { get; set; }

        //[Required]
        [MaxLength(MaxLengthConstants.MaxTillSerailNoLength, ErrorMessage = ErrorMessages.TillSerailNoLength)]
        public String SerialNo { get; set; }

    }
}
