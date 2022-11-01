using Coyote.Console.Common.Exceptions;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Coyote.Console.Common
{
    public static class FastReportHelper
    {
        public static DataSet ConvertToDataSet<T>(this IEnumerable<T> source, string name)
        {
            if (source == null)
                throw new NullReferenceCustomException(ErrorMessages.DataSourceIsEmpty.ToString(CultureInfo.CurrentCulture));
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceCustomException(ErrorMessages.DataSetNameIsEmpty.ToString(CultureInfo.CurrentCulture));
            var converted = new DataSet(name);
            converted.Tables.Add(NewTable(name, source));
            return converted;
        }

        private static DataTable NewTable<T>(string name, IEnumerable<T> list)
        {
            PropertyInfo[] propInfo = typeof(T).GetProperties();
            DataTable table = Table<T>(name, propInfo);
            IEnumerator<T> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
                table.Rows.Add(CreateRow<T>(table.NewRow(), enumerator.Current, propInfo));
            return table;
        }

        private static DataRow CreateRow<T>(DataRow row, T listItem, PropertyInfo[] pi)
        {
            foreach (PropertyInfo p in pi)
                row[p.Name.ToString()] = p.GetValue(listItem, null);
            return row;
        }

        private static DataTable Table<T>(string name, PropertyInfo[] pi)
        {
            DataTable table = new DataTable(name);
            foreach (PropertyInfo p in pi)
            {
                table.Columns.Add(p.Name, p.PropertyType);
            }
            return table;
        }
    }
}