using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("KeypadButton")]
    public class KeypadButton : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        public int KeypadId { get; set; }
        public virtual Keypad Keypad { get; set; }

        public int LevelId { get; set; }
        public virtual KeypadLevel KeypadLevel { get; set; }


        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadButtonTypeLength, ErrorMessage = ErrorMessages.KeypadButtonTypeLength)]
        public int Type { get; set; }
        public virtual MasterListItems ButtonType { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadButtonDescLength, ErrorMessage = ErrorMessages.KeypadButtonDescLength)]
        public string ShortDesc { get; set; }

        [MaxLength(MaxLengthConstants.MaxKeypadButtonDescLength, ErrorMessage = ErrorMessages.KeypadButtonDescLength)]
        public string Desc { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadButtonColorLength, ErrorMessage = ErrorMessages.KeypadButtonColorLength)]
        public string Color { get; set; }

        [MaxLength(MaxLengthConstants.MaxKeypadButtonPasswordLength, ErrorMessage = ErrorMessages.KeypadButtonPasswordLength)]
        public string Password { get; set; }

        public int Size { get; set; }
        
        public int CashierLevel { get; set; }

        //Fields for button typ details
        public int? PriceLevel { get; set; }
        public long? ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int? CategoryId { get; set; }
        public virtual MasterListItems Category { get; set; }
        public float? SalesDiscountPerc { get; set; }


        public virtual KeypadLevel BtnKeypadLevel { get; set; }
        public int? BtnKeypadLevelId { get; set; }

        public int? ButtonIndex { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }

        public int CreatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }

        public int UpdatedById { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public string AttributesDetails { get; set; }

    }
}



