using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("OutletSupplierSchedule")]
    public class SupplierOrderingSchedule : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public int DOWGenerateOrder { get; set; }
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
        public int CreatedById { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
