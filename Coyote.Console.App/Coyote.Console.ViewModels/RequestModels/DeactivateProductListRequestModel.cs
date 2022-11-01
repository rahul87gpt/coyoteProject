using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class DeactivateProductListRequestModel //: PagedInputModel
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int StoreId { get; set; }
        public string DepartmentIdsInc { get; set; }
        public string DepartmentIdsExc { get; set; }
        public string CommodityIdsInc { get; set; }
        public string CommodityIdsExc { get; set; }
        public bool QtyOnHandZero { get; set; }
        public string UserPassword { get; set; }
    }
}
