using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("HOSTUPD")]
    public class HostProcessing : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public long Number { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.HostLength, ErrorMessage = ErrorMessages.Host)]
        public string Code { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.Name, ErrorMessage = ErrorMessages.SuppDesc)]
        public string Description { get; set; }
        [Required]
        public string TimeStamp { get; set; }
        public bool? Posted { get; set; }
        [Required]
        public Status IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public virtual Users UserModifiedBy { get; set; }

        public virtual ICollection<HostUpdChange> HostUpdHostProcessing { get; }
    }
}
