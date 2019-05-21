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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ControllerTests
{
    [TestClass]
    public class RequestsControllerUnitTests : ControllerTestBase
    {
        private Mock<ITranscriptRequestRepository> _mockTranscriptRequestRepository;
        private Mock<ITranscriptRequestService> _mockTranscriptRequestService;
        private Mock<IInstitutionRepository> _mockInstitutionRepository;
        private Mock<ILinqWrapperService> _mockLinqWrapperService;
        private Mock<ITranscriptRepository> _mockTranscriptRepository;
        private Mock<ITranscriptService> _mockTranscriptService;
        private Mock<ITranscriptProviderService> _mockTranscriptProviderService;

        public RequestsControllerUnitTests()
        {
            _mockTranscriptRequestRepository = new Mock<ITranscriptRequestRepository>();
            _mockTranscriptRequestService = new Mock<ITranscriptRequestService>();
            _mockInstitutionRepository = new Mock<IInstitutionRepository>();
            _mockLinqWrapperService = new Mock<ILinqWrapperService>();
            _mockTranscriptRepository = new Mock<ITranscriptRepository>();
            _mockTranscriptService = new Mock<ITranscriptService>();
            _mockTranscriptProviderService = new Mock<ITranscriptProviderService>();
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task Post_should_return_bad_request_for_invalid_input()
        {
            // Arrange:
            var request = (RequestTranscriptRequestModel)null;

            // Act
            var actionResult = await CreateController().Post(request);

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task Post_should_return_ok_for_valid_input()
        {
            // Arrange:
            var requestId = 12;
            var request = new RequestTranscriptRequestModel
            {
                InunId = "45784",
                TranscriptRequestTypeId = TranscriptRequestType.InNetwork,
                ReceivingInstitutionName = "NCAA - National Collegiate Athletic Association",
                ReceivingInstitutionCity = "Indianapolis",
                ReceivingInstitutionStateCode = "IN",
                PortfolioId = 123456
            };
            _mockTranscriptRequestRepository.Setup(r => r.CreateTranscriptRequestAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>())).ReturnsAsync(requestId);

            // Act
            var actionResult = await CreateController().Post(request);

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<int>(actionResult);
            Assert.IsNotNull(response);
            Assert.AreEqual(requestId, response);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task Get_should_return_ok_with_list_requets()
        {
            // Arrange:
            var studentRequests = new List<TranscriptRequestResponseModel>
            {
                new TranscriptRequestResponseModel {
                    Id = 123,
                    InunId = "456",
                    ReceivingInstitutionCode = 456,
                    LatestHistory = new TranscriptRequestHistoryEvent
                    {
                         IsCreatedByStudent = true,
                         Status = TranscriptRequestStatus.Requested,
                         StatusDate = new DateTime(2019,01,10),
                         TranscriptRequestType = TranscriptRequestType.InNetwork

                    }
                }
            };
            _mockTranscriptRequestService.Setup(r => r.GetTranscriptRequestByPortfolioIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(studentRequests);

            // Act
            var actionResult = await CreateController().Get();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<List<TranscriptRequestResponseModel>>(actionResult);
            Assert.IsNotNull(response);
            CollectionAssert.AreEquivalent(studentRequests, response);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task GetTimeline_should_return_ok_with_student_timeline()
        {
            // Arrange:
            var studentTimeline = new List<TranscriptRequestTimelineResponseModel>
            {
                new TranscriptRequestTimelineResponseModel {
                    Id = 123,
                    InstitutionCard = new SavedSchool {
                        Institution = new TranscriptInstitutionModel
                        {
                            InunId = "123",
                            Name = "Institution Name",
                            ImageName = "institution.jpg",
                            City = "City",
                            StateProvCode = "SC",
                            StateProvName = "State"
                        },
                        IsSavedSchool = true
                    },
                    History = new List<TranscriptRequestHistoryEvent>
                    {
                        new TranscriptRequestHistoryEvent
                        {
                            IsCreatedByStudent = true,
                            Status = TranscriptRequestStatus.Requested,
                            StatusDate = new DateTime(),
                            TranscriptRequestType = TranscriptRequestType.InNetwork
                        }
                    }
                }
            };
            _mockTranscriptRequestService.Setup(s => s.GetTranscriptRequestsTimelineByPortfolioIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(studentTimeline);

            // Act
            var actionResult = await CreateController().GetTimeline();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<List<TranscriptRequestTimelineResponseModel>>(actionResult);
            Assert.IsNotNull(response);
            CollectionAssert.AreEquivalent(studentTimeline, response);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task GetTranscriptRequestForCurrentSchool_should_return_ok_with_result()
        {
            // Arrange:
            var currentSchool = new InstitutionModel
            {
                Id = 1234,
                InstitutionName = "Institution Name",
                InstitutionType = InstitutionTypeEnum.School
            };
            _mockInstitutionRepository.Setup(ir => ir.GetDefaultInstitutionByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(currentSchool);
            var requestsBySchool = new List<TranscriptRequestStudentViewModel> {
                new TranscriptRequestStudentViewModel
                {
                    Id = 1234,
                    InunId = "1234",
                    ReceivingInstitutionCode = 1234,
                    UserAccountId = 1234,
                    AvatarFileName = "avatar.jpg",
                    SchoolCountryType = CountryType.US,
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GenderId = 1,
                    GenderKey = "GENDER_FEMALE",
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptRequestId = 12,
                    ReceivingInstitutionName = "Institution Name",
                    ReceivingInstitutionCity = "City",
                    ReceivingInstitutionStateCode = "SC",
                    InstitutionCity = "City",
                    TranscriptRequestType = TranscriptRequestType.InNetwork,
                    TranscriptRequestTypeKey = "INNETWORK",
                    RequestedDate = new DateTime(),
                    ImportedDate = new DateTime()
                }
            };
            _mockTranscriptRequestRepository.Setup(rr => rr.GetTranscriptRequestBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(requestsBySchool);
            var requestsBySchoolLinqued = new ItemsCountModel<TranscriptRequestStudentViewModel>
            {
                Items = requestsBySchool,
                Count = 1
            };
            _mockLinqWrapperService.Setup(ls => ls.GetLinqedList(
                It.IsAny<IEnumerable<TranscriptRequestStudentViewModel>>(),
                It.IsAny<Func<TranscriptRequestStudentViewModel, bool>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<SortOrder>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).Returns(requestsBySchoolLinqued);
            var responseModel = new List<TranscriptRequestStudentResponseModel>
            {
                new TranscriptRequestStudentResponseModel
                {
                    Id = 1234,
                    AvatarUrl = "http://www.avatar.com/avatar.jpg",
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptRequestId = 12,
                    InunId = "1234",
                    ReceivingInstitutionCode = 1234,
                    ReceivingInstitutionName = "Institution Name",
                    ReceivingInstitutionCity = "City",
                    ReceivingInstitutionStateCode = "SC",
                    TranscriptRequestType = TranscriptRequestType.InNetwork,
                    TranscriptRequestTypeKey = "INNETWORK",
                    RequestedDate = new DateTime(),
                    ImportedDate = new DateTime()
                }
            };
            _mockTranscriptRequestService.Setup(rs => rs.GetTranscriptRequestStudentResponseModelAsync(It.IsAny<List<TranscriptRequestStudentViewModel>>(), It.IsAny<int>())).ReturnsAsync(responseModel);

            // Act
            var actionResult = await CreateController().GetTranscriptRequestForCurrentSchool();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<ItemsCountModel<TranscriptRequestStudentResponseModel>>(actionResult);
            Assert.IsNotNull(response);
            var expected = new ItemsCountModel<TranscriptRequestStudentResponseModel>
            {
                Items = responseModel,
                Count = 1
            };
            CollectionAssert.AreEquivalent(expected.Items.ToList(), response.Items.ToList());
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task Delete_should_return_ok_with_id_of_deleted_request()
        {
            // Arrange:
            var requestId = 12;
            _mockTranscriptRequestRepository.Setup(r => r.DeleteByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(requestId);

            // Act
            var actionResult = await CreateController().Delete(requestId);

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<int>(actionResult);
            Assert.IsNotNull(response);
            Assert.AreEqual(requestId, response);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task DeleteUndo_should_return_ok_with_id_of_deleted_request()
        {
            // Arrange:
            var requestId = 12;
            _mockTranscriptRequestRepository.Setup(r => r.DeleteUndoByIdAsync(It.IsAny<int>())).ReturnsAsync(requestId);

            // Act
            var actionResult = await CreateController().DeleteUndo(requestId);

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<int>(actionResult);
            Assert.IsNotNull(response);
            Assert.AreEqual(requestId, response);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task GetTranscriptRequestProgressForCurrentSchool_should_return_ok_with_result()
        {
            // Arrange:
            var currentSchool = new InstitutionModel
            {
                Id = 1234,
                InstitutionName = "Institution Name",
                InstitutionType = InstitutionTypeEnum.School
            };
            _mockInstitutionRepository.Setup(ir => ir.GetDefaultInstitutionByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(currentSchool);
            var requestsProgressBySchool = new List<TranscriptRequestProgressViewModel> {
                new TranscriptRequestProgressViewModel
                {
                    Id = 1234,
                    InunId = "1234",
                    ReceivingInstitutionCode = 1234,
                    UserAccountId = 1234,
                    AvatarFileName = "avatar.jpg",
                    SchoolCountryType = CountryType.US,
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptRequestId = 12,
                    ReceivingInstitutionName = "Institution Name",
                    ReceivingInstitutionCity = "City",
                    ReceivingInstitutionStateCode = "SC",
                    RequestedDate = new DateTime(),
                    TranscriptStatus = TranscriptRequestStatus.Submitted,
                    TranscriptStatusKey = "SUBMITTED",
                    TranscriptStatusDate = new DateTime()
                }
            };
            _mockTranscriptRequestRepository.Setup(rr => rr.GetTranscriptRequestProgressBySchoolIdAsync(It.IsAny<int>())).ReturnsAsync(requestsProgressBySchool);
            var requestsProgressBySchoolLinqed = new ItemsCountModel<TranscriptRequestProgressViewModel>
            {
                Items = requestsProgressBySchool,
                Count = 1
            };
            _mockLinqWrapperService.Setup(ls => ls.GetLinqedList(
                It.IsAny<IEnumerable<TranscriptRequestProgressViewModel>>(),
                It.IsAny<Func<TranscriptRequestProgressViewModel, bool>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<SortOrder>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).Returns(requestsProgressBySchoolLinqed);
            var responseModel = new List<TranscriptRequestProgressResponseModel>
            {
                new TranscriptRequestProgressResponseModel
                {
                    Id = 1234,
                    AvatarUrl = "http://www.avatar.com/avatar.jpg",
                    StudentName = "FirstName, LastName",
                    DateOfBirth = new DateTime(2000, 5, 23),
                    GradeId = 11,
                    GradeKey = "GRADE_11",
                    StudentId = "1234",
                    TranscriptRequestId = 12,
                    InunId = "1234",
                    ReceivingInstitutionCode = 1234,
                    ReceivingInstitutionName = "Institution Name",
                    ReceivingInstitutionCity = "City",
                    ReceivingInstitutionStateCode = "SC",
                    RequestedDate = new DateTime(),
                    TranscriptStatus = TranscriptRequestStatus.Submitted,
                    TranscriptStatusKey = "SUBMITTED",
                    TranscriptStatusDate = new DateTime()
                }
            };
            _mockTranscriptRequestService.Setup(rs => rs.GetTranscriptRequestProgressResponseModelAsync(It.IsAny<List<TranscriptRequestProgressViewModel>>(), It.IsAny<int>())).ReturnsAsync(responseModel);

            // Act
            var actionResult = await CreateController().GetTranscriptRequestProgressForCurrentSchool();

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<ItemsCountModel<TranscriptRequestProgressResponseModel>>(actionResult);
            Assert.IsNotNull(response);
            var expected = new ItemsCountModel<TranscriptRequestProgressResponseModel>
            {
                Items = responseModel,
                Count = 1
            };
            CollectionAssert.AreEquivalent(expected.Items.ToList(), response.Items.ToList());
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task GetTranscriptRequestProgressForStudent_should_return_ok_with_result()
        {
            // Arrange:
            var studentTranscript = new StudentTranscriptViewModel
            {
                Id = 15829045,
                UserAccountId = 5678,
                SchoolCountryType = CountryType.US,
                AvatarFileName = "avatar.jpg",
                StudentName = "Mike, Cohn",
                DateOfBirth = DateTime.Now.AddYears(-17),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = "STUD-1234",
                TranscriptId = null,
                ReceivedDateUtc = null
            };
            _mockTranscriptRepository.Setup(t => t.GetStudentTranscriptByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(studentTranscript);
            var studentTranscriptResponse = new StudentTranscriptResponseModel
            {
                Id = 15829045,
                AvatarUrl = "http://avatarrepos/avatar.jpg",
                StudentName = "Mike, Cohn",
                DateOfBirth = DateTime.Now.AddYears(-17),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = "STUD-1234",
                TranscriptId = null,
                ReceivedDateUtc = null
            };
            _mockTranscriptService.Setup(ts => ts.GetStudentTranscriptResponseModelAsync(It.IsAny<StudentTranscriptViewModel>(), It.IsAny<int>())).ReturnsAsync(studentTranscriptResponse);
            var progress = new List<TranscriptRequestTimelineResponseModel>
            {
                new TranscriptRequestTimelineResponseModel {
                    Id = 123,
                    InstitutionCard = new SavedSchool {
                        Institution = new TranscriptInstitutionModel
                        {
                            InunId = "123",
                            Name = "Institution Name",
                            ImageName = "institution.jpg",
                            City = "City",
                            StateProvCode = "SC",
                            StateProvName = "State"
                        },
                        IsSavedSchool = true
                    },
                    History = new List<TranscriptRequestHistoryEvent>
                    {
                        new TranscriptRequestHistoryEvent
                        {
                            IsCreatedByStudent = true,
                            Status = TranscriptRequestStatus.Requested,
                            StatusDate = new DateTime(),
                            TranscriptRequestType = TranscriptRequestType.InNetwork
                        }
                    }
                }
            };
            _mockTranscriptRequestService.Setup(rs => rs.GetTranscriptRequestsTimelineByPortfolioIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(progress);

            // Act
            var actionResult = await CreateController().GetTranscriptRequestProgressForStudent(1);

            //Assert
            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            var response = Result<StudentRequestProgressResponseModel>(actionResult);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task SendTranscriptRequest_should_return_bad_request_for_invalid_input()
        {
            // Arrange:
            _mockTranscriptRequestService.Setup(trs => trs.IsSendTranscriptRequestInputValidAsync(It.IsAny<SendTranscriptViewModel>(), It.IsAny<TranscriptSubmitRequestModel>())).ReturnsAsync(false);

            // Act
            var actionResult = await CreateController().SendTranscriptRequest(1, null);

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task SendTranscriptRequest_should_return_ok_for_mail()
        {
            // Arrange:
            var sentTime = DateTime.UtcNow;
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestTypeId = 1 };
            _mockTranscriptRequestRepository.Setup(trr => trr.GetSendTranscriptByTranscriptRequestIdAsync(It.IsAny<int>())).ReturnsAsync(sendTranscriptInfos);
            var submitData = new TranscriptSubmitRequestModel { TranscriptRequestType = TranscriptRequestType.Mail, DateSentByMail = sentTime };
            _mockTranscriptRequestService.Setup(trs => trs.IsSendTranscriptRequestInputValidAsync(It.IsAny<SendTranscriptViewModel>(), It.IsAny<TranscriptSubmitRequestModel>())).ReturnsAsync(true);
            _mockTranscriptRequestService.Setup(trs => trs.GetDateSentByMailUtcAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(sentTime);

            // Act
            var actionResult = await CreateController().SendTranscriptRequest(1, submitData);

            //Assert
            // Make sure the correct service methods are called
            _mockTranscriptRequestService.Verify(trs => trs.SubmitOutOfNetworkAsync(1, TranscriptRequestType.Mail, null, sentTime), Times.Once);
            _mockTranscriptRequestRepository.Verify(trr => trr.AppendHistoryAsync(1, TranscriptRequestStatus.Submitted, 456), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task SendTranscriptRequest_should_return_ok_for_email()
        {
            // Arrange:
            var email = "receiver@mail.com";
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestId = 1, TranscriptRequestTypeId = 1, StudentId = "123", TranscriptId = 2, SchoolId = 456, ReceivingInstitutionCode = "code", ReceivingInstitutionName = "name" };
            _mockTranscriptRequestRepository.Setup(trr => trr.GetSendTranscriptByTranscriptRequestIdAsync(It.IsAny<int>())).ReturnsAsync(sendTranscriptInfos);
            var submitData = new TranscriptSubmitRequestModel { TranscriptRequestType = TranscriptRequestType.Email, ReceivingInstitutionEmail = email };
            _mockTranscriptRequestService.Setup(trs => trs.IsSendTranscriptRequestInputValidAsync(It.IsAny<SendTranscriptViewModel>(), It.IsAny<TranscriptSubmitRequestModel>())).ReturnsAsync(true);

            // Act
            var actionResult = await CreateController().SendTranscriptRequest(1, submitData);

            //Assert
            // Make sure the correct service methods are called
            _mockTranscriptProviderService.Verify(tps => tps.SendTranscriptRequestAsync(1, "123", 2, 456, "code", "name", 456, email), Times.Once);
            _mockTranscriptRequestService.Verify(trs => trs.SubmitOutOfNetworkAsync(1, TranscriptRequestType.Email, email, null), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task SendTranscriptRequest_should_return_ok_for_innetwork()
        {
            // Arrange:
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestId = 1, TranscriptRequestTypeId = 1, StudentId = "123", TranscriptId = 2, SchoolId = 456, ReceivingInstitutionCode = "code", ReceivingInstitutionName = "name" };
            _mockTranscriptRequestRepository.Setup(trr => trr.GetSendTranscriptByTranscriptRequestIdAsync(It.IsAny<int>())).ReturnsAsync(sendTranscriptInfos);
            var submitData = new TranscriptSubmitRequestModel { TranscriptRequestType = TranscriptRequestType.InNetwork };
            _mockTranscriptRequestService.Setup(trs => trs.IsSendTranscriptRequestInputValidAsync(It.IsAny<SendTranscriptViewModel>(), It.IsAny<TranscriptSubmitRequestModel>())).ReturnsAsync(true);

            // Act
            var actionResult = await CreateController().SendTranscriptRequest(1, submitData);

            //Assert
            // Make sure the correct service methods are called
            _mockTranscriptProviderService.Verify(tps => tps.SendTranscriptRequestAsync(1, "123", 2, 456, "code", "name", 456, ""), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task BulkSendTranscriptRequest_should_return_bad_request_for_invalid_input()
        {
            // Arrange:
            var currentSchool = new InstitutionModel
            {
                Id = 1234,
                InstitutionName = "Institution Name",
                InstitutionType = InstitutionTypeEnum.School
            };
            _mockInstitutionRepository.Setup(ir => ir.GetDefaultInstitutionByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(currentSchool);
            _mockTranscriptRequestRepository.Setup(trr => trr.GetSendTranscriptByTranscriptRequestIdListAsync(It.IsAny<List<int>>())).ReturnsAsync((List<SendTranscriptViewModel>)null);

            // Act
            var actionResult = await CreateController().BulkSendTranscriptRequest(new List<int> { 1, 2 });

            //Assert
            var contentResult = new BadRequestObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        [TestCategory("Requests Controller")]
        public async Task BulkSendTranscriptRequest_should_return_ok()
        {
            // Arrange:
            var currentSchool = new InstitutionModel
            {
                Id = 1234,
                InstitutionName = "Institution Name",
                InstitutionType = InstitutionTypeEnum.School
            };
            _mockInstitutionRepository.Setup(ir => ir.GetDefaultInstitutionByEducatorIdAsync(It.IsAny<int>())).ReturnsAsync(currentSchool);
            var sendTranscriptList = new List<SendTranscriptViewModel>
            {
                new SendTranscriptViewModel { TranscriptRequestId = 1 },
                new SendTranscriptViewModel { TranscriptRequestId = 2 }
            };
            _mockTranscriptRequestRepository.Setup(trr => trr.GetSendTranscriptByTranscriptRequestIdListAsync(It.IsAny<List<int>>())).ReturnsAsync(sendTranscriptList);

            // Act
            var actionResult = await CreateController().BulkSendTranscriptRequest(new List<int> { 1, 2 });

            //Assert
            // Make sure the correct service methods are called
            _mockTranscriptProviderService.Verify(tps => tps.BulkSendTranscriptRequestAsync(sendTranscriptList, currentSchool.Id, 456), Times.Once);

            var contentResult = new OkObjectResult(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        private RequestsController CreateController()
        {
            var sut = new RequestsController(
                _mockTranscriptRequestRepository.Object,
                _mockTranscriptRequestService.Object,
                _mockInstitutionRepository.Object,
                _mockLinqWrapperService.Object,
                _mockTranscriptRepository.Object,
                _mockTranscriptService.Object,
                _mockTranscriptProviderService.Object);

            var claimCollection = new Claim[] {
                new Claim(CcClaimType.UserType.ToString(), "Student"),
                new Claim(CcClaimType.StudentPortfolioId.ToString(), "123"),
                new Claim(CcClaimType.UserAccountId.ToString(), "456"),
                new Claim(CcClaimType.EducatorId.ToString(), "789")
            };
            sut.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new TestPrincipal(claimCollection)
            };

            return sut;
        }
    }
}
