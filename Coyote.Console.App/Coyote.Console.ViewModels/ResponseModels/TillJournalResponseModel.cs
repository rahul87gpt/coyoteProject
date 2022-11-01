using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class TillJournalResponseModel
    {
        public int OutletId { get; set; }
        public int TillId { get; set; }
        public long TransactionNo { get; set; }
        public string TransactionType { get; set; }
        public float TransactionAmount { get; set; }
        public int CashierId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime JournalDate { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public long ProductId { get; set; }
        public long ProductNumber { get; set; }
        public string ProductDesc { get; set; }
        public string Desc { get; set; }
        public float Quantity { get; set; }
        public float Amount { get; set; }
        public float DiscountAmount { get; set; }
        public float Cost { get; set; }
        public int PromoSellId { get; set; }
        public int PromoMixMatchId { get; set; }
        public int PromoOfferId { get; set; }
        public int Sequence { get; set; }
        public long APNSold { get; set; }
      
        public string JournalStatus { get; set; }
    }
}
