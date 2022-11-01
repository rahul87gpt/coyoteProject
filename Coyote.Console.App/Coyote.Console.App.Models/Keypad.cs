using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Keypad")]
    public class Keypad : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        public int OutletId { get; set; }
        public virtual Store Store { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadCodeLength, ErrorMessage = ErrorMessages.KeypadCodeLength)]
        public String Code { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxKeypadDescLength, ErrorMessage = ErrorMessages.KeypadDescLength)]
        public string Desc { get; set; }
        public bool Status { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual ICollection<Till> TillKeypad { get; }
        public virtual ICollection<KeypadLevel> KeypadLevel { get; set; }
        public virtual ICollection<KeypadButton> KeypadButton { get; set; }
        public string KeyPadButtonJSONData { get; set; }
  }
}

