using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class AccessServiceUnitTests
    {
        private Mock<ISchoolSettingRepository> _mockSchoolSettingRepo;

        public AccessServiceUnitTests()
        {
            _mockSchoolSettingRepo = new Mock<ISchoolSettingRepository>();
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task CheckAccess_should_return_false_if_Transcripts_setup_not_complete()
        {
            // Arrange:
            var schoolSetting = (SchoolSettingModel)null;
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().CheckAccessAsync(1234, CountryType.US, new[] { 9, 10, 11, 12 });

            // Assert:
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task CheckAccess_should_return_false_if_IsTranscriptEnabled_false()
        {
            // Arrange:
            var schoolSetting = new SchoolSettingModel() { SchoolSettingId = 1, SchoolId = 1234, TranscriptProviderId = "cr-1234", IsTranscriptEnabled = false};
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().CheckAccessAsync(1234, CountryType.US, new[] { 9, 10, 11, 12 });

            // Assert:
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task CheckAccess_should_return_false_if_country_not_us()
        {
            // Arrange:
            var schoolSetting = new SchoolSettingModel() { SchoolSettingId = 1, SchoolId = 1234, TranscriptProviderId = "cr-1234", IsTranscriptEnabled = true };
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().CheckAccessAsync(1234, CountryType.Canada, new[] { 9, 10, 11, 12 });

            // Assert:
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task CheckAccess_should_return_false_if_grades_not_11_or_12()
        {
            // Arrange:
            var schoolSetting = new SchoolSettingModel() { SchoolSettingId = 1, SchoolId = 1234, TranscriptProviderId = "cr-1234", IsTranscriptEnabled = true };
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().CheckAccessAsync(1234, CountryType.US, new[] { 6, 7, 8 });

            // Assert:
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task CheckAccess_should_return_true()
        {
            // Arrange:
            var schoolSetting = new SchoolSettingModel() { SchoolSettingId = 1, SchoolId = 1234, TranscriptProviderId = "cr-1234", IsTranscriptEnabled = true };
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().CheckAccessAsync(1234, CountryType.US, new[] { 9, 10, 11, 12 });

            // Assert:
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task EducatorHasAccessToTranscripts_should_return_false_if_region()
        {
            // Arrange:
            var schoolSetting = new SchoolSettingModel() { SchoolSettingId = 1, SchoolId = 1234, TranscriptProviderId = "cr-1234", IsTranscriptEnabled = true };
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().EducatorHasAccessToTranscriptsAsync(1234, InstitutionTypeEnum.Region, CountryType.US, new[] { 9, 10, 11, 12 });

            // Assert:
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Access Service")]
        public async Task EducatorHasAccessToTranscripts_should_return_true_if_school()
        {
            // Arrange:
            var schoolSetting = new SchoolSettingModel() { SchoolSettingId = 1, SchoolId = 1234, TranscriptProviderId = "cr-1234", IsTranscriptEnabled = true };
            _mockSchoolSettingRepo.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(schoolSetting);

            // Act:
            var result = await CreateService().EducatorHasAccessToTranscriptsAsync(1234, InstitutionTypeEnum.School, CountryType.US, new[] { 9, 10, 11, 12 });

            // Assert:
            Assert.IsTrue(result);
        }

        private AccessService CreateService()
        {
            return new AccessService(_mockSchoolSettingRepo.Object);
        }
    }
}
