using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Coyote.Console.App.Services.Helper.EdiMetcashExtensions
{
    [XmlRoot(ElementName = "TransmissionHeader")]
    public class TransmissionHeader
    {

        [XmlElement(ElementName = "TransmissionType")]
        public string TransmissionType { get; set; }

        [XmlElement(ElementName = "SourceState")]
        public string SourceState { get; set; }

        [XmlElement(ElementName = "Destination")]
        public string Destination { get; set; }

        [XmlElement(ElementName = "CreationDate")]
        public DateTime CreationDate { get; set; }

        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "Header")]
    public class Header
    {

        [XmlElement(ElementName = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [XmlElement(ElementName = "CustomerName")]
        public string CustomerName { get; set; }

        [XmlElement(ElementName = "Street")]
        public string Street { get; set; }

        [XmlElement(ElementName = "Suburb")]
        public string Suburb { get; set; }

        [XmlElement(ElementName = "PostCode")]
        public int PostCode { get; set; }

        [XmlElement(ElementName = "OrderNumber")]
        public string OrderNumber { get; set; }

        [XmlElement(ElementName = "Message")]
        public string Message { get; set; }

        [XmlElement(ElementName = "InvoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [XmlElement(ElementName = "BusinessPillar")]
        public string BusinessPillar { get; set; }
    }

    [XmlRoot(ElementName = "ProductMLC")]
    public class ProductMLC
    {

        [XmlElement(ElementName = "CompanyCode")]
        public int CompanyCode { get; set; }

        [XmlElement(ElementName = "CommodityNumber")]
        public int CommodityNumber { get; set; }
    }

    [XmlRoot(ElementName = "ProductMSC")]
    public class ProductMSC
    {

        [XmlElement(ElementName = "DepartmentNumber")]
        public int DepartmentNumber { get; set; }

        [XmlElement(ElementName = "CategoryNumber")]
        public int CategoryNumber { get; set; }

        [XmlElement(ElementName = "CommodityNumber")]
        public int CommodityNumber { get; set; }

        [XmlElement(ElementName = "SubCommodityNumber")]
        public int SubCommodityNumber { get; set; }
    }

    [XmlRoot(ElementName = "GTINPLU")]
    public class GTINPLU
    {

        [XmlElement(ElementName = "Number")]
        public double Number { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "GTINPLUUnit")]
    public class GTINPLUUnit
    {

        [XmlElement(ElementName = "GTINPLU")]
        public GTINPLU GTINPLU { get; set; }

        [XmlAttribute(AttributeName = "Multiple")]
        public double Multiple { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "GTINPLUS")]
    public class GTINPLUS
    {

        [XmlElement(ElementName = "GTINPLUUnit")]
        public GTINPLUUnit GTINPLUUnit { get; set; }
    }

    [XmlRoot(ElementName = "Detail")]
    public class Detail
    {

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Indicator")]
        public string Indicator { get; set; }

        [XmlElement(ElementName = "ProductMLC")]
        public ProductMLC ProductMLC { get; set; }

        [XmlElement(ElementName = "ProductMSC")]
        public ProductMSC ProductMSC { get; set; }

        [XmlElement(ElementName = "SupplierCode")]
        public string SupplierCode { get; set; }

        [XmlElement(ElementName = "ProductDescription")]
        public string ProductDescription { get; set; }

        [XmlElement(ElementName = "ProductType")]
        public string ProductType { get; set; }

        [XmlElement(ElementName = "InvoicedProductCode")]
        public string InvoicedProductCode { get; set; }

        [XmlElement(ElementName = "OrderedProductCode")]
        public string OrderedProductCode { get; set; }

        [XmlElement(ElementName = "PackQuantity")]
        public float PackQuantity { get; set; }

        [XmlElement(ElementName = "PricingUnit")]
        public string PricingUnit { get; set; }

        [XmlElement(ElementName = "UnitsOrdered")]
        public int UnitsOrdered { get; set; }

        [XmlElement(ElementName = "UnitsInvoiced")]
        public int UnitsInvoiced { get; set; }

        [XmlElement(ElementName = "GSTRate")]
        public double GSTRate { get; set; }

        [XmlElement(ElementName = "WholesalePrice")]
        public double WholesalePrice { get; set; }

        [XmlElement(ElementName = "MarginAmount")]
        public double MarginAmount { get; set; }

        [XmlElement(ElementName = "GSTAmount")]
        public double GSTAmount { get; set; }

        [XmlElement(ElementName = "CostTotal")]
        public float CostTotal { get; set; }

        [XmlElement(ElementName = "ExtendedWholesaleTotal")]
        public double ExtendedWholesaleTotal { get; set; }

        [XmlElement(ElementName = "ExtendedMarginTotal")]
        public double ExtendedMarginTotal { get; set; }

        [XmlElement(ElementName = "ExtendedGSTTotal")]
        public double ExtendedGSTTotal { get; set; }

        [XmlElement(ElementName = "ExtendedCostTotal")]
        public double ExtendedCostTotal { get; set; }

        [XmlElement(ElementName = "LandedUnitCost")]
        public float LandedUnitCost { get; set; }

        [XmlElement(ElementName = "RetailPrice")]
        public double RetailPrice { get; set; }

        [XmlElement(ElementName = "MarginPercent")]
        public double MarginPercent { get; set; }

        [XmlElement(ElementName = "GTINPLUS")]
        public GTINPLUS GTINPLUS { get; set; }

        [XmlAttribute(AttributeName = "LineNumber")]
        public int LineNumber { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CategorySummary")]
    public class CategorySummary
    {

        [XmlElement(ElementName = "StandardCost")]
        public double StandardCost { get; set; }

        [XmlElement(ElementName = "Margin")]
        public double Margin { get; set; }

        [XmlElement(ElementName = "GST")]
        public double GST { get; set; }

        [XmlElement(ElementName = "TotalCost")]
        public double TotalCost { get; set; }

        [XmlElement(ElementName = "Retail")]
        public double Retail { get; set; }

        [XmlElement(ElementName = "GrossProfitPercent")]
        public double GrossProfitPercent { get; set; }

        [XmlAttribute(AttributeName = "Category")]
        public string Category { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ServiceLevelItem")]
    public class ServiceLevelItem
    {

        [XmlElement(ElementName = "AllItems")]
        public double AllItems { get; set; }

        [XmlElement(ElementName = "Promotions")]
        public double Promotions { get; set; }

        [XmlAttribute(AttributeName = "Category")]
        public string Category { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Trailer")]
    public class Trailer
    {

        [XmlElement(ElementName = "ExtendedWholesaleTotal")]
        public double ExtendedWholesaleTotal { get; set; }

        [XmlElement(ElementName = "ExtendedMarginTotal")]
        public double ExtendedMarginTotal { get; set; }

        [XmlElement(ElementName = "TotalFees")]
        public double TotalFees { get; set; }

        [XmlElement(ElementName = "TotalExGST")]
        public double TotalExGST { get; set; }

        [XmlElement(ElementName = "TotalGST")]
        public float TotalGST { get; set; }

        [XmlElement(ElementName = "Total")]
        public float Total { get; set; }

        [XmlElement(ElementName = "TotalWeight")]
        public double TotalWeight { get; set; }

        [XmlElement(ElementName = "NumberOfCases")]
        public double NumberOfCases { get; set; }

        [XmlElement(ElementName = "DamagedStockAllowance")]
        public double DamagedStockAllowance { get; set; }

        [XmlElement(ElementName = "ServiceFee")]
        public double ServiceFee { get; set; }

        [XmlElement(ElementName = "CategorySummary")]
        public CategorySummary CategorySummary { get; set; }

        [XmlElement(ElementName = "ServiceLevelItem")]
#pragma warning disable CA2227 // Collection properties should be read only
        public List<ServiceLevelItem> ServiceLevelItem { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }

    [XmlRoot(ElementName = "Invoice")]
    public class Invoice
    {

        [XmlElement(ElementName = "Header")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "Detail")]
#pragma warning disable CA2227 // Collection properties should be read only
        public List<Detail> Detail { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        [XmlElement(ElementName = "Trailer")]
        public Trailer Trailer { get; set; }

        [XmlAttribute(AttributeName = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "InvoiceBatch")]
    public class InvoiceBatch
    {

        [XmlElement(ElementName = "TransmissionHeader")]
        public TransmissionHeader TransmissionHeader { get; set; }

        [XmlElement(ElementName = "Invoice")]
#pragma warning disable CA2227 // Collection properties should be read only
        public List<Invoice> Invoice { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        [XmlAttribute(AttributeName = "ns0")]
        public string ns0 { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
