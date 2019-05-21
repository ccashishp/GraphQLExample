using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Controllers;
using ApplicationPlanner.Transcripts.Web.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ControllerTests
{
    [TestClass]
    public class AuthenticatedStudentControllerUnitTests : ControllerTestBase
    {
        private Mock<IStudentRepository> _mockStudentRepository;
        private Mock<IStudentService> _mockStudentService;

        public AuthenticatedStudentControllerUnitTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockStudentService = new Mock<IStudentService>();
        }

        [TestMethod]
        [TestCategory("AuthenticatedStudent Controller")]
        public async Task GetAuthenticatedStudent_should_return_not_found_for_invalid_token()
        {
            // Arrange:
            var studentGeneralInfo = (StudentGeneralInfoModel)null;
            _mockStudentRepository.Setup(sr => sr.StudentGeneralInfoGetByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(studentGeneralInfo);

            // Act
            var actionResult = await CreateController().GetAuthenticatedStudent();

            //Assert
            var contentResult = new NotFoundObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("AuthenticatedStudent Controller")]
        public async Task GetAuthenticatedStudent_should_return_ok()
        {
            // Arrange:
            var studentGeneralInfo = new StudentGeneralInfoModel { };
            _mockStudentRepository.Setup(sr => sr.StudentGeneralInfoGetByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(studentGeneralInfo);
            var authenticatedStudent = new AuthenticatedStudentModel { };
            _mockStudentService.Setup(ss => ss.AuthenticatedStudentGetByStudentGeneralInfo(It.IsAny<StudentGeneralInfoModel>())).Returns(authenticatedStudent);

            // Act
            var actionResult = await CreateController().GetAuthenticatedStudent();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<AuthenticatedStudentModel>(actionResult);
            Assert.IsNotNull(response);
        }

        private AuthenticatedStudentController CreateController()
        {
            var sut = new AuthenticatedStudentController(
                _mockStudentRepository.Object,
                _mockStudentService.Object);

            var claimCollection = new Claim[] {
                new Claim(CcClaimType.StudentPortfolioId.ToString(), "123")
            };
            sut.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new TestPrincipal(claimCollection)
            };

            return sut;
        }
    }
}
