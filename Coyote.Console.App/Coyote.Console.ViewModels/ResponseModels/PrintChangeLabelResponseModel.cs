using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class PrintChangeLabelResponseModel
    {
        public float? LabelQty { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public int? DefaultLabelId { get; set; }
        public string DefaultLabelType { get; set; }
        public string DefaultLabelTypeDesc { get; set; }
    }

    public class RePrintChangeLabelResponseModel
    {
        public float? LabelQty { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public DateTime? ChangeLabelPrinted { get; set; }
        public int? DefaultLabelId { get; set; }
        public string DefaultLabelType { get; set; }
        public string DefaultLabelTypeDesc { get; set; }
    }

    public class SpecPrintChangeLabelResponseModel
    {
        public float? LabelQty { get; set; }
        public float SpecPrice { get; set; }
        public DateTime? SpecFrom { get; set; }
        public DateTime? SpecTo { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public int? DefaultLabelId { get; set; }
        public string DefaultLabelType { get; set; }
        public string DefaultLabelTypeDesc { get; set; }
    }

    public class PrintLabelFromTableResponseModel
    {
        public int OutletId { get; set; }
        public int Outlet { get; set; }
        public string OutletDescription { get; set; }
        public DateTime BatchDateTime { get; set; }
        public int BatchCount { get; set; }
    }

    public class PrintLabelFromTabletPDEMode
    {
        public int Outlet { get; set; }
        public string OutletDescription { get; set; }
        public DateTime PrintBatchDateTime { get; set; }
        public DateTime LastImport { get; set; }
    }

}
