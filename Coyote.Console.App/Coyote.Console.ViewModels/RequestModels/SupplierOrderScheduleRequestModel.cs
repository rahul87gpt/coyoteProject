using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class SupplierOrderScheduleRequestModel
    {
        public int? Id { get; set; }
        public int StoreId { get; set; }
        public int SupplierId { get; set; }
        public DaysOfWeek DOWGenerateOrder { get; set; }
        public string GenerateOrderDOW { get; set; }
        public int SendOrderOffset { get; set; }
        public int ReceiveOrderOffset { get; set; }
        public DateTime? LastRun { get; set; }
        public int InvoiceOrderOffset { get; set; }
        public float DiscountThresholdOne { get; set; }
        public float DiscountThresholdTwo { get; set; }
        public float DiscountThresholdThree { get; set; }
        public int CoverDaysDiscountThreshold1 { get; set; }
        public int CoverDaysDiscountThreshold2 { get; set; }
        public int CoverDaysDiscountThreshold3 { get; set; }
        public int CoverDays { get; set; } = 0;
        public bool MultipleOrdersInAWeek { get; set; } = false;
        public bool OrderNonDefaultSupplier { get; set; } = false;
    }
}
