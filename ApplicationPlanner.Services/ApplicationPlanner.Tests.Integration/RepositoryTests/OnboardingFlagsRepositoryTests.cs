using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class OnboardingFlagsRepositoryTests : TestBase
    {
        private readonly OnboardingFlagsRepository _onboardingFlagsRepository;
        private readonly QARepository _qaRepository;

        public OnboardingFlagsRepositoryTests()
        {
            _onboardingFlagsRepository = new OnboardingFlagsRepository(_sql);
            _qaRepository = new QARepository(_sql);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Repository")]
        public async Task GetByPortfolioIdAsync_return_correct_output()
        {
            // Arrange:
            // Reset Flags
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(integrationTestPortfolioId);
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(integrationTestPortfolioId);

            // Act:
            // 2. Get
            var result = await _onboardingFlagsRepository.GetByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsFalse(result.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode);
            Assert.IsFalse(result.HasSeenCartTooltipForTranscriptsInSearchMode);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Repository")]
        public async Task GetByEducatorIdAsync_return_correct_output()
        {
            // Arrange:
            // Reset Flags
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(integrationTestEducatorId);
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(integrationTestEducatorId);

            // Act:
            // 2. Get
            var result = await _onboardingFlagsRepository.GetByEducatorIdAsync(integrationTestEducatorId);

            // Assert:
            Assert.IsFalse(result.FirstOrDefault(x => x.KeyName == OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString()).Displayed);
            Assert.IsFalse(result.FirstOrDefault(x => x.KeyName == OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString()).Displayed);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Repository")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync_success()
        {
            // Arrange:
            // Reset
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(integrationTestPortfolioId);

            // Act:
            // Set
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsTrue(result.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Repository")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync_success()
        {
            // Arrange:
            // Reset
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(integrationTestEducatorId);

            // Act:
            // Set
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(integrationTestEducatorId);

            // Assert:
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Repository")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync_success()
        {
            // Arrange:
            // Reset
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(integrationTestPortfolioId);

            // Act:
            // Set
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsTrue(result.HasSeenCartTooltipForTranscriptsInSearchMode);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Repository")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync_success()
        {
            // Arrange:
            // Reset
            await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(integrationTestEducatorId);

            // Act:
            // Set
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(integrationTestEducatorId);

            // Assert:
            Assert.IsTrue(result > 0);
        }
    }
}
