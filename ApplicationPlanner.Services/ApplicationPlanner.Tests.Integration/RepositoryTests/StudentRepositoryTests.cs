using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class StudentRepositoryTests : TestBase
    {
        private readonly StudentRepository _studentRepository;

        public StudentRepositoryTests()
        {
            _studentRepository = new StudentRepository(_sql, _cache);
        }

        [TestMethod]
        [TestCategory("Student Repository")]
        public async Task StudentGeneralInfoGetByPortfolioIdAsync_should_return_general_info()
        {
            // Arrange:
            var studentGeneralInfo = new StudentGeneralInfoModel
            {
                Id = integrationTestPortfolioId,
                UserAccountId = 293945,
                GradeNumber = 11,
                FirstName = "ApplicationPlanner.API",
                AvatarFileName = null,
                SchoolCountryType = CC.Common.Enum.CountryType.US,
                HasSeenCartTooltipForTranscriptsInSavedSchoolsMode = true,
                HasSeenCartTooltipForTranscriptsInSearchMode = true,
            };

            // Act:
            var result = await _studentRepository.StudentGeneralInfoGetByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.AreEqual(studentGeneralInfo.Id, result.Id);
            Assert.AreEqual(studentGeneralInfo.UserAccountId, result.UserAccountId);
            Assert.AreEqual(studentGeneralInfo.GradeNumber, result.GradeNumber);
            Assert.AreEqual(studentGeneralInfo.FirstName, result.FirstName);
            Assert.AreEqual(studentGeneralInfo.AvatarFileName, result.AvatarFileName);
            Assert.AreEqual(studentGeneralInfo.SchoolCountryType, result.SchoolCountryType);
            Assert.AreEqual(studentGeneralInfo.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, result.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode);
            Assert.AreEqual(studentGeneralInfo.HasSeenCartTooltipForTranscriptsInSearchMode, result.HasSeenCartTooltipForTranscriptsInSearchMode);
        }
    }
}
