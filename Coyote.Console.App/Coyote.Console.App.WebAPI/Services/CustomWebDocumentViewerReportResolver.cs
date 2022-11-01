using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace Coyote.Console.App.WebAPI
{
    public class CustomWebDocumentViewerReportResolver : IWebDocumentViewerReportResolver
    {
        public XtraReport Resolve(string reportEntry )
       {
           
            XtraReport rep = CreateReport();
            rep.DataSource = CreateObjectDataSource();
            return rep;
        }
       
        private object CreateObjectDataSource()
        {
            
                ObjectDataSource dataSource = new ObjectDataSource();

                dataSource.Name = "SaleObjectDS";
                dataSource.DataSource = typeof(DataSources.SalesReport);



                dataSource.Constructor = ObjectConstructorInfo.Default;
            dataSource.DataMember = "Filter";
                return dataSource;
            
        }
      
        private XtraReport CreateReport()
        {
            
                XtraReport report = new PredefinedReports.Report1();
                return report;
           
        }
    }
}
