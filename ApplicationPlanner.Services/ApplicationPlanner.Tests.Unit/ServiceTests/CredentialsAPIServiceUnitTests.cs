using ApplicationPlanner.Transcripts.Web.Services;
using ApplicationPlanner.Transcripts.Core.Models.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using ApplicationPlanner.Transcripts.Web.Configuration;
using RestSharp;
using ApplicationPlanner.Transcripts.Web.Models;
using System.IO;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class CredentialsAPIServiceUnitTests
    {
        ITranscriptProviderAPIService _transcriptAPIProviderService;
        private Mock<IRestClient> _mockRestClient;
        int xello_test_account_school_id = 250732;

        [TestInitialize]
        public void Test_Init()
        {
            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://samplecredentialsapi.com";
            config.Salt = "12345";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;

            _mockRestClient = new Mock<IRestClient>();
            
            _transcriptAPIProviderService = new CredentialsAPIService(_mockRestClient.Object, config);
        }

        #region TranscriptRequestSend Unit Tests
        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscriptRequest_Send_Success()
        {
            // Arrange
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.OK;
            response.Object.Data = new CredentialsAPIResponseModel();
            response.Object.Data.STATUS = "SUCCESS";
            response.Object.Data.ERROR = null;
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            // Act
            await _transcriptAPIProviderService.SendTranscriptRequestAsync("transcriptProviderId-1", 1, "12345", 1, xello_test_account_school_id, "45784", "NCAA");

            // Assert (No exception)
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        [ExpectedException(typeof(TranscriptProviderTranscriptRequestSendException))]
        public async Task CredentialsTranscriptRequestSend_EndpointNotFound_ShouldThrowTranscriptProviderException()
        {
            // Arrange (the API call to Credentials results in a 401)
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.NotFound;
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            // Act
            await _transcriptAPIProviderService.SendTranscriptRequestAsync("transcriptProviderId-1", 1, "12345", 1, xello_test_account_school_id, "45784", "NCAA");

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        [ExpectedException(typeof(TranscriptProviderTranscriptRequestSendException))]

        public async Task CredentialsTranscriptRequestSend_CredentialsCustomError_ShouldThrowTranscriptProviderException()
        {
            // Arrange (the API call to Credentials results in an error response)
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.OK;
            response.Object.Data = new CredentialsAPIResponseModel();
            response.Object.Data.STATUS = "ERROR";
            response.Object.Data.ERROR = "Mock Error";
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            // Act
            await _transcriptAPIProviderService.SendTranscriptRequestAsync("transcriptProviderId-1", 1, "12345", 1, xello_test_account_school_id, "45784", "NCAA");

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }
        #endregion

        #region TranscriptDelete Unit Tests
        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscript_Delete_Success()
        {
            // Arrange
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.OK;
            response.Object.Data = new CredentialsAPIResponseModel();
            response.Object.Data.STATUS = "SUCCESS";
            response.Object.Data.ERROR = null;
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            // Act
            await _transcriptAPIProviderService.DeleteTranscriptAsync("transcriptProviderId-1", 12345, "Student123", 55);

            // Assert (No exception)
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        [ExpectedException(typeof(TranscriptProviderTranscriptDeleteException))]
        public async Task CredentialsTranscriptRequestDelete_EndpointNotFound_ShouldThrowException()
        {
            // Arrange (the API call to Credentials results in a 401)
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.NotFound;
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            // Act
            await _transcriptAPIProviderService.DeleteTranscriptAsync("transcriptProviderId-1", 12345, "Student123", 55);

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        [ExpectedException(typeof(TranscriptProviderTranscriptDeleteException))]

        public async Task CredentialsTranscriptRequestSend_CredentialsCustomError_ShouldThrowException()
        {
            // Arrange (the API call to Credentials results in an error response)
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.OK;
            response.Object.Data = new CredentialsAPIResponseModel();
            response.Object.Data.STATUS = "ERROR";
            response.Object.Data.ERROR = "Mock Error";
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            // Act
            await _transcriptAPIProviderService.DeleteTranscriptAsync("transcriptProviderId-1", 12345, "Student123", 55);

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }
        #endregion

        #region TranscriptImport Unit Tests
        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscript_Import_Success()
        {
            // Arrange
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.OK;
            response.Object.Data = new CredentialsAPIResponseModel();
            response.Object.Data.STATUS = "SUCCESS";
            response.Object.Data.ERROR = null;
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            MemoryStream fileStream = new MemoryStream();

            // Act
            await _transcriptAPIProviderService.ImportTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "pdf", fileStream);

            // Assert (No exception)
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        [ExpectedException(typeof(TranscriptProviderTranscriptSendException))]
        public async Task CredentialsTranscriptImport_EndpointNotFound_ShouldThrowException()
        {
            // Arrange (the API call to Credentials results in a 401)
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.NotFound;
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            MemoryStream fileStream = new MemoryStream();

            // Act
            await _transcriptAPIProviderService.ImportTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "pdf", fileStream);

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        [ExpectedException(typeof(TranscriptProviderTranscriptSendException))]

        public async Task CredentialsTranscriptImport_CredentialsCustomError_ShouldThrowException()
        {
            // Arrange (the API call to Credentials results in an error response)
            var response = new Mock<RestResponse<CredentialsAPIResponseModel>>();
            response.Object.StatusCode = System.Net.HttpStatusCode.OK;
            response.Object.Data = new CredentialsAPIResponseModel();
            response.Object.Data.STATUS = "ERROR";
            response.Object.Data.ERROR = "Mock Error";
            _mockRestClient.Setup(s => s.ExecuteTaskAsync<CredentialsAPIResponseModel>(It.IsAny<RestRequest>())).ReturnsAsync(response.Object);

            MemoryStream fileStream = new MemoryStream();

            // Act
            await _transcriptAPIProviderService.ImportTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "pdf", fileStream);

            // Assert (ExpectedException) -> See the applied ExpectedException attribute to the test's method
        }
        #endregion 
    }
}
