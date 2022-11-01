using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
  public class HOSTUPDChangeResponseModel
    {
        public int? Outletcode { get; set; }
        public string Hostcode { get; set; }

        public string ChangeType { get; set; }

        public string Productdesc { get; set; }
        public long? Productcode { get; set; }
        public long Id { get; set; }
        public int HostUpdId { get; set; }
        public int HostId { get; set; }
      
        public int ChangeTypeId { get; set; }

      

        public long? ProductId { get; set; }

       

      
        public int? PromotionId { get; set; }
        public int? OutletId { get; set; }

      
        public bool? Status { get; set; }
        public float CtnCostBefore { get; set; }
        public float CtnCostAfter { get; set; }
        public float Price1Before { get; set; }
        public float CtnCostSuggested { get; set; }
        public float CtnQtyBefore { get; set; }
        public float CtnQtyAfter { get; set; }

        public long HostUpdTimeStamp { get; set; }

        
        public System.DateTime CreatedAt { get; set; }
       
        public System.DateTime UpdatedAt { get; set; }
        
        public int CreatedById { get; set; }
       
        public int UpdatedById { get; set; }

        public bool IsDeleted { get; set; }

        public string Promocode { get; set; }
        public string PromoDesc { get; set; }
    }
}
