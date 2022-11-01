using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Coyote.Console.App.Models

{
    [Table("SchedulerUser")]
    public class SchedulerUser : IAuditableField<int>, IKeyIdentifierField<long>
    {
        [Key]
        public long ID { get; set; }
        public int UserId { get; set; }
        public virtual Users UserScheduled { get; set; }
        public long SchedulerId { get; set; }
        public virtual ReportScheduler Scheduler { get; set; }

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
