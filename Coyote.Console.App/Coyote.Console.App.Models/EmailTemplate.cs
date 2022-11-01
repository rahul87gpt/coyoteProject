using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("EmailTemplate")]
    public class EmailTemplate : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateNameLength)]
        public string Name { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateDisplayNameLength)]
        public string DisplayName { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateSubjectLength)]
        public string Subject { get; set; }
        [MaxLength(MaxLengthConstants.MaxEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string Body { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
    }
}
