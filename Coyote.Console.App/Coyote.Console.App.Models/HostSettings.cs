using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("HostSettings")]
    public class HostSettings : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string Code { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.SuppDescLength, ErrorMessage = ErrorMessages.SuppDesc)]
        public string Description { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.HostLength, ErrorMessage = ErrorMessages.Host)]
        public string InitialLoadFileWeekly { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.HostLength, ErrorMessage = ErrorMessages.Host)]
        public string WeeklyFile { get; set; }
        [Required]
        public int FilePathID { get; set; }
        public virtual Paths Path { get; set; }
        [Required]
        public float NumberFactor { get; set; }
        public int? SupplierID { get; set; }
        public virtual Supplier Supplier { get; set; }
        [Required]
        public int WareHouseID { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        [Required]
        public int HostFormatID { get; set; }
        public virtual MasterListItems HostFormatWareHouse { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.HostLength, ErrorMessage = ErrorMessages.Host)]
        public string BuyPromoPrefix { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.HostLength, ErrorMessage = ErrorMessages.Host)]
        public string SellPromoPrefix { get; set; }
        public Status IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public virtual Users UserModifiedBy { get; set; }
        public virtual ICollection<CostPriceZones> CostPriceZonesHostSettings { get; }
        public virtual ICollection<HostUpdChange> HostUpdChangeHostSettings { get; }
    }
}
