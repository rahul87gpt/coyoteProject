using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StoreGroupRequestModel
    {
        [MaxLength(MaxLengthConstants.MinStoreGroupLength, ErrorMessage = ErrorMessages.StoreGroupCode)]
        [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.MaxStoreGroupLength, ErrorMessage = ErrorMessages.StoreGroupName)]
        [Required(ErrorMessage = ErrorMessages.StoreGroupNameReq)]
        public string Name { get; set; }

        public DateTime AddedAt { get; set; }

        public bool Status { get; set; }
    }
}
