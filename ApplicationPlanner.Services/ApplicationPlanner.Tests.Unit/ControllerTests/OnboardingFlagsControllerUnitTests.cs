using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Web.Controllers;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ControllerTests
{
    [TestClass]
    public class OnboardingFlagsControllerUnitTests : ControllerTestBase
    {
        private Mock<IOnboardingFlagsService> _mockOnboardingFlagsService;

        public OnboardingFlagsControllerUnitTests()
        {
            _mockOnboardingFlagsService = new Mock<IOnboardingFlagsService>();
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task Get_should_return_ok_with_correct_result_for_student()
        {
            // Arrange:
            var onboardingFlags = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );
            _mockOnboardingFlagsService.Setup(ofs => ofs.GetByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(onboardingFlags);
            
            // Act
            var actionResult = await CreateController(UserType.Student).Get();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<OnboardingFlagsModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task Get_should_return_ok_with_correct_result_for_educator()
        {
            // Arrange:
            var onboardingFlags = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );
            _mockOnboardingFlagsService.Setup(ofs => ofs.GetByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(onboardingFlags);

            // Act
            var actionResult = await CreateController(UserType.Educator).Get();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<OnboardingFlagsModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task Get_should_return_bad_request()
        {
            // Arrange:
            
            // Act
            var actionResult = await CreateController(UserType.Parent).Get();

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode_should_return_ok_with_correct_result_for_student()
        {
            // Arrange:
            var onboardingFlags = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );
            _mockOnboardingFlagsService.Setup(ofs => ofs.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(onboardingFlags);

            // Act
            var actionResult = await CreateController(UserType.Student).SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<OnboardingFlagsModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode_should_return_ok_with_correct_result_for_educator()
        {
            // Arrange:
            var onboardingFlags = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );
            _mockOnboardingFlagsService.Setup(ofs => ofs.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(onboardingFlags);

            // Act
            var actionResult = await CreateController(UserType.Educator).SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<OnboardingFlagsModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode_should_return_bad_request()
        {
            // Arrange:

            // Act
            var actionResult = await CreateController(UserType.Parent).SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode();

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchMode_should_return_ok_with_correct_result_for_student()
        {
            // Arrange:
            var onboardingFlags = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );
            _mockOnboardingFlagsService.Setup(ofs => ofs.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(onboardingFlags);

            // Act
            var actionResult = await CreateController(UserType.Student).SaveHasSeenCartTooltipForTranscriptsInSearchMode();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<OnboardingFlagsModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchMode_should_return_ok_with_correct_result_for_educator()
        {
            // Arrange:
            var onboardingFlags = new OnboardingFlagsModel(
                new Dictionary<OnboardingFlagsKeyName, bool>()
                {
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, true },
                    { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, true }
                }
            );
            _mockOnboardingFlagsService.Setup(ofs => ofs.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(onboardingFlags);

            // Act
            var actionResult = await CreateController(UserType.Educator).SaveHasSeenCartTooltipForTranscriptsInSearchMode();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<OnboardingFlagsModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("OnboardingFlags Controller")]
        public async Task SaveHasSeenCartTooltipForTranscriptsInSearchMode_should_return_bad_request()
        {
            // Arrange:

            // Act
            var actionResult = await CreateController(UserType.Parent).SaveHasSeenCartTooltipForTranscriptsInSearchMode();

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        private OnboardingFlagsController CreateController(UserType userType)
        {
            var sut = new OnboardingFlagsController(_mockOnboardingFlagsService.Object);

            var claimCollectionStudent = new Claim[] {
                new Claim(CcClaimType.UserType.ToString(), "Student"),
                new Claim(CcClaimType.StudentPortfolioId.ToString(), "123")
            };
            var claimCollectionEducator = new Claim[] {
                new Claim(CcClaimType.UserType.ToString(), "Educator"),
                new Claim(CcClaimType.EducatorId.ToString(), "123")
            };
            var claimCollectionOther = new Claim[] {
                new Claim(CcClaimType.UserType.ToString(), "Other")
            };
            var claimCollection = userType == UserType.Educator 
                ? claimCollectionEducator 
                : userType == UserType.Student
                ? claimCollectionStudent
                : claimCollectionOther;
            sut.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new TestPrincipal(claimCollection)
            };

            return sut;
        }
    }
}
