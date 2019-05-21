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
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ControllerTests
{
    [TestClass]
    public class InstitutionsControllerUnitTests : ControllerTestBase
    {
        private Mock<IInstitutionRepository> _mockInstitutionRepository;
        private Mock<ITranscriptProviderService> _mockTranscriptProviderService;

        public InstitutionsControllerUnitTests()
        {
            _mockInstitutionRepository = new Mock<IInstitutionRepository>();
            _mockTranscriptProviderService = new Mock<ITranscriptProviderService>();
        }

        [TestMethod]
        [TestCategory("Institutions Controller")]
        public void GetInNetworkReceivers_should_return_ok()
        {
            // Arrange:
            var receiverList = new List<InstitutionReceiverModel> { };
            _mockTranscriptProviderService.Setup(tps => tps.GetTranscriptInNetworkReceiverList()).Returns(receiverList);
            var transcriptInNetworkInstitutionList = new List<InstitutionReceiverResponseModel> { };
            _mockTranscriptProviderService.Setup(tps => tps.GetInstitutionReceiverResponseModel(It.IsAny<List<InstitutionReceiverModel>>())).Returns(transcriptInNetworkInstitutionList);

            // Act
            var actionResult = CreateController().GetInNetworkReceivers();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<List<InstitutionReceiverResponseModel>>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("Institutions Controller")]
        public async Task GetSavedSchools_should_return_ok()
        {
            // Arrange:
            var savedSchools = new List<TranscriptInstitutionModel> {
                new TranscriptInstitutionModel
                {
                    InunId = "123",
                    Name = "University Of New York",
                    ImageName = "new-york.jog",
                    City = "New York",
                    StateProvCode = "NY",
                    StateProvName = "New York"
                }
            };
            _mockInstitutionRepository.Setup(ir => ir.SavedSchoolsByPortfolioIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(savedSchools);

            // Act
            var actionResult = await CreateController().GetSavedSchools();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<List<SavedSchool>>(actionResult);
            Assert.IsNotNull(response);
        }

        private InstitutionsController CreateController()
        {
            var sut = new InstitutionsController(
                _mockInstitutionRepository.Object,
                _mockTranscriptProviderService.Object);

            var claimCollection = new Claim[] {
                new Claim(CcClaimType.UserType.ToString(), "Student"),
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
