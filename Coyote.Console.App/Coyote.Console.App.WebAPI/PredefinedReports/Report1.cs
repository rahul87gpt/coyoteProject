using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.Expressions;
using DevExpress.XtraReports.UI;

namespace Coyote.Console.App.WebAPI.PredefinedReports
{
    public partial class Report1
    {

        public Report1()
        {
            InitializeComponent();
        }

        private void Report1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            //DrillDownExpanded = !DrillDownExpanded;
            Report1 report = new Report1();
            List<DetailReportBand> bands = report.AllControls<DetailReportBand>().ToList();
            foreach (DetailReportBand item in bands)
                item.DrillDownExpanded = false;
             //report.CreateDocument();
            //DocumentViewer1.DocumentSource = report;
        }
    }
}
