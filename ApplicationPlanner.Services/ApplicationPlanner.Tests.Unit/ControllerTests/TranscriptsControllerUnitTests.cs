using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Controllers;
using ApplicationPlanner.Transcripts.Web.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ControllerTests
{
    [TestClass]
    public class TranscriptsControllerUnitTests : ControllerTestBase
    {
        private Mock<IInstitutionRepository> _mockInstitutionRepository;
        private Mock<ITranscriptProviderService> _mockTranscriptProviderService;
        private Mock<ITranscriptRepository> _mockTranscriptRepository;
        private Mock<ITranscriptService> _mockTranscriptService;
        private Mock<ILinqWrapperService> _mockLinqWrapperService;

        public TranscriptsControllerUnitTests()
        {
            _mockInstitutionRepository = new Mock<IInstitutionRepository>();
            _mockTranscriptProviderService = new Mock<ITranscriptProviderService>();
            _mockTranscriptRepository = new Mock<ITranscriptRepository>();
            _mockTranscriptService = new Mock<ITranscriptService>();
            _mockLinqWrapperService = new Mock<ILinqWrapperService>();
            var _mockCurrentSchool = new InstitutionModel
            {
                Id = 1234,
                InstitutionName = "Test Institution",
                InstitutionType = InstitutionTypeEnum.School
            };
            _mockInstitutionRepository.Setup(ir => ir.GetDefaultInstitutionByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(_mockCurrentSchool);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task Import_should_return_bad_request_for_invalid_file_format()
        {
            // Arrange:
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.invalidextension";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            var sut = CreateController();
            var file = fileMock.Object;

            // Act
            var actionResult = await sut.Import(file);

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task Import_should_return_ok()
        {
            // Arrange:
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            var sut = CreateController();
            var file = fileMock.Object;

            // Act
            var actionResult = await sut.Import(file);

            //Assert
            // Make sure the correct service method is called
            _mockTranscriptProviderService.Verify(tps => tps.ImportTranscriptAsync(1234, 123, "pdf", file.OpenReadStream()), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task GetTranscriptImportedForCurrentSchool_should_return_ok_with_result()
        {
            // Arrange:
            var importedTranscriptsBySchool = new List<TranscriptImportedViewModel> {
                new TranscriptImportedViewModel
                {
                    Id = 1234,
                    UserAccountId = 1234,
                    AvatarFileName = "avatar.jpg",
                    SchoolCountryType = CountryType.US,
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptId = 1000000,
                    ImportedDate = new DateTime()
                }
            };
            _mockTranscriptRepository.Setup(tr => tr.GetTranscriptImportedBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(importedTranscriptsBySchool);
            var importedTranscriptsBySchoolLinqed = new ItemsCountModel<TranscriptImportedViewModel>
            {
                Items = importedTranscriptsBySchool,
                Count = 1
            };
            _mockLinqWrapperService.Setup(ls => ls.GetLinqedList(
                It.IsAny<IEnumerable<TranscriptImportedViewModel>>(),
                It.IsAny<Func<TranscriptImportedViewModel, bool>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<SortOrder>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).Returns(importedTranscriptsBySchoolLinqed);
            var responseModel = new List<TranscriptImportedResponseModel>
            {
                new TranscriptImportedResponseModel
                {
                    Id = 1234,
                    AvatarUrl = "http://www.avatar.com/avatar.jpg",
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptId = 1000000,
                    ImportedDate = new DateTime()
                }
            };
            _mockTranscriptService.Setup(ts => ts.GetTranscriptImportedResponseModelAsync(It.IsAny<List<TranscriptImportedViewModel>>(), It.IsAny<int>())).ReturnsAsync(responseModel);

            // Act
            var actionResult = await CreateController().GetTranscriptImportedForCurrentSchool();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<ItemsCountModel<TranscriptImportedResponseModel>>(actionResult);
            Assert.IsNotNull(response);
            var expected = new ItemsCountModel<TranscriptImportedResponseModel>
            {
                Items = responseModel,
                Count = 1
            };
            CollectionAssert.AreEquivalent(expected.Items.ToList(), response.Items.ToList());
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task GetTranscriptUnmatchedForCurrentSchool_should_return_ok_with_result()
        {
            // Arrange:
            var unmatchedTranscriptsBySchool = new List<TranscriptBaseModel> {
                new TranscriptBaseModel
                {
                    TranscriptId = 1000000,
                    StudentName = "FirstName, LastName",
                    StudentNumber = "1234",
                    DateOfBirth = "30/05/2000",
                    ReceivedDateUtc = new DateTime()
                }
            };
            _mockTranscriptRepository.Setup(tr => tr.GetTranscriptUnmatchedBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(unmatchedTranscriptsBySchool);
            var unmatchedTranscriptsBySchoolLinqed = new ItemsCountModel<TranscriptBaseModel>
            {
                Items = unmatchedTranscriptsBySchool,
                Count = 1
            };
            _mockLinqWrapperService.Setup(ls => ls.GetLinqedList(
                It.IsAny<IEnumerable<TranscriptBaseModel>>(),
                It.IsAny<Func<TranscriptBaseModel, bool>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<SortOrder>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).Returns(unmatchedTranscriptsBySchoolLinqed);
            var responseModel = unmatchedTranscriptsBySchool;
            _mockTranscriptService.Setup(ts => ts.GetTranscriptUnmatchedResponseModelAsync(It.IsAny<List<TranscriptBaseModel>>(), It.IsAny<int>())).ReturnsAsync(responseModel);

            // Act
            var actionResult = await CreateController().GetTranscriptUnmatchedForCurrentSchool();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<ItemsCountModel<TranscriptBaseModel>>(actionResult);
            Assert.IsNotNull(response);
            var expected = new ItemsCountModel<TranscriptBaseModel>
            {
                Items = responseModel,
                Count = 1
            };
            CollectionAssert.AreEquivalent(expected.Items.ToList(), response.Items.ToList());
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task Delete_should_return_ok()
        {
            // Arrange:
            var transcriptId = 1000000;
            var sut = CreateController();

            // Act
            var actionResult = await sut.Delete(transcriptId);

            //Assert
            // Make sure the correct service method is called
            _mockTranscriptRepository.Verify(tr => tr.DeleteByIdAsync(transcriptId), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task DeleteUndo_should_return_ok()
        {
            // Arrange:
            var transcriptId = 1000000;
            var sut = CreateController();

            // Act
            var actionResult = await sut.DeleteUndo(transcriptId);

            //Assert
            // Make sure the correct service method is called
            _mockTranscriptRepository.Verify(tr => tr.DeleteUndoByIdAsync(transcriptId), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task GetStudentTranscriptForCurrentSchool_should_return_ok_with_result()
        {
            // Arrange:
            var studentTranscriptsBySchool = new List<StudentTranscriptViewModel> {
                new StudentTranscriptViewModel
                {
                    Id = 1234,
                    UserAccountId = 1234,
                    AvatarFileName = "avatar.jpg",
                    SchoolCountryType = CountryType.US,
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptId = 1000000,
                    ReceivedDateUtc = new DateTime()
                }
            };
            _mockTranscriptRepository.Setup(tr => tr.GetStudentTranscriptBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(studentTranscriptsBySchool);
            var studentTranscriptsBySchoolLinqed = new ItemsCountModel<StudentTranscriptViewModel>
            {
                Items = studentTranscriptsBySchool,
                Count = 1
            };
            _mockLinqWrapperService.Setup(ls => ls.GetLinqedList(
                It.IsAny<IEnumerable<StudentTranscriptViewModel>>(),
                It.IsAny<Func<StudentTranscriptViewModel, bool>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<SortOrder>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).Returns(studentTranscriptsBySchoolLinqed);
            var responseModel = new List<StudentTranscriptResponseModel>
            {
                new StudentTranscriptResponseModel
                {
                    Id = 1234,
                    AvatarUrl = "http://www.avatar.com/avatar.jpg",
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptId = 1000000,
                    ReceivedDateUtc = new DateTime()
                }
            };
            _mockTranscriptService.Setup(ts => ts.GetStudentTranscriptResponseModelAsync(It.IsAny<List<StudentTranscriptViewModel>>())).ReturnsAsync(responseModel);

            // Act
            var actionResult = await CreateController().GetStudentTranscriptForCurrentSchool();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<ItemsCountModel<StudentTranscriptResponseModel>>(actionResult);
            Assert.IsNotNull(response);
            var expected = new ItemsCountModel<StudentTranscriptResponseModel>
            {
                Items = responseModel,
                Count = 1
            };
            CollectionAssert.AreEquivalent(expected.Items.ToList(), response.Items.ToList());
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task MatchTranscriptToStudent_should_return_ok()
        {
            // Arrange:
            var transcriptId = 1000000;
            var portfolioId = 1234;
            var sut = CreateController();

            // Act
            var actionResult = await sut.MatchTranscriptToStudent(transcriptId, portfolioId);

            //Assert
            // Make sure the correct service method is called
            _mockTranscriptRepository.Verify(tr => tr.MatchTranscriptToStudentAsync(transcriptId, portfolioId, 123), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Transcripts Controller")]
        public async Task Get_should_return_ok_with_correct_result()
        {
            // Arrange:
            var transcriptId = 1000000;
            var _mockTranscript = new TranscriptModel {
                TranscriptId = transcriptId
            };
            _mockTranscriptRepository.Setup(tr => tr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_mockTranscript);
            _mockTranscriptService.Setup(ts => ts.GetTranscriptResponseModelAsync(It.IsAny<TranscriptModel>(), It.IsAny<int>())).ReturnsAsync(_mockTranscript);
            var sut = CreateController();

            // Act
            var actionResult = await sut.Get(transcriptId);

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<TranscriptModel>(actionResult);
            Assert.IsNotNull(response);
            Assert.AreEqual(transcriptId, response.TranscriptId);
        }

        // All the actions in this controller are exclusively used by Educators
        private TranscriptsController CreateController()
        {
            var sut = new TranscriptsController(
                _mockInstitutionRepository.Object,
                _mockTranscriptProviderService.Object,
                _mockTranscriptRepository.Object,
                _mockTranscriptService.Object,
                _mockLinqWrapperService.Object);

            var claimCollection = new Claim[] {
                new Claim(CcClaimType.UserType.ToString(), "Educator"),
                new Claim(CcClaimType.EducatorId.ToString(), "123")
            };
            sut.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new TestPrincipal(claimCollection)
            };

            return sut;
        }
    }
}
