using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Coyote.Console.App.Models
{
    [Table("CSCPERIOD")]
    public class CSCPeriod : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.Heading, ErrorMessage = ErrorMessages.Heading)]
        public string Heading { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }      
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
    }
}
