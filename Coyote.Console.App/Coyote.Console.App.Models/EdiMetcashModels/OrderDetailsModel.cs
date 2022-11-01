using System;

namespace Coyote.Console.App.Models.EdiMetcashModels

{
    public class OrderDetailsModel
    {
        public string CustomerId { get; set; }
        public string StateCode { get; set; }
        public string PillarId { get; set; }
        public string OrderType { get; set; }
        public string Order { get; set; }
        public string fileName { get; set; }
    }
    public class PlaceOrderRequestModel
    {
        public Authentic Authentication { get; set; }
        public OperatingSystemModel operatingSystemModel { get; set; }
        public VendorDetailsModel vendorDetailsModel { get; set; }
        public PlaceOrderModel PlaceOrderModel { get; set; }
    }
    public class OrderSummaryRequestModel
    {
        public Authentic Authentication { get; set; }
        public OperatingSystemModel operatingSystemModel { get; set; }
        public VendorDetailsModel vendorDetailsModel { get; set; }
        public OrderSummayModel OrderSummayModel { get; set; }
    }
    public class ListDocumentRequestModel
    {
        public Authentic Authentication { get; set; }
        public OperatingSystemModel operatingSystemModel { get; set; }
        public VendorDetailsModel vendorDetailsModel { get; set; }
        public ListDocumentModel ListDocumentModel { get; set; }
    }
    public class ListDocumentResponseModel
    {
        public Authentic Authentication { get; set; }
        #pragma warning disable CA1819 // Properties should not return arrays
        public ListAllDocument[] ListAllDocuments { get; set; }
        #pragma warning restore CA1819 // Properties should not return arrays
    }

    public class ListAllDocument
    {
        public string CustomerId { get; set; }
        public string DocumentGUID { get; set; }
        public string FileName { get; set; }
        public string DocumentType { get; set; }
        public string HostType { get; set; }
        #pragma warning disable CA1819 // Properties should not return arrays
        public DocumentReference[] DocumentReference { get; set; }
        #pragma warning restore CA1819 // Properties should not return arrays
        public DateTime DocDate { get; set; }
        public bool DocDateSpecified { get; set; }
    }
    public class DocumentReference
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
    public class RetrieveDocumentRequestModel
    {
        public Authentic Authentication { get; set; }
        public OperatingSystemModel operatingSystemModel { get; set; }
        public VendorDetailsModel vendorDetailsModel { get; set; }
        public RetrieveDocumentModel RetrieveDocumentModel { get; set; }
    }
    public class NextDocumentResponseModel
    {
        public string DocumentGUID { get; set; }
        public string Payload { get; set; }
        public bool ZipFlag { get; set; }
        public string DocumentVersion { get; set; }
        public string Digest { get; set; }
        public string DocumentSize { get; set; }
    }
    public class NextDocumentRequestModel
    {
        public Authentic Authentication { get; set; }
        public OperatingSystemModel operatingSystemModel { get; set; }
        public VendorDetailsModel vendorDetailsModel { get; set; }
        public NextDocumentModel NextDocumentModel { get; set; }
    }
    public class NextDocumentModel
    {
        public string CustomerId { get; set; }
        public string StateCode { get; set; }
        public string PillarId { get; set; }
        public string DocumentType { get; set; }
        public string HostType { get; set; }
        public DocumentReference DocumentReference { get; set; }
    }
    public class RetrieveDocumentModel
    {
        public string DocumentGUID { get; set; }
        public string FileName { get; set; }
        public bool ZipFlag { get; set; }
        public bool ZipFlagSpecified { get; set; }
        public string DocumentVersion { get; set; }
        public string TransactionId { get; set; }
        public bool FlagAsCollected { get; set; }
        public bool FlagAsCollectedSpecified { get; set; }
    }
    public class RetrieveDocumentResponseModel
    {
        public string DocumentGUID { get; set; }
        public string Payload { get; set; }
        public bool ZipFlag { get; set; }
        public string DocumentVersion { get; set; }
        public string Digest { get; set; }
        public string DocumentSize { get; set; }
    }
    public class ListDocumentModel
    {

        public string CustomerId { get; set; }
        public string StateCode { get; set; }
        public string PillarId { get; set; }
        public string DocumentType { get; set; }
        public string HostType { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool ListRetrievedFlag { get; set; }
        public bool ListRetrievedFlagSpecified { get; set; }
        public DocumentReference DocumentReference { get; set; }
    }
    public class OrderSummayModel
    {
        public string CustomerId { get; set; }
        public string StateCode { get; set; }
        public string PillarId { get; set; }
        public string BatchId { get; set; }
        public string Timestamp { get; set; }
    }
    public class Authentic
    {
        public string B2BAccount { get; set; }
        public string Password { get; set; }
        public string SecurityToken { get; set; }
    }
    public class OperatingSystemModel
    {
        public string ServicePack { get; set; }
        public string SystemName { get; set; }
        public string SystemType { get; set; }
        public string Version { get; set; }
    }
    public class VendorDetailsModel
    {
        public string Software { get; set; }
        public string Vendor { get; set; }
        public string Version { get; set; }
    }
    public class PlaceOrderModel
    {
        public string CustomerId { get; set; }
        public string StateCode { get; set; }
        public string PillarId { get; set; }
        public string OrderType { get; set; }
        public string Order { get; set; }
        public string fileName { get; set; }
    }
    public class PlaceOrderResponseModel
    {
        public string BatchId { get; set; }
        public string Comments { get; set; }
        public string Timestamp { get; set; }
    }
}
