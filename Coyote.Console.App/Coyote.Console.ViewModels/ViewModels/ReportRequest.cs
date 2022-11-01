using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic; 
using System.Text;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class ReportRequest: PagedInputModel
    {
        public long? ProductNumberFrom { get; set; }
        public long? ProductNumberTo { get; set; }
        //TODO: Days of week is not used anywhere in old system
        public List<Enums.DayOfWeek> DaysOfWeek { get; set; }
        public int TillId { get; set; }
        public List<int> Departments { get; set; }
        public List<int> Commodities { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Groups { get; set; }
        public List<int> Suppliers { get; set; }
        public List<int> Manufacturers { get; set; }
        //TODO: in future scope
        //public List<int> Members { get; set; }
        public List<int> Outlets { get; set; }
        public List<int> Zones { get; set; }
        public bool Summary { get; set; }
        public bool DrillDown { get; set; }
        public bool Chart { get; set; }
        public bool Continous { get; set; }
        public string StoreIds { get; set; }
        public string SupplierIds { get; set; }
        public string ZoneIds { get; set; }
        public string MemberIds { get; set; }
    }


    
}
