using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class APNRequestModel
    {
        [Required]
        public long Number { get; set; }

        [Required]
        public long ProductId { get; set; }
        public DateTime? SoldDate { get; set; }
        public bool Status { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxAPNLength, ErrorMessage = ErrorMessages.APNCode)]
        public string Desc { get; set; }
    }
}
