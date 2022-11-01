using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("Messages")]
    public class POSMessages : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.ReferenceLength, ErrorMessage = ErrorMessages.ReferenceLength)]
        public string ReferenceId { get; set; }
        public int ZoneId { get; set; }
        public virtual MasterListItems Zone { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.ReferenceLength, ErrorMessage = ErrorMessages.ReferenceLength)]
        public string ReferenceType { get; set; }

        [MaxLength(MaxLengthConstants.ReferenceLength, ErrorMessage = ErrorMessages.ReferenceLength)]
        public string ReferenceOverrideType { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.ReferenceLength, ErrorMessage = ErrorMessages.ReferenceLength)]
        public string DisplayType { get; set; }
        public int Priority { get; set; }
        public string POSMessage { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.DayPartLength, ErrorMessage = ErrorMessages.DayPartLength)]
        public string DayParts { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MsgDescLength, ErrorMessage = ErrorMessages.MsgDescLength)]
        public string Desc { get; set; }
        public bool Status { get; set; } = true;
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Image { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
        public string ImageType { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }


        public DateTime UpdatedAt { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
