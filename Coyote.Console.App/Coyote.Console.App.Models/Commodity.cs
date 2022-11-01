using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Commodity")]
    public class Commodity : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MinCommodityLength, ErrorMessage = ErrorMessages.CommodityCode)]
        public string Code { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxCommodityLength, ErrorMessage = ErrorMessages.CommodityName)]
        public string Desc { get; set; }
        public int? CoverDays { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public double? GPPcntLevel1 { get; set; }
        public double? GPPcntLevel2 { get; set; }
        public double? GPPcntLevel3 { get; set; }
        public double? GPPcntLevel4 { get; set; }
        public bool Status { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Department Departments { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public int? CategoryId { get; set; }
        public virtual MasterListItems Category { get; set; }
        public virtual ICollection<Product> ProductsCommodity { get; }
        public virtual ICollection<Transaction> TransactionCommodity { get; }
        public virtual ICollection<TransactionReport> TransactionReportCommodity { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionCommodity { get; }
    }
}
