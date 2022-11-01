using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Till")]
    public class Till : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxTillCodeLength, ErrorMessage = ErrorMessages.TillCodeLength)]
        public String Code { get; set; }

        [Required]
        public int OutletId { get; set; }
        public virtual Store Store { get; set; }

        [Required]
        public int KeypadId { get; set; }
        public virtual Keypad Keypad { get; set; }

        [Required]
        public int TypeId { get; set; }
        public virtual MasterListItems Type { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxTillDescLength, ErrorMessage = ErrorMessages.TillDescLength)]
        public string Desc { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxTillSerailNoLength, ErrorMessage = ErrorMessages.TillSerailNoLength)]
        public String SerialNo { get; set; }

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
        public virtual Users UserUpdatedBy { get; set; }

        public virtual ICollection<JournalHeader> JournalHeaderTill { get; }
        public virtual ICollection<Transaction> TransactionTills { get; }
        public virtual ICollection<TransactionReport> TransactionReportTills { get; }
        public virtual ICollection<TillSync> SyncTills { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionTills { get; }
    }
}


