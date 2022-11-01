using Coyote.Console.Common;
using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class ReportSchedulerResponseModel : ReportRequestModel
    {
        public long? ID { get; set; }
        public string FilterName { get; set; }
        public List<int> UserIds { get; set; }

        public string TillIds { get; set; }
        public int ReportId { get; set; }

        public string ReportName { get; set; }

        [Required]
        public DateTime InceptionDate { get; set; }
        [Required]
        public string InceptionTime { get; set; }

        [Required]
        public SchedulerInterval IntervalInd { get; set; }
        public string IntervalIndex { get; set; }
        [Required]
        public int IntervalBracket { get; set; }
        public DateTime? LastRun { get; set; }
        public bool? ExcelExport { get; set; } = false;
        public bool? PdfExport { get; set; } = false;
        public bool? CsvExport { get; set; } = false;

        public Status IsActive { get; set; }


        //Sales Summary
        public bool stockNegativeOH { get; set; } = false;
        public bool stockSOHLevel { get; set; } = false;
        public bool stockSOHButNoSales { get; set; } = false;
        public bool stockLowWarn { get; set; } = false;
        public int? stockNoOfDaysWarn { get; set; }
        public bool SplitOverOutlet { get; set; } = false;
        public bool OrderByAmt { get; set; } = false;
        public bool OrderByQty { get; set; } = false;
        public bool OrderByGP { get; set; } = false;
        public bool OrderByMargin { get; set; } = false;
        public bool OrderBySOH { get; set; } = false;
        public bool OrderByAlp { get; set; } = false;
        public int? SalesSOH { get; set; }
        public float? salesSOHRange { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public List<UserDetailResponseModel> UserList { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only


        public string FilterBody { get; set; }

    }
}
