using System;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class HostSettingsResponseModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string InitialLoadFileWeekly { get; set; }
        public string WeeklyFile { get; set; }
        public int FilePathID { get; set; }
        public string FilePath { get; set; }
        public float NumberFactor { get; set; }
        public int SupplierID { get; set; }
        public string Supplier { get; set; }
        public int WareHouseID { get; set; }
        public string WareHouse { get; set; }
        public int HostFormatId { get; set; }
        public string HostFormatCode { get; set; }
        public string HostFormatName { get; set; }
        public string BuyPromoPrefix { get; set; }
        public string SellPromoPrefix { get; set; }
    }
}
