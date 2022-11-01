using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("KeypadLevel")]
    public class KeypadLevel : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        public int KeypadId { get; set; }
        public virtual Keypad Keypad { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadLevelDescLength, ErrorMessage = ErrorMessages.KeypadLevelDescLength)]
        public string Desc { get; set; }

        public int? LevelIndex { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        
        public int CreatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        
        public int UpdatedById { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual ICollection<KeypadButton> KeypadButton { get; set; }
        public virtual ICollection<KeypadButton> ButtonKeypadLevel { get; }

    }
}



