using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class UserActivityResponseModel
    {
        public int UserId { get; set; }
      //  public string UserNumber { get; set; }
        public string UserName { get; set; }
      //  public string OutletIds { get; set; }
        public long ProductId { get; set; }
    //    public long StoreId { get; set; }
        public long ProductNumber { get; set; }
        public string Decription { get; set; }
        public string Module { get; set; }
        public string Activity { get; set; }
        public string ActivityType { get; set; }
        public DateTime ActionAt { get; set; }
        public string UserRole { get; set; }

        public string UserFullName { get; set; }
        public string StoreIds { get; set; }
    }
}
