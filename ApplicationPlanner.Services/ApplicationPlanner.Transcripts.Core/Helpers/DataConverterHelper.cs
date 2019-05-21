using System.Collections.Generic;
using System.Data;

namespace ApplicationPlanner.Transcripts.Core.Helpers
{
    public class DataConverterHelper
    {
        public static DataTable GetDataTableFromListOfInteger(IEnumerable<int> list, string column)
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
                {
                    new DataColumn(column, typeof (int))
                });
            if (list != null)
            {
                foreach (var el in list)
                {
                    dt.Rows.Add(el);
                }
            }
            return dt;
        }
       
    }
}
