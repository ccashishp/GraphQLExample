using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class TranscriptServiceUnitTests
    {
        private Mock<IAvatarService> _mockAvatarService;
        private readonly string _avatarUrl;
        private Mock<ITimeZoneRepository> _mockTimeZoneRepository;

        public TranscriptServiceUnitTests()
        {
            _avatarUrl = "https://test-storage.com/avatar.jpg"; ;
        }

        [TestInitialize]
        public void Test_Init()
        {
            _mockAvatarService = new Mock<IAvatarService>();
            _mockTimeZoneRepository = new Mock<ITimeZoneRepository>();
        }

        [TestMethod]
        [TestCategory("Transcript Service")]
        public async Task GetTranscriptImportedResponseModelAsync_should_set_all_properties()
        {
            // Arrange
            var input = new TranscriptImportedViewModel
            {
                Id = 15829045,
                UserAccountId = 5678,
                SchoolCountryType = CountryType.US,
                AvatarFileName = "",
                StudentName = "Todd, Motto",
                DateOfBirth = DateTime.Now.AddYears(-17),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = "STUD-1234",
                TranscriptId = 1,
                ImportedDate = DateTime.UtcNow
            };
            var timezoneDetails = new TimeZoneDetailModel { SQLKey = "Eastern Standard Time" };
            _mockAvatarService.Setup(x => x.GetStudentAvatarDefaultUrl()).Returns(_avatarUrl);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneIdByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneDetailByIdAsync(It.IsAny<int>())).ReturnsAsync(timezoneDetails);

            // Act
            var result = await CreateService().GetTranscriptImportedResponseModelAsync(new List<TranscriptImportedViewModel> { input }, 1234);

            // Assert
            Assert.AreEqual(input.Id, result.First().Id);
            Assert.AreEqual(_avatarUrl, result.First().AvatarUrl);
            Assert.AreEqual(input.StudentName, result.First().StudentName);
            Assert.AreEqual(input.DateOfBirth, result.First().DateOfBirth);
            Assert.AreEqual(input.GradeId, result.First().GradeId);
            Assert.AreEqual(input.GradeKey, result.First().GradeKey);
            Assert.AreEqual(input.StudentId, result.First().StudentId);
            Assert.AreEqual(input.TranscriptId, result.First().TranscriptId);
            Assert.AreEqual(DateTime.SpecifyKind(input.ImportedDate, DateTimeKind.Utc).ToLocalTime(), result.First().ImportedDate);
        }

        [TestMethod]
        [TestCategory("Transcript Service")]
        public async Task GetStudentTranscriptResponseModelAsync_should_set_all_properties()
        {
            // Arrange
            var input = new StudentTranscriptViewModel
            {
                Id = 15829045,
                UserAccountId = 5678,
                SchoolCountryType = CountryType.US,
                AvatarFileName = "",
                StudentName = "Todd, Motto",
                DateOfBirth = DateTime.Now.AddYears(-17),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = "STUD-1234",
                TranscriptId = null,
                ReceivedDateUtc = null
            };
            var timezoneDetails = new TimeZoneDetailModel { SQLKey = "Eastern Standard Time" };
            _mockAvatarService.Setup(x => x.GetStudentAvatarDefaultUrl()).Returns(_avatarUrl);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneIdByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneDetailByIdAsync(It.IsAny<int>())).ReturnsAsync(timezoneDetails);

            // Act
            var result = await CreateService().GetStudentTranscriptResponseModelAsync(new List<StudentTranscriptViewModel> { input });

            // Assert
            Assert.AreEqual(input.Id, result.First().Id);
            Assert.AreEqual(_avatarUrl, result.First().AvatarUrl);
            Assert.AreEqual(input.StudentName, result.First().StudentName);
            Assert.AreEqual(input.DateOfBirth, result.First().DateOfBirth);
            Assert.AreEqual(input.GradeId, result.First().GradeId);
            Assert.AreEqual(input.GradeKey, result.First().GradeKey);
            Assert.AreEqual(input.StudentId, result.First().StudentId);
            Assert.AreEqual(input.TranscriptId, result.First().TranscriptId);
            Assert.AreEqual(input.ReceivedDateUtc, result.First().ReceivedDateUtc);
        }

        private TranscriptService CreateService()
        {
            return new TranscriptService(_mockAvatarService.Object, _mockTimeZoneRepository.Object);
        }
    }
}
