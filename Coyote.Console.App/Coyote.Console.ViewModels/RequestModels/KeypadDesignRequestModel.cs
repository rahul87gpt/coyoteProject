using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class KeypadDesignRequestModel : KeypadRequestModel
    {
        public List<KeypadLevelRequestModel> KeypadLevel { get; set; }
    }

    public class KeypadLevelRequestModel
    {

        [Required]
        public string LevelDesc { get; set; }
        public int LevelId { get; set; }
        [Required]
        public int LevelIndex { get; set; }
        public List<KeypadButtonRequestModel> KeypadButtons { get; set; }
    }
    public class KeypadButtonRequestModel
    {
        public int Id { get; set; }
        [Required]
        public int ButtonIndex { get; set; }
        [Required]
        public int Type { get; set; }
        public string ShortDesc { get; set; }
        //[Required]
        [MaxLength(MaxLengthConstants.MaxKeypadButtonDescLength, ErrorMessage = ErrorMessages.KeypadButtonDescLength)]
        public string Desc { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadButtonColorLength, ErrorMessage = ErrorMessages.KeypadButtonColorLength)]
        public string Color { get; set; }
        public int Size { get; set; }
        public int? PriceLevel { get; set; }
        public int CashierLevel { get; set; }
        public string AttributesDetails { get; set; }

        [MaxLength(MaxLengthConstants.MaxKeypadButtonPasswordLength, ErrorMessage = ErrorMessages.KeypadButtonPasswordLength)]
        public string Password { get; set; }
        public bool? isVisible { get; set; } = false;
        //Fields for button typ details
        public long? ProductId { get; set; }
        public int? CategoryId { get; set; }
        public int? ButtonLevelIndex { get; set; }
        public float? SalesDiscountPerc { get; set; }
    }
}
