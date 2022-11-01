using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("JournalHeader")]
    public class JournalHeader : IAuditable<int>, IKeyIdentifier<long>
    {
        [Key]
        public long Id { get; set; }
        [Required] 
        public int HeaderDate { get; set; }
        [Required]
        public int HeaderTime { get; set; }
        [Required]
        public int OutletId { get; set; }
        //[MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress1)]
        //public string OutletCode { get; set; }
        public virtual Store Store { get; set; }
        [Required]
        public int TillId { get; set; }
        //public string TillCode { get; set; }
        [MaxLength(MaxLengthConstants.MaxTillCodeLength, ErrorMessage = ErrorMessages.TillCodeLength)]
        public virtual Till Till { get; set; }
        [Required]
        public int TransactionNo { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxJournalTypeLength, ErrorMessage =ErrorMessages.JournalHeaderTypeLength)]
        public string Type { get; set; }
        public bool Status { get; set; }
        public float TransactionAmount { get; set; }
        public float TransactionGST { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public DateTime TradingDate { get; set; }
        [Required]
        public int Hour { get; set; }
        [Required]
        public int CashierId { get; set; }
        //public long? CashierNumber { get; set; }
        public virtual Cashier Cashier { get; set; }
        public bool PostStatus { get; set; }

        public DateTime? TransactionTimeStamp { get; set; }
        public bool? ProcessStatus { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public virtual ICollection<JournalDetail> JournalDetails { get; }
    }
}
