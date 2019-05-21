using ApplicationPlanner.Transcripts.Web.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApplicationPlanner.Transcripts.Core.Models.Exceptions;
using ApplicationPlanner.Transcripts.Web.Models;
using RestSharp;
using System.IO;
using ApplicationPlanner.Transcripts.Core.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface ITranscriptProviderAPIService
    {
        Task SendTranscriptRequestAsync(string transcriptProviderId, int transcriptRequestId, string studentId, int transcriptId, int schoolId, string receivingInstitutionCode, string receivingInstitutionName, string receivingInstitutionEmail = "");
        Task DeleteTranscriptAsync(string transcriptProviderId, int schoolId, string studentId, int transcriptId);
        Task ImportTranscriptAsync(string transcriptProviderId, int schoolId, string fileType, Stream fileStream);
        IEnumerable<InstitutionReceiverModel> GetTranscriptReceiverList();
    }

    public class CredentialsAPIService : ITranscriptProviderAPIService 
    {
        private IRestClient _restClient;
        private CredentialsAPIConfig _config;
        public const string TEST_ENVIRONMENT = "TEST";
        public const string PROD_ENVIRONMENT = "PROD";
        public const string PREFIX_FAKE_TRANSCRIPT_PROVIDER_ID = "credentials-";
        public const int INTEGRATION_TEST_TRANSCRIPT_REQUEST_ID = 1;

        public CredentialsAPIService(IRestClient restClient, CredentialsAPIConfig config)
        {
            _restClient = restClient;
            _config = config;
        }

        public async Task SendTranscriptRequestAsync(string transcriptProviderId, int transcriptRequestId, string studentId, int transcriptId, int schoolId, string receivingInstitutionCode, string receivingInstitutionName, string receivingInstitutionEmail = "")
        {
            // @TODO: Find a better way of testing Credenitals API
            // If we're not in a production environment, we're stuck using one single school that is specifically setup 
            // in Credentials for testing -- there's no proper test environment in Credentials :(
            // Same for all other info we pass to Credentials when sending a request
            if (_config.Environment.ToUpper() == TEST_ENVIRONMENT) // Bypass Credentials in the test environment and not for Integration test
            {
                if (transcriptRequestId == INTEGRATION_TEST_TRANSCRIPT_REQUEST_ID)
                    schoolId = _config.TestSchoolId;
                else
                    return;
            }

            if (IsSalesAccountInProd(transcriptProviderId)) // Bypass Credentials in the PROD environment and for Sales Accounts
                return;

            string action = "TRANSCRIPT_REQUEST";
            string now = GetUTCNowFormatted();
            string sha = GetCredentialsHash(transcriptId, action, now);
            string transcriptName = schoolId + "_" + studentId + "_" + transcriptId;
            
            var request = new RestRequest(Method.POST);                
            request.AddParameter("ACTION", action);
            request.AddParameter("DATETIMENOW", now);
            request.AddParameter("SHAHASH", sha);
            request.AddParameter("REQUEST_ID", transcriptRequestId);            
            request.AddParameter("TRANSCRIPT_NAME", transcriptName);
            request.AddParameter("RECEIVING_CODE", receivingInstitutionCode);
            request.AddParameter("RECEIVING_NAME", receivingInstitutionName);
            request.AddParameter("RECEIVING_EMAIL", receivingInstitutionEmail);
            request.AddParameter("ENVIRONMENT", _config.Environment.ToUpper());

            var response = await _restClient.ExecuteTaskAsync<CredentialsAPIResponseModel>(request);

            // Validate for connection issues.
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new TranscriptProviderTranscriptRequestSendException(string.Format("Unable to communicate with transcript provider API. Status code {0}", response.StatusCode), transcriptRequestId, studentId, transcriptId, schoolId, receivingInstitutionCode, receivingInstitutionName, receivingInstitutionEmail);

            // Validate for Credentials errors.
            if (response.Data.STATUS != "SUCCESS")
                throw new TranscriptProviderTranscriptRequestSendException(string.Format("{0}: {1}", response.Data.STATUS, response.Data.ERROR), transcriptRequestId, studentId, transcriptId, schoolId, receivingInstitutionCode, receivingInstitutionName, receivingInstitutionEmail);
        }

        public async Task DeleteTranscriptAsync(string transcriptProviderId, int schoolId, string studentId, int transcriptId)
        {
            // @TODO: Find a better way of testing Credenitals API
            // If we're not in a production environment, we're stuck using one single school that is specifically setup 
            // in Credentials for testing -- there's no proper test environment in Credentials :(
            if (_config.Environment.ToUpper() == TEST_ENVIRONMENT)
                schoolId = _config.TestSchoolId;

            if (IsSalesAccountInProd(transcriptProviderId)) // Bypass Credentials in the PROD environment and for Sales Accounts
                return;

            string action = "DELETE_TRANSCRIPT";
            string now = GetUTCNowFormatted();
            string sha = GetCredentialsHash(transcriptId, action, now);
            string transcriptName = schoolId + "_" + studentId + "_" + transcriptId;

            var request = new RestRequest(Method.POST);
            request.AddParameter("ACTION", action);
            request.AddParameter("DATETIMENOW", now);
            request.AddParameter("SHAHASH", sha);
            request.AddParameter("TRANSCRIPT_NAME", transcriptName);
            request.AddParameter("ENVIRONMENT", _config.Environment.ToUpper());

            var response = await _restClient.ExecuteTaskAsync<CredentialsAPIResponseModel>(request);

            // Validate for connection issues.
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new TranscriptProviderTranscriptDeleteException(string.Format("Unable to communicate with transcript provider API. Status code {0}", response.StatusCode), schoolId, studentId, transcriptId);

            // Validate for Credentials errors.
            if (response.Data.STATUS != "SUCCESS")
                throw new TranscriptProviderTranscriptDeleteException(string.Format("{0}: {1}", response.Data.STATUS, response.Data.ERROR), schoolId, studentId, transcriptId);
        }

        public async Task ImportTranscriptAsync(string transcriptProviderId, int schoolId, string fileType, Stream fileStream)
        {
            // @TODO: Find a better way of testing Credenitals API
            // If we're not in a production environment, we're stuck using one single school that is specifically setup 
            // in Credentials for testing -- there's no proper test environment in Credentials :(
            if (_config.Environment.ToUpper() == TEST_ENVIRONMENT)
                schoolId = _config.TestSchoolId;

            if (IsSalesAccountInProd(transcriptProviderId)) // Bypass Credentials in the PROD environment and for Sales Accounts
                return;

            string action = "BATCH_TRANSCRIPT";
            string now = GetUTCNowFormatted();
            string sha = GetCredentialsHash(schoolId, action, now);

            MemoryStream ms = new MemoryStream();
            fileStream.CopyTo(ms);
            byte[] rawbytes = ms.ToArray();
            string encodedFile = Convert.ToBase64String(rawbytes);
            int fileSize = rawbytes.Length;

            var request = new RestRequest(Method.POST);
            request.AddParameter("ACTION", action);
            request.AddParameter("DATETIMENOW", now);
            request.AddParameter("SHAHASH", sha);
            request.AddParameter("SENDING_CODE", schoolId);
            request.AddParameter("ENVIRONMENT", _config.Environment.ToUpper());
            request.AddParameter("DOC_TYPE", fileType);
            request.AddParameter("PDF_DOC", encodedFile);

            var response = await _restClient.ExecuteTaskAsync<CredentialsAPIResponseModel>(request);

            // Validate for connection issues.
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new TranscriptProviderTranscriptSendException(string.Format("Unable to communicate with transcript provider API. Status code {0}", response.StatusCode), schoolId, fileType, fileSize);

            // Validate for Credentials errors.
            if (response.Data.STATUS != "SUCCESS")
                throw new TranscriptProviderTranscriptSendException(string.Format("{0}: {1}", response.Data.STATUS, response.Data.ERROR), schoolId, fileType, fileSize);
        }

        public IEnumerable<InstitutionReceiverModel> GetTranscriptReceiverList()
        {
            string url = _config.InNetworkUrl;
            var request = new RestRequest(url) { Method = Method.GET };
            request.Timeout = 20 * 1000;
            var response = BuildResponse(_restClient.ExecuteTaskAsync(request).Result);
            return response;
        }

        #region Private Methods
        private string GetUTCNowFormatted()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public string GetCredentialsHash(int id, string Action, string now)
        {
            string credString = now + id + Action + _config.Salt;

            return GetHashSha256(credString);
        }

        private string GetHashSha256(string text)
        {
            byte[] newbytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hasher = new SHA256Managed();
            byte[] third = hasher.ComputeHash(newbytes);

            return Convert.ToBase64String(third);
        }

        private IEnumerable<InstitutionReceiverModel> BuildResponse(IRestResponse result)
        {
            var content = (CredentialsReceiversDto)JsonConvert.DeserializeObject(result.Content, typeof(CredentialsReceiversDto));
            // Special case for NCAA
            var receivers = content.ReceiverList.Receiver.ToList();
            receivers.Find(r => r.EssId == "45784").CruzId = "45784";
            return receivers;
        }

        private bool IsSalesAccountInProd(string transcriptProviderId)
        {
            return _config.Environment.ToUpper() == PROD_ENVIRONMENT && transcriptProviderId.Trim().StartsWith(PREFIX_FAKE_TRANSCRIPT_PROVIDER_ID);
        }
        #endregion 
    }
}
