using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Models.Exceptions;
using ApplicationPlanner.Transcripts.Web.Services;
using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.IO;
using CC.Cache;
using System.Collections.Generic;
using System.Linq;
using ApplicationPlanner.Transcripts.Web.Models;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class TranscriptProviderServiceUnitTests
    {        
        private Mock<ITranscriptRequestRepository> _mockTranscriptRequestRepository;
        private Mock<ISchoolSettingRepository> _mockSchoolSettingRepository;
        private Mock<ITranscriptProviderAPIService> _mockTranscriptProviderAPIService;
        private Mock<ICache> _mockCache;
        int xello_test_account_school_id = 250732;

        [TestInitialize]
        public void Test_Init()
        {            
            _mockTranscriptRequestRepository = new Mock<ITranscriptRequestRepository>();
            _mockSchoolSettingRepository = new Mock<ISchoolSettingRepository>();
            _mockTranscriptProviderAPIService = new Mock<ITranscriptProviderAPIService>();
            _mockCache = new Mock<ICache>();
        }

        #region TranscriptRequestSend Unit Tests
        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public async Task TranscriptRequest_Send_Success()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel();
            schoolSettings.SchoolSettingId = 1;
            schoolSettings.SchoolId = xello_test_account_school_id;
            schoolSettings.IsTranscriptEnabled = true;

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));
            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.SendTranscriptRequestAsync(55, "12345", 1, xello_test_account_school_id, "45784", "NCAA", 1);

            // Assert (ran the underlying API call)
            _mockTranscriptProviderAPIService.Verify(m => m.SendTranscriptRequestAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), ""), Times.Once());

            // Assert (appended to transcript request history)
            _mockTranscriptRequestRepository.Verify(m => m.AppendHistoryAsync(It.IsAny<int>(), TranscriptRequestStatus.Submitted, It.IsAny<int>()));
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public async Task TranscriptRequest_SendWithEmailParameter_Success()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = true
            };

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.SendTranscriptRequestAsync(55, "12345", 1, xello_test_account_school_id, "45784", "NCAA", 1, "bob@sampleregistrar.com");

            // Assert (ran the underlying API call)
            _mockTranscriptProviderAPIService.Verify(m => m.SendTranscriptRequestAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),"bob@sampleregistrar.com"), Times.Once());

            // Assert (appended to transcript request history)
            _mockTranscriptRequestRepository.Verify(m => m.AppendHistoryAsync(It.IsAny<int>(), TranscriptRequestStatus.Submitted, It.IsAny<int>()));
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        // If a school isn't licensed with the transcript provider, we need to throw an exception that contains 
        // the relevant details of the action being attempted as well as the unlicensed school details.
        [ExpectedException(typeof(UnlicensedSchoolException))]
        public async Task TranscriptRequest_SendForUnlicensedSchool_ShouldThrowUnlicensedSchoolException()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = false
            };

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.SendTranscriptRequestAsync(55, "12345", 1, 12345, "45784", "NCAA", 1, "");

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        // If a school isn't licensed with the transcript provider, we need to throw an exception that contains 
        // the relevant details of the action being attempted as well as the unlicensed school details.
        [ExpectedException(typeof(UnlicensedSchoolException))]
        public async Task TranscriptRequest_SendForInactiveSchool_ShouldThrowUnlicensedSchoolException()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel();
            schoolSettings.SchoolSettingId = 1;
            schoolSettings.SchoolId = 12345;
            schoolSettings.IsTranscriptEnabled = false;

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));
            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.SendTranscriptRequestAsync(55, "12345", 1, 12345, "45784", "NCAA", 1, "");

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public async Task TranscriptRequest_Bulk_Send_Success()
        {
            // Arrange
            List<SendTranscriptViewModel> input = new List<SendTranscriptViewModel>()
            {
                new SendTranscriptViewModel { TranscriptRequestId = 65, StudentId = "12345", TranscriptId = 1, SchoolId = xello_test_account_school_id, ReceivingInstitutionCode = "45784", ReceivingInstitutionName = "NCAA" },
                new SendTranscriptViewModel { TranscriptRequestId = 83, StudentId = "12345", TranscriptId = 1, SchoolId = xello_test_account_school_id, ReceivingInstitutionCode = "45785", ReceivingInstitutionName = "Xello College" },
                new SendTranscriptViewModel { TranscriptRequestId = 94, StudentId = "12345", TranscriptId = 1, SchoolId = xello_test_account_school_id, ReceivingInstitutionCode = "45786", ReceivingInstitutionName = "Great College" }
            };
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = true
            };
            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            // Act
            await CreateService().BulkSendTranscriptRequestAsync(input, xello_test_account_school_id, 1);

            // Assert (ran the underlying API call) in loop
            _mockTranscriptProviderAPIService.Verify(m => m.SendTranscriptRequestAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), ""), Times.Exactly(3));

            // Assert (appended to transcript request history)
            _mockTranscriptRequestRepository.Verify(m => m.AppendHistoryAsync(It.IsAny<List<int>>(), TranscriptRequestStatus.Submitted, It.IsAny<int>()));
        }
        #endregion

        #region TranscriptDelete Unit Tests
        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public async Task Transcript_Delete_Success()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = true
            };

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.DeleteTranscriptAsync(xello_test_account_school_id, "Student1234", 55);

            // Assert (ran the underlying API call)
            _mockTranscriptProviderAPIService.Verify(m => m.DeleteTranscriptAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        // If a school isn't licensed with the transcript provider, we need to throw an exception that contains 
        // the relevant details of the action being attempted as well as the unlicensed school details.
        [ExpectedException(typeof(UnlicensedSchoolException))]
        public async Task TranscriptRequest_DeleteForUnlicensedSchool_ShouldThrowUnlicensedSchoolException()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = false
            };

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.DeleteTranscriptAsync(xello_test_account_school_id, "Student1234", 55);

            // Assert (ExpectedException)  -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        // If a school isn't licensed with the transcript provider, we need to throw an exception that contains 
        // the relevant details of the action being attempted as well as the unlicensed school details.
        [ExpectedException(typeof(UnlicensedSchoolException))]
        public async Task TranscriptRequest_DeleteForInactiveSchool_ShouldThrowUnlicensedSchoolException()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel();
            schoolSettings.SchoolSettingId = 1;
            schoolSettings.SchoolId = xello_test_account_school_id;
            schoolSettings.IsTranscriptEnabled = false;

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();

            // Act
            await tps.DeleteTranscriptAsync(xello_test_account_school_id, "Student1234", 55);

            // Assert (ExpectedException)  -> See the applied ExpectedException attribute to the test's method
        }

        #endregion

        #region TranscriptImport Unit Tests
        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public async Task Transcript_Import_Success()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = true
            };

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();
            MemoryStream fileStream = new MemoryStream();

            // Act            
            await tps.ImportTranscriptAsync(xello_test_account_school_id, 12, "pdf", fileStream);

            // Assert (ran the underlying API call)
            _mockTranscriptProviderAPIService.Verify(m => m.ImportTranscriptAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<MemoryStream>()), Times.Once());
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        // If a school isn't licensed with the transcript provider, we need to throw an exception that contains 
        // the relevant details of the action being attempted as well as the unlicensed school details.
        [ExpectedException(typeof(UnlicensedSchoolException))]
        public async Task Transcript_ImportForUnlicensedSchool_ShouldThrowUnlicensedSchoolException()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel
            {
                SchoolSettingId = 1,
                SchoolId = xello_test_account_school_id,
                IsTranscriptEnabled = false
            };

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();
            MemoryStream fileStream = new MemoryStream();

            // Act
            await tps.ImportTranscriptAsync(xello_test_account_school_id, 12, "pdf", fileStream);

            // Assert (ExpectedException)  -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        // If a school isn't licensed with the transcript provider, we need to throw an exception that contains 
        // the relevant details of the action being attempted as well as the unlicensed school details.
        [ExpectedException(typeof(UnlicensedSchoolException))]
        public async Task Transcript_ImportInactiveSchool_ShouldThrowUnlicensedSchoolException()
        {
            // Arrange
            SchoolSettingModel schoolSettings = new SchoolSettingModel();
            schoolSettings.SchoolSettingId = 1;
            schoolSettings.SchoolId = xello_test_account_school_id;
            schoolSettings.IsTranscriptEnabled = false;

            _mockSchoolSettingRepository.Setup(x => x.GetBySchoolIdAsync(It.IsAny<int>())).Returns(Task.FromResult<SchoolSettingModel>(schoolSettings));

            ITranscriptProviderService tps = CreateService();
            MemoryStream fileStream = new MemoryStream();

            // Act
            await tps.ImportTranscriptAsync(xello_test_account_school_id, 12, "pdf", fileStream);

            // Assert (ExpectedException)  -> See the applied ExpectedException attribute to the test's method
        }
        #endregion 

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public void GetTranscriptReceiverList_ShouldReturnListFromCache()
        {
            // Arrange
            var transcriptReceiverList = new List<InstitutionReceiverModel>()
            {
                new InstitutionReceiverModel
                {
                    Name = "Receiving Institution #1",
                    CruzId = "123",
                    Fice = "456"
                },
                new InstitutionReceiverModel
                {
                    Name = "Receiving Institution #2",
                    CruzId = "123",
                    Fice = "456"
                }
            };
            _mockCache.Setup(x => x.CreateKey(It.IsAny<string>())).Returns("cacheKey");
            _mockCache.Setup(x => x.Get<IEnumerable<InstitutionReceiverModel>>(It.IsAny<string>())).Returns(transcriptReceiverList);

            // Act
            var result = CreateService().GetTranscriptReceiverList();

            // Assert
            Assert.AreSame(transcriptReceiverList, result);
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public void GetTranscriptReceiverList_ShouldReturnListFromProviderApiService()
        {
            // Arrange
            _mockCache.Setup(x => x.CreateKey(It.IsAny<string>())).Returns("cacheKey");
             var transcriptReceiverList = new List<InstitutionReceiverModel>()
            {
                new InstitutionReceiverModel
                {
                    Name = "Receiving Institution #1",
                    CruzId = "123",
                    Fice = "456"
                },
                new InstitutionReceiverModel
                {
                    Name = "Receiving Institution #2",
                    CruzId = "123",
                    Fice = "456"
                }
            };
           _mockCache.Setup(x => x.Get<IEnumerable<InstitutionReceiverModel>>(It.IsAny<string>())).Returns(new List<InstitutionReceiverModel>());
            _mockTranscriptProviderAPIService.Setup(x => x.GetTranscriptReceiverList()).Returns(transcriptReceiverList);

            // Act
            var result = CreateService().GetTranscriptReceiverList();

            // Assert
            Assert.AreSame(transcriptReceiverList, result);
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public void GetTranscriptInNetworkReceiverList_ShouldFilterOutInstitutionsWithInvalidCruzIdAndEssId()
        {
            // Arrange
            var transcriptReceiverList = new List<InstitutionReceiverModel>()
            {
                new InstitutionReceiverModel
                {
                    Name = "Valid Receiving Institution",
                    CruzId = "123",
                    EssId = "456"
                },
                new InstitutionReceiverModel
                {
                    Name = "Invalid Receiving Institution #1",
                    CruzId = ""
                },
                new InstitutionReceiverModel
                {
                    Name = "Invalid Receiving Institution #2",
                    CruzId = "1"
                },
                new InstitutionReceiverModel
                {
                    Name = "Invalid Receiving Institution #3",
                    EssId = ""
                }
            };
            _mockCache.Setup(x => x.CreateKey(It.IsAny<string>())).Returns("cacheKey");
            _mockCache.Setup(x => x.Get<IEnumerable<InstitutionReceiverModel>>(It.IsAny<string>())).Returns(transcriptReceiverList);

            // Act
            var result = CreateService().GetTranscriptInNetworkReceiverList().ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Valid Receiving Institution", result.First().Name);
        }

        [TestMethod]
        [TestCategory("Transcript Provider Service")]
        public void GetInstitutionReceiverResponseModel_ShoulReturnListOfResponseModel()
        {
            // Arrange
            var transcriptReceiverList = new List<InstitutionReceiverModel>()
            {
                new InstitutionReceiverModel
                {
                    Name = "Receiving Institution #1",
                    CruzId = "1",
                    EssId = "11",
                    City = "City 1",
                    State = "State 1"
                },
                new InstitutionReceiverModel
                {
                    Name = "Multiple Campuses Receiving Institution #2",
                    CruzId = "2",
                    EssId = "21",
                    City = "City 21",
                    State = "State 2"
                },
                new InstitutionReceiverModel
                {
                    Name = "Multiple Campuses Receiving Institution #2",
                    CruzId = "2",
                    EssId = "22",
                    City = "City 22",
                    State = "State 2"
                },
                new InstitutionReceiverModel
                {
                    Name = "Duplicated Multiple Campuses Receiving Institution #2",
                    CruzId = "2",
                    EssId = "22",
                    City = "City 22",
                    State = "State 2"
                }
            };

            var transcriptReceiverResponseModelList = new List<InstitutionReceiverResponseModel>()
            {
                new InstitutionReceiverResponseModel
                {
                    Name = "Receiving Institution #1",
                    InunId = "1",
                    ReceiverList = new List<InstitutionReceiverOfficeResponseModel>()
                    {
                        new InstitutionReceiverOfficeResponseModel(){ Id = "11", City = "City 1", State = "State 1" }
                    }
                },
                new InstitutionReceiverResponseModel
                {
                    Name = "Multiple Campuses Receiving Institution #2",
                    InunId = "2",
                    ReceiverList = new List<InstitutionReceiverOfficeResponseModel>()
                    {
                        new InstitutionReceiverOfficeResponseModel(){ Id = "21", City = "City 21", State = "State 2" },
                        new InstitutionReceiverOfficeResponseModel(){ Id = "22", City = "City 22", State = "State 2" }
                    }
                }
            };
            
            // Act
            var result = CreateService().GetInstitutionReceiverResponseModel(transcriptReceiverList).ToList();

            // Assert
            // Make sure multiple campuses are set correctly
            CollectionAssert.AreEqual(transcriptReceiverResponseModelList, result);
        }

        private TranscriptProviderService CreateService()
        {
            return new TranscriptProviderService(
                _mockTranscriptRequestRepository.Object,
                _mockSchoolSettingRepository.Object,
                _mockTranscriptProviderAPIService.Object,
                _mockCache.Object);
        }
    }
}
