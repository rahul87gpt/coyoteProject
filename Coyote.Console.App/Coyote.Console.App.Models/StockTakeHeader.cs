using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("StockTakeHeader")]
    public class StockTakeHeader : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int OutletId { get; set; }
        [MaxLength(MaxLengthConstants.StoreDescLength, ErrorMessage = ErrorMessages.StockTakeLength)]
        public string Desc { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PostToDate { get; set; }
        public float Total { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }


        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<StockTakeDetail> StockTakeDetails { get; set; }
    }
}
