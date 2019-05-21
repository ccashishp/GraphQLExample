using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class TranscriptRequestServiceUnitTests
    {
        private Mock<ITranscriptRequestRepository> _mockTranscriptRequestRepo;
        private Mock<IInstitutionRepository> _mockInstitutionRepo;
        private Mock<IAvatarService> _mockAvatarService;
        private readonly string _avatarUrl;
        private Mock<ITimeZoneRepository> _mockTimeZoneRepository;

        public TranscriptRequestServiceUnitTests()
        {
            _avatarUrl = "https://test-storage.com/avatar.jpg"; ;
        }

        [TestInitialize]
        public void Test_Init()
        {
            _mockTranscriptRequestRepo = new Mock<ITranscriptRequestRepository>();
            _mockInstitutionRepo = new Mock<IInstitutionRepository>();
            _mockAvatarService = new Mock<IAvatarService>();
            _mockTimeZoneRepository = new Mock<ITimeZoneRepository>();
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task GetTranscriptRequestsTimelineByPortfolioIdAsync_should_set_all_properties()
        {
            // Arrange:
            var userAccountId = 1234;
            var inudId_1 = "I1";
            var mockDto = new TranscriptRequestTimelineDtoV2
            {
                TranscriptRequestInstitutionList = new List<TranscriptRequestInstitutionModel>
                {
                    new TranscriptRequestInstitutionModel { TranscriptRequestId = 1, InunId = inudId_1, Name = "Institution 1", City = "San Francisco", StateProvCode = "CA"  },
                    new TranscriptRequestInstitutionModel { TranscriptRequestId = 2, InunId = "I2", Name = "Institution 2", City = "Queensbury", StateProvCode = "NY"  }
                },
                TranscriptRequestHistoryList = new List<TranscriptRequestHistoryModelV2>
                {
                    new TranscriptRequestHistoryModelV2 { TranscriptRequestId = 1, InunId = inudId_1, TranscriptStatusId = 1, ModifiedById = userAccountId, StatusDateUTC = DateTime.UtcNow.AddDays(-5), TranscriptRequestTypeId = (int)TranscriptRequestType.Mail  },
                    new TranscriptRequestHistoryModelV2 { TranscriptRequestId = 2, InunId = "I2", TranscriptStatusId = 1, ModifiedById = 5678, StatusDateUTC = DateTime.UtcNow.AddDays(-3), TranscriptRequestTypeId = (int)TranscriptRequestType.Email },
                    new TranscriptRequestHistoryModelV2 { TranscriptRequestId = 1, InunId = inudId_1, TranscriptStatusId = 2, ModifiedById = 5678, StatusDateUTC = DateTime.UtcNow.AddDays(-1), TranscriptRequestTypeId = (int)TranscriptRequestType.Mail }
                }
            };
            var mockSavedSchools = new List<TranscriptInstitutionModel>
            {
                new TranscriptInstitutionModel { InunId = inudId_1}
            };
            var timezoneDetails = new TimeZoneDetailModel { SQLKey = "Eastern Standard Time" };
            _mockTranscriptRequestRepo.Setup(x => x.GetTranscriptRequestProgressByPortfolioIdAsyncV2(It.IsAny<int>()))
                .ReturnsAsync(mockDto);
            _mockInstitutionRepo.Setup(x => x.SavedSchoolsByPortfolioIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(mockSavedSchools);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneIdByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneDetailByIdAsync(It.IsAny<int>())).ReturnsAsync(timezoneDetails);

            // Act:
            var result = await CreateService().GetTranscriptRequestsTimelineByPortfolioIdAsync(1, userAccountId, 2);

            // Assert:
            Assert.IsNotNull(result);
            // All transcript requests in the dto should also be in the result
            Assert.AreEqual(result.Count(), mockDto.TranscriptRequestInstitutionList.Count());
            // History for each transcript request in the dto should be in the result
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).History.Count(),
                mockDto.TranscriptRequestHistoryList.Where(x => x.TranscriptRequestId == 1).Count());
            
            // Set correctly the IsCreatedByStudent
            Assert.IsTrue(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).History.ToList()[0].IsCreatedByStudent);
            Assert.IsFalse(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).History.ToList()[1].IsCreatedByStudent);

            // Set correcty the Status
            Assert.AreEqual((int)result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).History.ToList()[0].Status,
                mockDto.TranscriptRequestHistoryList.Where(x => x.TranscriptRequestId == 1).ToList()[0].TranscriptStatusId);

            // Set correctly the StatusDate
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).History.ToList()[0].StatusDate,
                DateTime.SpecifyKind(mockDto.TranscriptRequestHistoryList.Where(x => x.TranscriptRequestId == 1).ToList()[0].StatusDateUTC ?? default(DateTime), DateTimeKind.Utc).ToLocalTime());

            // Set correcty the TranscriptRequestType
            Assert.AreEqual((int)result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).History.ToList()[0].TranscriptRequestType,
                mockDto.TranscriptRequestHistoryList.Where(x => x.TranscriptRequestId == 1).ToList()[0].TranscriptRequestTypeId);

            // Set correctly the Institution
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.Institution.City,
                mockDto.TranscriptRequestInstitutionList.First(x => x.InunId == inudId_1).City);
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.Institution.ImageName,
               mockDto.TranscriptRequestInstitutionList.First(x => x.InunId == inudId_1).ImageName);
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.Institution.InunId,
               mockDto.TranscriptRequestInstitutionList.First(x => x.InunId == inudId_1).InunId);
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.Institution.Name,
               mockDto.TranscriptRequestInstitutionList.First(x => x.InunId == inudId_1).Name);
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.Institution.StateProvCode,
               mockDto.TranscriptRequestInstitutionList.First(x => x.InunId == inudId_1).StateProvCode);
            Assert.AreEqual(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.Institution.StateProvName,
               mockDto.TranscriptRequestInstitutionList.First(x => x.InunId == inudId_1).StateProvName);

            // Set correctly the IsSavedSchool
            Assert.IsTrue(result.First(x => x.InstitutionCard.Institution.InunId == inudId_1).InstitutionCard.IsSavedSchool);
            Assert.IsFalse(result.First(x => x.InstitutionCard.Institution.InunId == "I2").InstitutionCard.IsSavedSchool);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task GetTranscriptRequestStudentResponseModelAsync_should_set_all_properties()
        {
            // Arrange
            var input = new TranscriptRequestStudentViewModel
            {
                Id = 15829045,
                UserAccountId = 5678,
                SchoolCountryType = CountryType.US,
                AvatarFileName = "",
                StudentName = "Todd, Motto",
                DateOfBirth = DateTime.Now.AddYears(-17),
                GenderId = 1,
                GenderKey = "GENDER_MALE",
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = "STUD-1234",
                TranscriptRequestId = 1,
                InunId = "7358",
                ReceivingInstitutionCode = 1234,
                ReceivingInstitutionName = "Massachusetts Institute of Technology",
                ReceivingInstitutionCity = "Cambridge",
                ReceivingInstitutionStateCode = "MA",
                TranscriptRequestType = TranscriptRequestType.InNetwork,
                TranscriptRequestTypeKey = "TRANSCRIPT_REQUEST_TYPE_IN_NETWORK",
                RequestedDate = DateTime.UtcNow.AddDays(-2),
                ImportedDate = null
            };
            var timezoneDetails = new TimeZoneDetailModel { SQLKey = "Eastern Standard Time" };
            _mockAvatarService.Setup(x => x.GetStudentAvatarDefaultUrl()).Returns(_avatarUrl);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneIdByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneDetailByIdAsync(It.IsAny<int>())).ReturnsAsync(timezoneDetails);

            // Act
            var result = await CreateService().GetTranscriptRequestStudentResponseModelAsync(new List<TranscriptRequestStudentViewModel> { input }, 1234);

            // Assert
            Assert.AreEqual(input.Id, result.First().Id);
            Assert.AreEqual(_avatarUrl, result.First().AvatarUrl);
            Assert.AreEqual(input.StudentName, result.First().StudentName);
            Assert.AreEqual(input.DateOfBirth, result.First().DateOfBirth);
            Assert.AreEqual(input.GradeId, result.First().GradeId);
            Assert.AreEqual(input.GradeKey, result.First().GradeKey);
            Assert.AreEqual(input.StudentId, result.First().StudentId);
            Assert.AreEqual(input.TranscriptRequestId, result.First().TranscriptRequestId);
            Assert.AreEqual(input.InunId, result.First().InunId);
            Assert.AreEqual(input.ReceivingInstitutionCode, result.First().ReceivingInstitutionCode);
            Assert.AreEqual(input.ReceivingInstitutionName, result.First().ReceivingInstitutionName);
            Assert.AreEqual(input.ReceivingInstitutionCity, result.First().ReceivingInstitutionCity);
            Assert.AreEqual(input.ReceivingInstitutionStateCode, result.First().ReceivingInstitutionStateCode);
            Assert.AreEqual(input.TranscriptRequestType, result.First().TranscriptRequestType);
            Assert.AreEqual(input.TranscriptRequestTypeKey, result.First().TranscriptRequestTypeKey);
            Assert.AreEqual(DateTime.SpecifyKind(input.RequestedDate, DateTimeKind.Utc).ToLocalTime(), result.First().RequestedDate);
            Assert.AreEqual(input.ImportedDate, result.First().ImportedDate);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task GetTranscriptRequestProgressResponseModelAsync_should_set_all_properties()
        {
            // Arrange
            var input = new TranscriptRequestProgressViewModel
            {
                Id = 15829124,
                UserAccountId = 178513,
                AvatarFileName = "",
                SchoolCountryType = CountryType.US,
                StudentName = "Uwatowenimana, Jeanne d'Arc",
                DateOfBirth = DateTime.Now.AddYears(-18),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = "JU",
                TranscriptRequestId = 1003442,
                InunId = "7450",
                ReceivingInstitutionCode = 5678,
                ReceivingInstitutionName = "Michigan State University",
                ReceivingInstitutionCity = "Michigan",
                ReceivingInstitutionStateCode = "MI",
                RequestedDate = DateTime.UtcNow.AddDays(-3),
                TranscriptStatus = TranscriptRequestStatus.Submitted,
                TranscriptStatusKey = "TRANSCRIPT_STATUS_SUBMITTED",
                TranscriptStatusDate = DateTime.UtcNow.AddDays(-3)
            };
            var timezoneDetails = new TimeZoneDetailModel { SQLKey = "Eastern Standard Time" };
            _mockAvatarService.Setup(x => x.GetStudentAvatarDefaultUrl()).Returns(_avatarUrl);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneIdByPortfolioIdAsync(It.IsAny<int>())).ReturnsAsync(1);
            _mockTimeZoneRepository.Setup(x => x.GeTimeZoneDetailByIdAsync(It.IsAny<int>())).ReturnsAsync(timezoneDetails);

            // Act
            var result = await CreateService().GetTranscriptRequestProgressResponseModelAsync(new List<TranscriptRequestProgressViewModel> { input }, 1234);

            // Assert
            Assert.AreEqual(input.Id, result.First().Id);
            Assert.AreEqual(_avatarUrl, result.First().AvatarUrl);
            Assert.AreEqual(input.StudentName, result.First().StudentName);
            Assert.AreEqual(input.DateOfBirth, result.First().DateOfBirth);
            Assert.AreEqual(input.GradeId, result.First().GradeId);
            Assert.AreEqual(input.GradeKey, result.First().GradeKey);
            Assert.AreEqual(input.StudentId, result.First().StudentId);
            Assert.AreEqual(input.TranscriptRequestId, result.First().TranscriptRequestId);
            Assert.AreEqual(input.InunId, result.First().InunId);
            Assert.AreEqual(input.ReceivingInstitutionCode, result.First().ReceivingInstitutionCode);
            Assert.AreEqual(input.ReceivingInstitutionName, result.First().ReceivingInstitutionName);
            Assert.AreEqual(input.ReceivingInstitutionCity, result.First().ReceivingInstitutionCity);
            Assert.AreEqual(input.ReceivingInstitutionStateCode, result.First().ReceivingInstitutionStateCode);
            Assert.AreEqual(DateTime.SpecifyKind(input.RequestedDate, DateTimeKind.Utc).ToLocalTime(), result.First().RequestedDate);
            Assert.AreEqual(input.TranscriptStatus, result.First().TranscriptStatus);
            Assert.AreEqual(input.TranscriptStatusKey, result.First().TranscriptStatusKey);
            Assert.AreEqual(DateTime.SpecifyKind(input.TranscriptStatusDate, DateTimeKind.Utc).ToLocalTime(), result.First().TranscriptStatusDate);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task SubmitOutOfNetworkAsync_for_OON_email_should_call_2_sprocs()
        {
            // Arrange 

            // Act
            await CreateService().SubmitOutOfNetworkAsync(1, TranscriptRequestType.Email, receivingInstitutionEmail: "test@test.com");

            // Assert
            _mockTranscriptRequestRepo.Verify(m => m.UpdateTypeAsync(It.IsAny<int>(), It.IsAny<int>()));
            _mockTranscriptRequestRepo.Verify(m => m.AddTranscriptRequestSubmittedAsync(It.IsAny<int>(), It.IsAny<string>(), null));
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task SubmitOutOfNetworkAsync_for_OON_mail_should_call_2_sprocs()
        {
            // Arrange 

            // Act
            await CreateService().SubmitOutOfNetworkAsync(1, TranscriptRequestType.Mail, dateSentByMailUtc: DateTime.Now);

            // Assert
            _mockTranscriptRequestRepo.Verify(m => m.UpdateTypeAsync(It.IsAny<int>(), It.IsAny<int>()));
            _mockTranscriptRequestRepo.Verify(m => m.AddTranscriptRequestSubmittedAsync(It.IsAny<int>(), null, It.IsAny<DateTime>()));
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsDateSentByMailInRange_with_date_post_requestedDate_should_return_false()
        {
            // Arrange
            var requestedDateUtc = DateTime.UtcNow.AddDays(-5);
            var dateSentByMailUtc = DateTime.UtcNow.AddDays(-6);
            _mockTranscriptRequestRepo.Setup(x => x.GetRequestedDateByIdAsync(It.IsAny<int>())).ReturnsAsync(requestedDateUtc);

            // Act
            var result = await CreateService().IsDateSentByMailInRangeAsync(1, dateSentByMailUtc);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsDateSentByMailInRange_with_date_past_today_should_return_false()
        {
            // Arrange
            var requestedDateUtc = DateTime.UtcNow.AddDays(-5);
            var dateSentByMailUtc = DateTime.Now.AddDays(5);
            _mockTranscriptRequestRepo.Setup(x => x.GetRequestedDateByIdAsync(It.IsAny<int>())).ReturnsAsync(requestedDateUtc);

            // Act
            var result = await CreateService().IsDateSentByMailInRangeAsync(1, dateSentByMailUtc);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsDateSentByMailInRange_should_return_true()
        {
            // Arrange
            var requestedDateUtc = DateTime.UtcNow.AddDays(-5);
            var dateSentByMailUtc = DateTime.Now.AddDays(-3);
            _mockTranscriptRequestRepo.Setup(x => x.GetRequestedDateByIdAsync(It.IsAny<int>())).ReturnsAsync(requestedDateUtc);

            // Act
            var result = await CreateService().IsDateSentByMailInRangeAsync(1, dateSentByMailUtc);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task GetDateSentByMailUtc_should_return_requested_date_plus5ms()
        {
            // Arrange
            var requestedDateUtc = DateTime.UtcNow;
            var dateSentByMail = new DateTime(requestedDateUtc.Year, requestedDateUtc.Month, requestedDateUtc.Day);
            _mockTranscriptRequestRepo.Setup(x => x.GetRequestedDateByIdAsync(It.IsAny<int>())).ReturnsAsync(requestedDateUtc);

            // Act
            var result = await CreateService().GetDateSentByMailUtcAsync(1, dateSentByMail, false);
            var expectedValue = requestedDateUtc.Millisecond + 5;

            // Assert
            Assert.AreEqual(expectedValue, result.Millisecond);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task GetDateSentByMailUtc_should_return_date_with_time_set_to_noon()
        {
            // Arrange
            var requestedDateUtc = DateTime.UtcNow;
            var dateSentByMail = requestedDateUtc.AddDays(1);
            _mockTranscriptRequestRepo.Setup(x => x.GetRequestedDateByIdAsync(It.IsAny<int>())).ReturnsAsync(requestedDateUtc);

            // Act
            var result = await CreateService().GetDateSentByMailUtcAsync(1, dateSentByMail, false);
            var expectedValue = new DateTime(dateSentByMail.Year, dateSentByMail.Month, dateSentByMail.Day, 12, 0, 0);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsSendTranscriptRequestInputValidAsync_should_return_false_for_invalid_transcriptrequestId()
        {
            // Arrange

            // Act
            var result = await CreateService().IsSendTranscriptRequestInputValidAsync(null, null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsSendTranscriptRequestInputValidAsync_should_return_false_for_invalid_transcriptrequestType()
        {
            // Arrange
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestTypeId = 2 };
            var submitData = new TranscriptSubmitRequestModel { TranscriptRequestType = (TranscriptRequestType)4 };

            // Act
            var result = await CreateService().IsSendTranscriptRequestInputValidAsync(sendTranscriptInfos, submitData);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsSendTranscriptRequestInputValidAsync_should_return_false_for_invalid_ReceivingInstitutionEmail()
        {
            // Arrange
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestTypeId = 2 };
            var submitData = new TranscriptSubmitRequestModel();

            // Act
            var result = await CreateService().IsSendTranscriptRequestInputValidAsync(sendTranscriptInfos, submitData);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsSendTranscriptRequestInputValidAsync_should_return_false_for_invalid_DateSentByMail()
        {
            // Arrange
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestTypeId = 2 };
            var submitData = new TranscriptSubmitRequestModel { TranscriptRequestType = TranscriptRequestType.Mail, DateSentByMail = DateTime.UtcNow.AddDays(-2) };
            _mockTranscriptRequestRepo.Setup(tr => tr.GetRequestedDateByIdAsync(It.IsAny<int>())).ReturnsAsync(DateTime.UtcNow);

            // Act
            var result = await CreateService().IsSendTranscriptRequestInputValidAsync(sendTranscriptInfos, submitData);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Transcript Request Service")]
        public async Task IsSendTranscriptRequestInputValidAsync_should_return_true_for_valid_input()
        {
            // Arrange
            var sendTranscriptInfos = new SendTranscriptViewModel { TranscriptRequestTypeId = 1 };
            var submitData = new TranscriptSubmitRequestModel { TranscriptRequestType = TranscriptRequestType.InNetwork };

            // Act
            var result = await CreateService().IsSendTranscriptRequestInputValidAsync(sendTranscriptInfos, submitData);

            // Assert
            Assert.IsTrue(result);
        }

        private TranscriptRequestService CreateService()
        {
            return new TranscriptRequestService(_mockTranscriptRequestRepo.Object, _mockInstitutionRepo.Object, _mockAvatarService.Object, _mockTimeZoneRepository.Object);
        }
    }
}
