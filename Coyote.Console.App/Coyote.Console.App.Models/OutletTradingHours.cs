using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("OutletTradingHours")]
    public class OutletTradingHours : IKeyIdentifier<int>, IAuditable<int>
    {
        public int Id { get; set; }
        public int OuteltId { get; set; }
        public Store Outelt { get; set; }

        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string MonOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string MonCloseTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string TueOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string TueCloseTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string WedOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string WedCloseTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string ThuOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string ThuCloseTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string FriOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string FriCloseTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string SatOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string SatCloseTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string SunOpenTime { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string SunCloseTime { get; set; }

        public int CreatedById { get; set; }
        public Users CreatedBy { get; set; }
        public int UpdatedById { get; set; }
        public Users UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
