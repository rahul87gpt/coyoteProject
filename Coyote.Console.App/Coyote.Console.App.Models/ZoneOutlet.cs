using Coyote.Console.App.Models.EntityContracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("ZoneOutlet")]
    public class ZoneOutlet : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
       
        [Required]
        public int StoreId { get; set; }
        [Required]
        public int ZoneId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual MasterListItems ZoneMasterListItems { get; set; }
        public virtual Store Store { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
    }
}
