using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("SendEmail")]
    public class SendEmail : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string FromAddress { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string ToAddress { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string CC { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string BCC { get; set; }
        [MaxLength(MaxLengthConstants.MaxEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string MsgBody { get; set; }
        public int TemplateId { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateNameLength)]
        public string TemplateName { get; set; }
        [MaxLength(MaxLengthConstants.MaxEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateBodyLength)]
        public string TemplateContent { get; set; }
        [MaxLength(MaxLengthConstants.MinEmailTemplateLength, ErrorMessage = ErrorMessages.EmailTemplateNameLength)]
        public string EmailSubject { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsSendEmail { get; set; }

        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
