using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class HostProcessingRequestModel
    {
        public bool HoldGDP { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public List<Host> Hosts { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }

    public class Host
    {
        [Required]
        public int HostSettingID { get; set; }
        [Required]
        public HostType HostType { get; set; }
    }
}
