using DevExpress.DataAccess.Web;
using System;
using System.Collections.Generic;
using Coyote.Console.App.WebAPI.DataSources;

namespace Coyote.Console.App.WebAPI.Services {
    public class ObjectDataSourceWizardCustomTypeProvider : IObjectDataSourceWizardTypeProvider {
        public IEnumerable<Type> GetAvailableTypes(string context) {
            return new[] { typeof(SalesReport) };
        }
    }
}
