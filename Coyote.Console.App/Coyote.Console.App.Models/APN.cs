using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("APN")]
    public class APN : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        public long Number { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SoldDate { get; set; }
        public bool Status { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxAPNLength, ErrorMessage = ErrorMessages.APNCode)]
        public string Desc { get; set; }
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

        public virtual ICollection<JournalDetail> APNJournalDetail { get; }

    }
}
