using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class GlobalSettingRepositoryTests : TestBase
    {
        private readonly GlobalSettingRepository _globalSettingRepository;
        private readonly QARepository _qaRepository;

        public GlobalSettingRepositoryTests()
        {
            _globalSettingRepository = new GlobalSettingRepository(_sql);
            _qaRepository = new QARepository(_sql);
        }

        [TestMethod]
        [TestCategory("GlobalSetting Repository")]
        public async Task GlobalSettingHasSeenCartTooltipForTranscriptsInSavedSchoolsMode_Save()
        {
            // Arrange:

            // Act:
            // 1. Reset
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(integrationTestPortfolioId);
            // 2. Set
            var result = await _globalSettingRepository.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsTrue(result.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode);
        }

        [TestMethod]
        [TestCategory("GlobalSetting Repository")]
        public async Task GlobalSettingHasSeenCartTooltipForTranscriptsInSearchMode_Save()
        {
            // Arrange:

            // Act:
            // 1. Reset
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(integrationTestPortfolioId);
            // 2. Set
            var result = await _globalSettingRepository.SaveHasSeenCartTooltipForTranscriptsInSearchModeAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsTrue(result.HasSeenCartTooltipForTranscriptsInSearchMode);
        }
    }
}
