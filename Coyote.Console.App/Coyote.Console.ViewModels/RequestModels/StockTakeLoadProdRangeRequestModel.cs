using System;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;
using Microsoft.AspNetCore.Http;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class StockTakeLoadProdRangeRequestModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public int OutLetId { get; set; }
        public long? ProductStartId { get; set; }
        public long? ProductEndId { get; set; }
        public string ProductIds { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string DayRange { get; set; }
        public long? TillId { get; set; }
    }
}
