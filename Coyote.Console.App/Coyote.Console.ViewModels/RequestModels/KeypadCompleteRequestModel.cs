using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class KeypadCompleteRequestModel : KeypadRequestModel
    {
#pragma warning disable CA2227 // Collection properties should be read only
        public List<KeypadAllLevelRequestModel> KeypadLevel { get; set; } = new List<KeypadAllLevelRequestModel>();

        public List<KeypadAllButtonRequestModel> KeypadButtons { get; set; } = new List<KeypadAllButtonRequestModel>();
#pragma warning restore CA2227 // Collection properties should be read only
    }


    public class KeypadAllLevelRequestModel
    {
        public string LevelDesc { get; set; }
        [Range(MaxLengthConstants.MinLevelIndex, MaxLengthConstants.MaxLevelIndex, ErrorMessage = ErrorMessages.KeypadLevelInvalidIndex)]
        public int LevelIndex { get; set; }
    }

    public class KeypadAllButtonRequestModel
    {
        public int Id { get; set; }

        [Range(MaxLengthConstants.MinButtonIndex, MaxLengthConstants.MaxButtonIndex, ErrorMessage = ErrorMessages.KeypadButtonInvalidIndex)]
        public int ButtonIndex { get; set; }

        [Required]
        public int Type { get; set; }
        public string ShortDesc { get; set; }
        [Required]
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
        public int? BtnTypeLevelInd { get; set; }
        public float? SalesDiscountPerc { get; set; }
    }

    public class KeypadButtonSPRequestModel
    {
        public string Desc { get; set; }
        public int Size { get; set; }
        public int? PriceLevel { get; set; }
        public int CashierLevel { get; set; }
        public string Color { get; set; }
        public string Password { get; set; }
        public string ShortDesc { get; set; }
        public int Type { get; set; }
        public int? LevelIndex { get; set; }
        public string AttributesDetails { get; set; }
        public int? CategoryId { get; set; }
        public long? ProductId { get; set; }
        public float? SalesDiscountPerc { get; set; }
        public int ButtonIndex { get; set; }
        public int? BtnTypeLevelIndex { get; set; }
        
      
    }

}
