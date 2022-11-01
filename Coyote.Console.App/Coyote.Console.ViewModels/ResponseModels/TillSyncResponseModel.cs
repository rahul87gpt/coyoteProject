using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class TillSyncResponseModel
    {
        public int Id { get; set; }
        public int TillId { get; set; }
        public string TillCode { get; set; }
        public string TillDesc { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public string ProductSync { get; set; }
        public string CashierSync { get; set; }
        public string AccountSync { get; set; }
        public string KeypadSync { get; set; }
        public DateTime? TillActivity { get; set; }
        [MaxLength(MaxLengthConstants.MaxTillVersionLength)]
        public string ClientVersion { get; set; }
        [MaxLength(MaxLengthConstants.MaxTillVersionLength)]
        public string PosVersion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
    }
}
