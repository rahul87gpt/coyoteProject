using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class MasterListItemRequestModel
    {
        public int ListId { get; set; }
        [MaxLength(MaxLengthConstants.ListItemCodeLength, ErrorMessage = ErrorMessages.ListItemCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.ListItemNameLength, ErrorMessage = ErrorMessages.ListItemName)]
       // [Required(ErrorMessage = ErrorMessages.DescRequired)]
        public string Name { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col1 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col2 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col3 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col4 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col5 { get; set; }
        public double? Num1 { get; set; }
        public double? Num2 { get; set; }
        public double? Num3 { get; set; }
        public double? Num4 { get; set; }
        public double? Num5 { get; set; }
        [Required]
        public bool Status { get; set; }

        public MasterListAccess AccessId { get; set; } = MasterListAccess.ReadWrite;
    }
}
