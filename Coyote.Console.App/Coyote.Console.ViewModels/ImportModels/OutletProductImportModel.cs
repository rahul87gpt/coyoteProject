using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class OutletProductImportModel
    {
        public string StoreCode { get; set; }

        public long ProductNumber { get; set; }

        public string SupplierCode { get; set; }
        public bool Status { get; set; }

        public bool Till { get; set; }
        public bool OpenPrice { get; set; }
        public float NormalPrice1 { get; set; }
        public float NormalPrice2 { get; set; }
        public float NormalPrice3 { get; set; }
        public float NormalPrice4 { get; set; }
        public float NormalPrice5 { get; set; }
        public float CartonCost { get; set; }

        public float CartonCostHost { get; set; }
        public float CartonCostInv { get; set; }
        public float CartonCostAvg { get; set; }
        public string SellPromoCode { get; set; }

        public string BuyPromoCode { get; set; }

        public float PromoPrice1 { get; set; }
        public float PromoPrice2 { get; set; }
        public float PromoPrice3 { get; set; }
        public float PromoPrice4 { get; set; }
        public float PromoPrice5 { get; set; }
        public float PromoCartonCost { get; set; }
        public float QtyOnHand { get; set; }

        public float MinOnHand { get; set; }

        public float MaxOnHand { get; set; }
        public float MinReorderQty { get; set; }
        public int PickingBinNo { get; set; }
        public bool ChangeLabelInd { get; set; }
        public bool ChangeTillInd { get; set; }
        public string HoldNorm { get; set; }

        public DateTime ChangeLabelPrinted { get; set; }
        public float LabelQty { get; set; }
        public bool ShortLabelInd { get; set; }
        public bool SkipReorder { get; set; }

        public float SpecPrice { get; set; }
        public string SpecCode { get; set; }

        public DateTime SpecFrom { get; set; }
        public DateTime SpecTo { get; set; }
        public string GenCode { get; set; }

        public float SpecCartonCost { get; set; }
        public float ScalePlu { get; set; }
        public bool FifoStock { get; set; }
        public float Mrp { get; set; }
    }
}
