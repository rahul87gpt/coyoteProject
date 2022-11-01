using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("EPAY")]
    public class EPay : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.ITEM, ErrorMessage = ErrorMessages.ItemLength)]
        public string Item { get; set; }
        public long? ProductID { get; set; }
        public virtual Product Product { get; set; }
        public DateTime TIMESTAMP { get; set; }
        [MaxLength(MaxLengthConstants.TypeCode, ErrorMessage = ErrorMessages.Type)]
        public string ProductTypeCode { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ProductTypeTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ProductCategoryCode { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ProductCategoryTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string SupplierAccCode { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string SupplierAccTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ProductGroupCode { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ProductGroupTitle { get; set; }
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string ProductCode { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ProductTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string Value { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUTrack1IINFieldNo { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUTrack1IINPosition { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUTrack1IIN { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUTrack2IIN { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUPANTrack { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUPANTrackFieldNo { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUPANTrackPosition { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ETUPANTrackSize { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string PINPAN { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string CustomField { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string BarCode { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ReceiptTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ReceiptPINTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ReceiptSerialTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ReceiptTopupTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ReceiptTopupReferenceTitle { get; set; }
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string ReceiptLogo1 { get; set; }
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string ReceiptLogo2 { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string ReceiptText { get; set; }
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string Margin { get; set; }
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string CostExGST { get; set; }
        [MaxLength(MaxLengthConstants.ProductCode, ErrorMessage = ErrorMessages.Code)]
        public string GST { get; set; }
        public float IMPORTSEQ { get; set; }
        [MaxLength(MaxLengthConstants.TypeCode, ErrorMessage = ErrorMessages.Type)]
        public string VariableValue { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string MinValue { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string MaxValue { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string DivValue { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public string MenuTemplateItemTitle { get; set; }
        [MaxLength(MaxLengthConstants.EPAYLn, ErrorMessage = ErrorMessages.EpayLn)]
        public int DataVersion { get; set; }
        [Required]
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
