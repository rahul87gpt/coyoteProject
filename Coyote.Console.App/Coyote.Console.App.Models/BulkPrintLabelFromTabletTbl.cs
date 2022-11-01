using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("BulkPrintLabelFromTabletTbl")]
    public class BulkPrintLabelFromTabletTbl
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public int BPLT_Outlet { get; set; }
        public float BPLT_Product_Number { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime BPLT_Print_Batch { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BPLT_Last_Import { get; set; }
    }
#pragma warning restore CA1707 // Identifiers should not contain underscores
}

