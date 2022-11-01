using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class POSMessageRequestModel
    {
        [Required]
        [MaxLength(MaxLengthConstants.ReferenceLength, ErrorMessage = ErrorMessages.ReferenceLength)]
        public string ReferenceId { get; set; }
       
        [Required]
        public int ReferenceTypeId { get; set; }

        public int ReferenceOverrideTypeId { get; set; }

        public int ZoneId { get; set; }
        public int DisplayTypeId { get; set; }
        public int Priority { get; set; } = MaxLengthConstants.MsgPriority;
        public string POSMessage { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public byte[] Image { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.DayPartLength, ErrorMessage = ErrorMessages.DayPartLength), MinLength(MaxLengthConstants.DayPartLength,ErrorMessage =ErrorMessages.MinDayPartLength)]
        public string DayParts { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MsgDescLength, ErrorMessage = ErrorMessages.MsgDescLength)]
        public string Desc { get; set; }
        public bool Status { get; set; } = true;


    }
}
