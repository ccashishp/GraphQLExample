using System;
using ApplicationPlanner.Transcripts.Web.Services;
using ApplicationPlanner.Transcripts.Core.Models.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using ApplicationPlanner.Transcripts.Web.Configuration;
using RestSharp;
using System.IO;

namespace ApplicationPlanner.Tests.Integration.ServiceTests
{
    [TestClass]
    public class CredentialsAPIServiceTests
    {
        int xello_test_account_school_id = 250732;

        #region TranscriptRequest Send Integration Tests
        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscriptRequestSend_NoMatchingTranscript_ShouldThrowTranscriptProviderException()
        {
            // Arrange
            string errorMessage = string.Empty;

            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://www.credentials-inc.com/cgi-bin/ut_cruise.pgm";
            config.Salt = "eZsh9qGzuFP6a5H684arSlMKHCOfPIKSASeeDxSpiPVPw35zonFpVFWugYYlagg6MvwgvfcdlBp0zji0yYo4zAHU2cPvKfKSx351QfwGLkeCmkn20HFwdmFwsvQonykUIchFzoUWKvb2Vsf3SftOFXu2PgMY:UC0KCqayA0BBIdQQ9Zaf6Qnm:ZWPqOnlLtF";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;

            IRestClient restClient = new RestClient(config.Url);

            ITranscriptProviderAPIService credentials = new CredentialsAPIService(restClient, config);

            // Act 
            try
            {
                await credentials.SendTranscriptRequestAsync("transcriptProviderId-1", 1, "1618861", 136182, xello_test_account_school_id, "67499", "Alma College");
            } catch (TranscriptProviderTranscriptRequestSendException ex)
            {
                errorMessage = ex.Message;
            }

            // Assert (no exception thrown)
            Assert.AreEqual(String.Format("NO RECRD: Did not find a control record for 136182 for school {0} for action TRANSCRIPT", xello_test_account_school_id), errorMessage);
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]        
        public async Task CredentialsTranscriptRequestSend_BadSha_ShouldThrowTranscriptProviderException()
        {
            // Arrange
            string errorMessage = string.Empty;

            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://www.credentials-inc.com/cgi-bin/ut_cruise.pgm";
            config.Salt = "badsalt";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;
           
            IRestClient restClient = new RestClient(config.Url);

            ITranscriptProviderAPIService credentials = new CredentialsAPIService(restClient, config);

            // Act 
            try
            {
                await credentials.SendTranscriptRequestAsync("transcriptProviderId-1", 1, "1618861", 136182, xello_test_account_school_id, "67499", "Alma College");
            } catch (TranscriptProviderTranscriptRequestSendException ex)
            {
                errorMessage = ex.Message;
            }

            // Assert (Exception thrown)
            Assert.AreEqual("ERROR: SHA Failed", errorMessage);
        }
        #endregion

        #region Transcript Delete Integration Tests
        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscriptDelete_NoMatchingTranscript_ShouldThrowTranscriptProviderException()
        {
            // Arrange
            string errorMessage = string.Empty;

            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://www.credentials-inc.com/cgi-bin/ut_cruise.pgm";
            config.Salt = "eZsh9qGzuFP6a5H684arSlMKHCOfPIKSASeeDxSpiPVPw35zonFpVFWugYYlagg6MvwgvfcdlBp0zji0yYo4zAHU2cPvKfKSx351QfwGLkeCmkn20HFwdmFwsvQonykUIchFzoUWKvb2Vsf3SftOFXu2PgMY:UC0KCqayA0BBIdQQ9Zaf6Qnm:ZWPqOnlLtF";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;

            IRestClient restClient = new RestClient(config.Url);

            ITranscriptProviderAPIService credentials = new CredentialsAPIService(restClient, config);

            // Act 
            try
            {
                await credentials.DeleteTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "Student123", 55);
            }
            catch (TranscriptProviderTranscriptDeleteException ex)
            {
                errorMessage = ex.Message;
            }

            // Assert (no exception thrown)
            Assert.AreEqual(String.Format("NO RECRD: Did not find a control record for 55 for school {0} for action DELETE_TRANSCR", xello_test_account_school_id), errorMessage);
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscriptDelete_BadSha_ShouldThrowTranscriptProviderException()
        {
            // Arrange
            string errorMessage = string.Empty;

            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://www.credentials-inc.com/cgi-bin/ut_cruise.pgm";
            config.Salt = "badsalt";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;

            IRestClient restClient = new RestClient(config.Url);

            ITranscriptProviderAPIService credentials = new CredentialsAPIService(restClient, config);

            // Act 
            try
            {
                await credentials.DeleteTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "Student123", 55);
            }
            catch (TranscriptProviderTranscriptDeleteException ex)
            {
                errorMessage = ex.Message;
            }

            // Assert (Exception thrown)
            Assert.AreEqual("ERROR: SHA Failed", errorMessage);
        }
        #endregion

        #region Transcript Send Integration Tests
        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscriptSend_Success()
        {
            // Arrange
            string errorMessage = string.Empty;

            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://www.credentials-inc.com/cgi-bin/ut_cruise.pgm";
            config.Salt = "eZsh9qGzuFP6a5H684arSlMKHCOfPIKSASeeDxSpiPVPw35zonFpVFWugYYlagg6MvwgvfcdlBp0zji0yYo4zAHU2cPvKfKSx351QfwGLkeCmkn20HFwdmFwsvQonykUIchFzoUWKvb2Vsf3SftOFXu2PgMY:UC0KCqayA0BBIdQQ9Zaf6Qnm:ZWPqOnlLtF";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;

            IRestClient restClient = new RestClient(config.Url);

            ITranscriptProviderAPIService credentials = new CredentialsAPIService(restClient, config);

            // Setup the sample file to send.
            MemoryStream memStream = new MemoryStream();
            using (FileStream file = new FileStream("Resources\\SampleTranscript.pdf", FileMode.Open, FileAccess.Read))
                file.CopyTo(memStream);

            // Act // TODO: commenting this out until Credentials fixes their end
            // await credentials.ImportTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "pdf", memStream);

            // Assert (no exception thrown)            
        }

        [TestMethod]
        [TestCategory("Credentials API Service")]
        public async Task CredentialsTranscriptSend_BadSha_ShouldThrowTranscriptProviderException()
        {
            // Arrange
            string errorMessage = string.Empty;

            CredentialsAPIConfig config = new CredentialsAPIConfig();
            config.Url = "https://www.credentials-inc.com/cgi-bin/ut_cruise.pgm";
            config.Salt = "badsalt";
            config.Environment = "TEST";
            config.TestSchoolId = xello_test_account_school_id;

            IRestClient restClient = new RestClient(config.Url);

            ITranscriptProviderAPIService credentials = new CredentialsAPIService(restClient, config);

            // Setup the sample PDF to send.
            MemoryStream memStream = new MemoryStream();
            using (FileStream file = new FileStream("Resources\\SampleTranscript.pdf", FileMode.Open, FileAccess.Read))
                file.CopyTo(memStream);

            // Act 
            try
            {
                await credentials.ImportTranscriptAsync("transcriptProviderId-1", xello_test_account_school_id, "pdf", memStream);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            // Assert (Exception thrown)
            Assert.AreEqual("ERROR: SHA Failed", errorMessage);
        }
        #endregion 

    }
}
