using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class LinqWrapperServiceUnitTests
    {
        [TestInitialize]
        public void Test_Init()
        {
        }

        [TestMethod]
        [TestCategory("Linq Wrapper Service")]
        public void GetLinqedList_should_return_list_that_meet_requirements()
        {
            // Arrange
            var completeList = new List<TranscriptRequestStudentViewModel>
            {
                new TranscriptRequestStudentViewModel { Id = 1, StudentName = "Valeria", StudentId = "123" },
                new TranscriptRequestStudentViewModel { Id = 2, StudentName = "Victor", StudentId = "124" },
                new TranscriptRequestStudentViewModel { Id = 3, StudentName = "Alex", StudentId = "125" },
                new TranscriptRequestStudentViewModel { Id = 4, StudentName = "Graham", StudentId = "126" },
                new TranscriptRequestStudentViewModel { Id = 5, StudentName = "Fahad", StudentId = "127" },
                new TranscriptRequestStudentViewModel { Id = 6, StudentName = "Jeanne", StudentId = "128" },
                new TranscriptRequestStudentViewModel { Id = 7, StudentName = "Jeanne", StudentId = "128" }
            };
            var predicate = new Func<TranscriptRequestStudentViewModel, bool>(x => x.StudentName.ToLower().Contains("12") || x.StudentId.ToLower().Contains("12"));

            // Act
            var result = CreateService().GetLinqedList(completeList, predicate, "", "", "studentName", Transcripts.Web.Models.SortOrder.ASC, 1, 2);

            // Assert
            Assert.AreEqual(result.Count, 7); // send back the total count
            Assert.AreEqual(result.Items.Count(), 2); // take parameter
            Assert.AreEqual(result.Items.ToList()[0].Id, 5); // skip parameter (will skip Alex and send Fahad)
        }

        [TestMethod]
        [TestCategory("Linq Wrapper Service")]
        public void GetMatchQueryList_should_return_list_that_matches_studentName()
        {
            // Arrange
            var listToSearch = new List<TranscriptRequestStudentViewModel>
            {
                new TranscriptRequestStudentViewModel { Id = 1, StudentName = "Valeria", StudentId = "123" },
                new TranscriptRequestStudentViewModel { Id = 2, StudentName = "Victor", StudentId = "124" },
                new TranscriptRequestStudentViewModel { Id = 3, StudentName = "Alex", StudentId = "125" },
                new TranscriptRequestStudentViewModel { Id = 4, StudentName = "Graham", StudentId = "126" },
                new TranscriptRequestStudentViewModel { Id = 5, StudentName = "Fahad", StudentId = "127" },
                new TranscriptRequestStudentViewModel { Id = 6, StudentName = "Jeanne", StudentId = "128" },
                new TranscriptRequestStudentViewModel { Id = 7, StudentName = "Jeanne", StudentId = "128" }
            };
            var predicate = new Func<TranscriptRequestStudentViewModel, bool>(x => x.StudentName.ToLower().Contains("jeanne") || x.StudentId.ToLower().Contains("jeanne"));

            // Act
            var result = CreateService().GetMatchQueryList(listToSearch, predicate).ToList();

            // Assert
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        [TestCategory("Linq Wrapper Service")]
        public void GetMatchQueryList_should_return_list_that_matches_studentId()
        {
            // Arrange
            var listToSearch = new List<TranscriptRequestStudentViewModel>
            {
                new TranscriptRequestStudentViewModel { Id = 1, StudentName = "Valeria", StudentId = "123" },
                new TranscriptRequestStudentViewModel { Id = 2, StudentName = "Victor", StudentId = "124" },
                new TranscriptRequestStudentViewModel { Id = 3, StudentName = "Alex", StudentId = "125" },
                new TranscriptRequestStudentViewModel { Id = 4, StudentName = "Graham", StudentId = "126" },
                new TranscriptRequestStudentViewModel { Id = 5, StudentName = "Fahad", StudentId = "127" },
                new TranscriptRequestStudentViewModel { Id = 6, StudentName = "Jeanne", StudentId = "128" },
                new TranscriptRequestStudentViewModel { Id = 7, StudentName = "Jeanne", StudentId = "128" }
            };
            var predicate = new Func<TranscriptRequestStudentViewModel, bool>(x => x.StudentName.ToLower().Contains("127") || x.StudentId.ToLower().Contains("127"));

            // Act
            var result = CreateService().GetMatchQueryList(listToSearch, predicate).ToList();

            // Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        [TestCategory("Linq Wrapper Service")]
        public void GetFilteredList_should_return_list_that_is_filtered()
        {
            // Arrange
            var listToFilter = new List<TranscriptRequestStudentViewModel>
            {
                new TranscriptRequestStudentViewModel { Id = 1, StudentName = "Valeria", StudentId = "123" },
                new TranscriptRequestStudentViewModel { Id = 2, StudentName = "Victor", StudentId = "124" },
                new TranscriptRequestStudentViewModel { Id = 3, StudentName = "Alex", StudentId = "125" },
            };

            // Act
            var result = CreateService().GetFilteredList(listToFilter, "studentName", "Alex").ToList();

            // Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        [TestCategory("Linq Wrapper Service")]
        public void GetSortedList_should_return_list_that_is_sorted()
        {
            // Arrange
            var listToSort = new List<TranscriptRequestStudentViewModel>
            {
                new TranscriptRequestStudentViewModel { Id = 1, StudentName = "Valeria", StudentId = "123" },
                new TranscriptRequestStudentViewModel { Id = 2, StudentName = "Victor", StudentId = "124" },
                new TranscriptRequestStudentViewModel { Id = 3, StudentName = "Alex", StudentId = "125" },
            };

            // Act
            var result = CreateService().GetSortedList(listToSort, "studentName", Transcripts.Web.Models.SortOrder.ASC).ToList();

            // Assert
            Assert.AreEqual(result[0].Id, 3);
            Assert.AreEqual(result[1].Id, 1);
            Assert.AreEqual(result[2].Id, 2);
        }

        private LinqWrapperService CreateService()
        {
            return new LinqWrapperService();
        }
    }
}
