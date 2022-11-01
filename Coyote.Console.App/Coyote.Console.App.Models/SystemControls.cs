using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("SystemControls")]
    public class SystemControls : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.Name, ErrorMessage = ErrorMessages.Name)]
        public string Name { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string SerialNo { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string LicenceKey { get; set; }
        public int? MaxStores { get; set; }
        [Required]
        public TillJournal TillJournal { get; set; }
        public bool? AllocateGroups { get; set; } = false;
        [Required]
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string MassPriceUpdate { get; set; }
        public bool? AllowALM { get; set; } = false;
        public string DatabaseUsage { get; set; }
        public float GrowthFactor { get; set; }
        public bool AllowFIFO { get; set; } = false;
        [Required]
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string Color { get; set; }
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string TransactionRef { get; set; }
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string WastageRef { get; set; }
        [MaxLength(MaxLengthConstants.KeySystem, ErrorMessage = ErrorMessages.KeySystem)]
        public string TransferRef { get; set; }
        [Required]
        public long NumberFactor { get; set; }
        public bool HostUpdatePricing { get; set; } = false;
        public bool InvoicePostPricing { get; set; } = false;
        public PriceRounding PriceRounding { get; set; } //('None''5Cents')
        public DefaultItemPricing DefaultItemPricing { get; set; }//( 'Price''HoldGP%''HostPrice''BestPrice')
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
