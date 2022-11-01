using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("UserLog")]
    public class UserLog : IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(MaxLengthConstants.MaxActivityLength)]
        public string Activity { get; set; }
        [MaxLength(MaxLengthConstants.MaxActivityLength)]
        public string ActivityType { get; set; }
        public string Module { get; set; }
        public string Table { get; set; }
        public long TableId { get; set; }
        public string Action { get; set; }
        public string DataLog { get; set; }
        public int ActionBy { get; set; }
        public string UserRole { get; set; }
        public Users ActionById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ActionAt { get; set; }
        public int? RoleId { get; set; }

    }
}
