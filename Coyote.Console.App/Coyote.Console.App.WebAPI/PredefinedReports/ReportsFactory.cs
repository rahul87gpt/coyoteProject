using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.PredefinedReports
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["Report1"] = () => new PredefinedReports.Report1()
        };

        }
}
