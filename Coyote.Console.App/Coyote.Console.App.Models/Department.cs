using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Department")]
    public class Department : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(MaxLengthConstants.MinDepartmentLength, ErrorMessage = ErrorMessages.DepartmentCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxDepartmentLength, ErrorMessage = ErrorMessages.DepartmentName)]
        [Required]
        public string Desc { get; set; }
        public int? MapTypeId { get; set; }
        public double? BudgetGroethFactor { get; set; }
        public double? RoyaltyDisc { get; set; }
        public double? AdvertisingDisc { get; set; }
        public bool? AllowSaleDisc { get; set; }
        public bool? ExcludeWastageOptimalOrdering { get; set; }
        public bool? IsDefault { get; set; }
        public bool Status { get; set; }

        [Column(TypeName = "datetime")]
        [Required]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Commodity> Commodity { get; }
        public virtual MasterListItems MapTypeMasterListItems { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }

        public virtual ICollection<Product> ProductDept { get; }
        public virtual ICollection<Transaction> TransactionDepartments { get; }
        public virtual ICollection<TransactionReport> TransactionReportDepartments { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionDepartments { get; }
        public virtual ICollection<AccessDepartment> AccessDepartmentDept { get; }
    }
}
