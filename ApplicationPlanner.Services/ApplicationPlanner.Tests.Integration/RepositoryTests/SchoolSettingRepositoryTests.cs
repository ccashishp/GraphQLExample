using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class SchoolSettingRepositoryTests : TestBase
    {
        private readonly SchoolSettingRepository _schoolSettingRepository;

        public SchoolSettingRepositoryTests()
        {
            _schoolSettingRepository = new SchoolSettingRepository(_sql, _cache);
        }

        [TestMethod]
        [TestCategory("SchoolSetting Repository")]
        public async Task GetBySchoolIdAsync_should_return_setting_from_db()
        {
            // Arrange:
            // Clear the cache to make sure we are testing the sproc
            var cachekey = _cache.CreateKey("TranscriptsSchoolSettingGetBySchoolId", integrationTestSchoolId);
            await _cache.DeleteAsync(cachekey);
            var schoolSetting = new SchoolSettingModel {
                SchoolSettingId = 2,
                SchoolId = 250055,
                IsTranscriptEnabled = true,
                TranscriptProviderId = "credentials-250055"
            };

            // Act:
            var result = await _schoolSettingRepository.GetBySchoolIdAsync(integrationTestSchoolId);

            // Assert:
            Assert.AreEqual(schoolSetting.SchoolSettingId, result.SchoolSettingId);
            Assert.AreEqual(schoolSetting.SchoolId, result.SchoolId);
            Assert.AreEqual(schoolSetting.IsTranscriptEnabled, result.IsTranscriptEnabled);
            Assert.AreEqual(schoolSetting.TranscriptProviderId, result.TranscriptProviderId);
        }
    }
}
