using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("ReportSchedulerLog")]
   public class ReportSchedulerLog : IKeyIdentifierField<long>
    {
        [Key]
        public long ID { get; set; }
        public int? UserId { get; set; }
        public virtual Users UserScheduled { get; set; }
        public long SchedulerId { get; set; }
        public virtual ReportScheduler Scheduler { get; set; }
        public bool? IsEmailSent { get; set; }
        public bool? IsReported { get; set; }
        public int? EmailTryCount { get; set; }
        public bool IsReportGenerated { get; set; }
        public int? ReportTryCount { get; set; }
        public string EmailTemplate { get; set; }
        public byte[] ReportGenerated { get; set; }
        public string ErrorMessage { get; set; }
        public Status IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

    }
}
