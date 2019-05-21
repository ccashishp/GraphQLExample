using ApplicationPlanner.Transcripts.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ApplicationPlanner.Tests.Unit.HelperTests
{
    [TestClass]
    public class DataConverterHelperUnitTests
    {
        [TestMethod]
        [TestCategory("DataConverter Helper")]
        public void GetDataTableFromListOfInteger_should_return_datatable_with_correct_column_name()
        {
            // Arrange:
            var list = new List<int>() { 1, 2, 3, 4 };
            var columnName = "Id";

            // Act:
            var result = DataConverterHelper.GetDataTableFromListOfInteger(list, columnName);

            // Assert:
            Assert.AreEqual(result.Columns.Count, 1);
            Assert.AreEqual(result.Columns[0].ColumnName, columnName);
            Assert.AreEqual(result.Columns[0].DataType, typeof(int));
            Assert.AreEqual(result.Rows.Count, 4);
        }
    }
}
