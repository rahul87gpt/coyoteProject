using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("Paths")]
    public class Paths : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public PathType PathType { get; set; }
        public int? OutletID { get; set; }
        public virtual Store Store { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.DescriptionLength, ErrorMessage = ErrorMessages.DescriptionLength)]
        public string Description { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.PathLength, ErrorMessage = ErrorMessages.PathLength)]
        public string Path { get; set; }
        public Status IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public virtual Users UserModifiedBy { get; set; }
        public virtual ICollection<HostSettings> HostSettingsFilePath { get; }
    }
}
