using System;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class CostPriceZonesResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }    
        public CostPriceZoneType Type { get; set; }     
        public string Description { get; set; }      
        public int HostSettingID { get; set; }
        public string HostSetting { get; set; }
        public float Factor1 { get; set; }
        public float Factor2 { get; set; }
        public float Factor3 { get; set; }    
        public bool SuspUpdOutlet { get; set; }
    }
}
