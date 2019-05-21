using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class OnboardingFlagsServiceUnitTests
    {
        private Mock<IOnboardingFlagsRepository> _mockOnboardingFlagsRepo;

        public OnboardingFlagsServiceUnitTests()
        {
            _mockOnboardingFlagsRepo = new Mock<IOnboardingFlagsRepository>();
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task GetByPortfolioIdAsync_should_return_default_value_model_if_not_exists()
        {
            // Arrange:
            var sut = CreateService();
            var expected = new OnboardingFlagsModel(sut._defaultOnboardingFlags);

            // Act:
            var result = await sut.GetByPortfolioIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task GetByPortfolioIdAsync_should_return_expected_result()
        {
            // Arrange:
            var sut = CreateService();
            var mockGlobalSettings = new GlobalSettingModel { HasSeenCartTooltipForTranscriptsInSavedSchoolsMode = true, HasSeenCartTooltipForTranscriptsInSearchMode = true };
            _mockOnboardingFlagsRepo.Setup(x => x.GetByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(mockGlobalSettings);
            var expected = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );

            // Act:
            var result = await sut.GetByPortfolioIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task GetByEducatorIdAsync_should_return_default_value_model_if_not_exists()
        {
            // Arrange:
            var sut = CreateService();
            var expected = new OnboardingFlagsModel(sut._defaultOnboardingFlags);

            // Act:
            var result = await sut.GetByEducatorIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task GetByEducatorIdAsync_should_return_expected_result()
        {
            // Arrange:
            var sut = CreateService();
            var mockEducatorHelpOverlay = new List<EducatorHelpOverlayModel> {
                new EducatorHelpOverlayModel { KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString(), Displayed = true },
                new EducatorHelpOverlayModel { KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString(), Displayed = true }
            };
            _mockOnboardingFlagsRepo.Setup(x => x.GetByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(mockEducatorHelpOverlay);
            var expected = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );

            // Act:
            var result = await sut.GetByEducatorIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync_should_call_repo()
        {
            // Arrange:
            var sut = CreateService();
            var expected = new OnboardingFlagsModel(sut._defaultOnboardingFlags);

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(1);

            // Assert:
            // Make sure the correct service method is called
            _mockOnboardingFlagsRepo.Verify(tr => tr.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(1), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync_should_return_correct_result()
        {
            // Arrange:
            var sut = CreateService();
            var mockGlobalSettings = new GlobalSettingModel { HasSeenCartTooltipForTranscriptsInSavedSchoolsMode = true, HasSeenCartTooltipForTranscriptsInSearchMode = true };
            _mockOnboardingFlagsRepo.Setup(x => x.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(mockGlobalSettings);
            _mockOnboardingFlagsRepo.Setup(x => x.GetByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(mockGlobalSettings);
            var expected = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync_should_call_repo()
        {
            // Arrange:
            var sut = CreateService();
            var expected = new OnboardingFlagsModel(sut._defaultOnboardingFlags);

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(1);

            // Assert:
            // Make sure the correct service method is called
            _mockOnboardingFlagsRepo.Verify(tr => tr.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(1), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync_should_return_correct_result()
        {
            // Arrange:
            var sut = CreateService();
            var mockEducatorHelpOverlay = new List<EducatorHelpOverlayModel> {
                new EducatorHelpOverlayModel { KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString(), Displayed = true },
                new EducatorHelpOverlayModel { KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString(), Displayed = true }
            };
            _mockOnboardingFlagsRepo.Setup(x => x.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockOnboardingFlagsRepo.Setup(x => x.GetByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(mockEducatorHelpOverlay);
            var expected = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync_should_call_repo()
        {
            // Arrange:
            var sut = CreateService();
            var expected = new OnboardingFlagsModel(sut._defaultOnboardingFlags);

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(1);

            // Assert:
            // Make sure the correct service method is called
            _mockOnboardingFlagsRepo.Verify(tr => tr.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(1), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync_should_return_correct_result()
        {
            // Arrange:
            var sut = CreateService();
            var mockGlobalSettings = new GlobalSettingModel { HasSeenCartTooltipForTranscriptsInSavedSchoolsMode = true, HasSeenCartTooltipForTranscriptsInSearchMode = true };
            _mockOnboardingFlagsRepo.Setup(x => x.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(mockGlobalSettings);
            _mockOnboardingFlagsRepo.Setup(x => x.GetByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(mockGlobalSettings);
            var expected = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync_should_call_repo()
        {
            // Arrange:
            var sut = CreateService();
            var expected = new OnboardingFlagsModel(sut._defaultOnboardingFlags);

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(1);

            // Assert:
            // Make sure the correct service method is called
            _mockOnboardingFlagsRepo.Verify(tr => tr.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(1), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Service")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync_should_return_correct_result()
        {
            // Arrange:
            var sut = CreateService();
            var mockEducatorHelpOverlay = new List<EducatorHelpOverlayModel> {
                new EducatorHelpOverlayModel { KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString(), Displayed = true },
                new EducatorHelpOverlayModel { KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString(), Displayed = true }
            };
            _mockOnboardingFlagsRepo.Setup(x => x.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockOnboardingFlagsRepo.Setup(x => x.GetByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(mockEducatorHelpOverlay);
            var expected = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );

            // Act:
            var result = await sut.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(1);

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));
        }

        private string ToAssertableString(IDictionary<OnboardingFlagsKeyName, bool> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        private OnboardingFlagsService CreateService()
        {
            return new OnboardingFlagsService(_mockOnboardingFlagsRepo.Object);
        }
    }
}
