using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class ZoneOutletResponseModel
    {
        public int ZoneId { get; set; }

        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }

        public List<ZoneOutletStore> Stores { get; set; }
    }

    public class ZoneOutletStore
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public bool IsSelected { get; set; }
    }
}
