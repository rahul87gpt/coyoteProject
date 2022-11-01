using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class RebateHeaderResponseModel
    { 
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerCode { get; set; }
        public string ManufacturerDesc { get; set; }
        public bool ManufacturerIsDeleted { get; set; }
        public int ZoneId { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneDesc { get; set; }
        public bool ZoneIsDeleted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
