using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class PromoProductImportModel
    {
        //public int Id { get; set; }
        public string DESC { get; set; }
        public long APN { get; set; }
        public float COST { get; set; }
        public float Price1 { get; set; }
        public float Price2 { get; set; }
        public float Price3 { get; set; }
        public float Price4 { get; set; }
        public float AmtOff { get; set; }
        public int? Group { get; set; }
        public string Supplier { get; set; }
        public bool CostIsPromo { get; set; }
        public string HostPromoType { get; set; }

      //  public long? ProductId { get; set; }

    }
}
