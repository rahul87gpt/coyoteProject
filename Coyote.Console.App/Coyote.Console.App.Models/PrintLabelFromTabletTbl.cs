using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("PrintLabelFromTabletTbl")]
    public class PrintLabelFromTabletTbl
    {       
        public int PLT_Outlet { get; set; }
        public float PLT_Product_Number { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PLT_Print_Batch { get; set; }
    }
}
