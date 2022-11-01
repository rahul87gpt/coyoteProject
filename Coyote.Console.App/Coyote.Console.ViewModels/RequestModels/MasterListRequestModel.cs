using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class MasterListRequestModel
    {
        [MaxLength(MaxLengthConstants.MinListCodeLength, ErrorMessage = ErrorMessages.ListCode)]
        [Required(ErrorMessage = ErrorMessages.ListCodeRequired)]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxListCodeLength, ErrorMessage = ErrorMessages.ListName)]
        [Required(ErrorMessage = ErrorMessages.ListNameRequired)]
        public string Name { get; set; }
        [Required]
        public bool Status { get; set; }
        public MasterListAccess AccessId { get; set; } = MasterListAccess.ReadWrite;
    }
}
