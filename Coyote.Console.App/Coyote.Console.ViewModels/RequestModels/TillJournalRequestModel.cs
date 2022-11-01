using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class TillJournalRequestModel //: PagedInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //Jounal Filters
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string TransactionType { get; set; }
        public long? TillId { get; set; }
        public long? CashierId { get; set; }
        public bool IsPromoSale { get; set; }
        public int PromoId { get; set; }
        public string StoreIds { get; set; }
        public string DocketNo { get; set; }
        public string MemberIds { get; set; }
    }

    public class TillJournalInputModel : PagedInputModel
    {
        public string JournalType { get; set; }
        public bool ShowException { get; set; } = false;
    }

    public class MasterListFilterModel : PagedInputModel
    {
        public bool ZoneOutlet { get; set; }
        public bool Dashboard { get; set; }
    }
}
