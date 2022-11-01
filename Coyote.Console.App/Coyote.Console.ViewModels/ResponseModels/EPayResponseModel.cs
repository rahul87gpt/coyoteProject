using System;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class EPayResponseModel
    {
        public int ID { get; set; }
        public string Item { get; set; }
        public long? ProductID { get; set; }
        public DateTime? TIMESTAMP { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductTypeTitle { get; set; }
        public string ProductCategoryCode { get; set; }
        public string ProductCategoryTitle { get; set; }
        public string SupplierAccCode { get; set; }
        public string SupplierAccTitle { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductGroupTitle { get; set; }
        public string ProductCode { get; set; }
        public string ProductTitle { get; set; }
        public string Value { get; set; }
        public string ETUTrack1IINFieldNo { get; set; }
        public string ETUTrack1IINPosition { get; set; }
        public string ETUTrack1IIN { get; set; }
        public string ETUTrack2IIN { get; set; }
        public string ETUPANTrack { get; set; }
        public string ETUPANTrackFieldNo { get; set; }
        public string ETUPANTrackPosition { get; set; }
        public string ETUPANTrackSize { get; set; }
        public string PINPAN { get; set; }
        public string CustomField { get; set; }
        public string BarCode { get; set; }
        public string ReceiptTitle { get; set; }
        public string ReceiptPINTitle { get; set; }
        public string ReceiptSerialTitle { get; set; }
        public string ReceiptTopupTitle { get; set; }
        public string ReceiptTopupReferenceTitle { get; set; }
        public string ReceiptLogo1 { get; set; }
        public string ReceiptLogo2 { get; set; }
        public string ReceiptText { get; set; }
        public string Margin { get; set; }
        public string CostExGST { get; set; }
        public string GST { get; set; }
        public float IMPORTSEQ { get; set; }
        public string VariableValue { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string DivValue { get; set; }
        public string MenuTemplateItemTitle { get; set; }
        public int DataVersion { get; set; }
    }
}
