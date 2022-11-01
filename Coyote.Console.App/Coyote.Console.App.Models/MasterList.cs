using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("MasterList")]
    public class MasterList : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.MinListCodeLength, ErrorMessage = ErrorMessages.ListCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxListCodeLength, ErrorMessage = ErrorMessages.ListName)]
        [Required]
        public string Name { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<MasterListItems> Items { get; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public MasterListAccess AccessId { get; set; } = MasterListAccess.ReadWrite;
    }
}
