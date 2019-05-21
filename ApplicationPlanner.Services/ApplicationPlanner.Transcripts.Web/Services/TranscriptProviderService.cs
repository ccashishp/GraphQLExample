using System;
using System.Threading.Tasks;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Core.Models.Exceptions;
using ApplicationPlanner.Transcripts.Core.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ApplicationPlanner.Transcripts.Web.Models;
using CC.Cache;
using CC.Common.Extensions;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface ITranscriptProviderService
    {
        Task SendTranscriptRequestAsync(int transcriptRequestId, string studentId, int transcriptId, int schoolId, string receivingInstitutionCode, string receivingInstitutionName, int modifiedById, string receivingInstitutionEmail = "");
        Task BulkSendTranscriptRequestAsync(IEnumerable<SendTranscriptViewModel> sendTranscriptInfos, int currentSchool, int modifiedById);
        Task DeleteTranscriptAsync(int schoolId, string studentId, int transcriptId);
        Task ImportTranscriptAsync(int schoolId, int educatorId, string fileType, Stream fileStream);
        IEnumerable<InstitutionReceiverModel> GetTranscriptReceiverList();
        IEnumerable<InstitutionReceiverModel> GetTranscriptInNetworkReceiverList();
        IEnumerable<InstitutionReceiverResponseModel> GetInstitutionReceiverResponseModel(IEnumerable<InstitutionReceiverModel> list);
    }

    public class TranscriptProviderService : ITranscriptProviderService 
    {
        private ITranscriptRequestRepository _transcriptRequestRepository;
        private ISchoolSettingRepository _schoolSettingRepository;
        private ITranscriptProviderAPIService _transcriptProviderAPIService;
        private ICache _cache;

        public TranscriptProviderService(
            ITranscriptRequestRepository transcriptRequestRepository,
            ISchoolSettingRepository schoolSettingRepository,
            ITranscriptProviderAPIService transcriptProviderAPIService,
            ICache cache)
        {            
            _transcriptRequestRepository = transcriptRequestRepository;
            _schoolSettingRepository = schoolSettingRepository;
            _transcriptProviderAPIService = transcriptProviderAPIService;
            _cache = cache;
        }

        public async Task SendTranscriptRequestAsync(int transcriptRequestId, string studentId, int transcriptId, int schoolId, string receivingInstitutionCode, string receivingInstitutionName, int modifiedById, string receivingInstitutionEmail = "")
        {
            // Get School Settings
            SchoolSettingModel schoolSettings = await _schoolSettingRepository.GetBySchoolIdAsync(schoolId);

            // License check
            LicenseCheck(schoolSettings);

            // Send the transcript request via API
            await _transcriptProviderAPIService.SendTranscriptRequestAsync(schoolSettings.TranscriptProviderId, transcriptRequestId, studentId, transcriptId, schoolId, receivingInstitutionCode, receivingInstitutionName, receivingInstitutionEmail);

            // Update the transcript request history
            await _transcriptRequestRepository.AppendHistoryAsync(transcriptRequestId, TranscriptRequestStatus.Submitted, modifiedById);
        }

        public async Task BulkSendTranscriptRequestAsync(IEnumerable<SendTranscriptViewModel> sendTranscriptInfos, int currentSchool, int modifiedById)
        {
            // Get School Settings
            SchoolSettingModel schoolSettings = await _schoolSettingRepository.GetBySchoolIdAsync(currentSchool);

            // License check
            LicenseCheck(schoolSettings);
            var successfullySentIdList = new List<int>();

            //@TODO: Find a way to use Task.WhenAll and handle failure
            foreach(SendTranscriptViewModel st in sendTranscriptInfos)
            {
                // Send the transcript request via API
                await _transcriptProviderAPIService.SendTranscriptRequestAsync(schoolSettings.TranscriptProviderId, st.TranscriptRequestId, st.StudentId, st.TranscriptId, st.SchoolId, st.ReceivingInstitutionCode, st.ReceivingInstitutionName);
                successfullySentIdList.Add(st.TranscriptRequestId);
            }
            // Update the transcript request history
            await _transcriptRequestRepository.AppendHistoryAsync(successfullySentIdList, TranscriptRequestStatus.Submitted, modifiedById);
        }

        public async Task DeleteTranscriptAsync(int schoolId, string studentId, int transcriptId)
        {
            // Get School Settings
            SchoolSettingModel schoolSettings = await _schoolSettingRepository.GetBySchoolIdAsync(schoolId);

            // License check
            LicenseCheck(schoolSettings);

            // Delete the transcript via API
            await _transcriptProviderAPIService.DeleteTranscriptAsync(schoolSettings.TranscriptProviderId, schoolId, studentId, transcriptId);
        }

        public async Task ImportTranscriptAsync(int schoolId, int educatorId, string fileType, Stream fileStream)
        {
            // Get School Settings
            SchoolSettingModel schoolSettings = await _schoolSettingRepository.GetBySchoolIdAsync(schoolId);

            // License check
            LicenseCheck(schoolSettings);

            // Send the transcript via API
            await _transcriptProviderAPIService.ImportTranscriptAsync(schoolSettings.TranscriptProviderId, schoolId, fileType, fileStream);
        }

        public IEnumerable<InstitutionReceiverModel> GetTranscriptReceiverList()
        {
            // Get the transcript Receivers from Redis Cache or via API and Cache it
            var cacheKey = _cache.CreateKey("TranscriptReceiverList");
            IEnumerable<InstitutionReceiverModel> transcriptReceiverList = new List<InstitutionReceiverModel>();
            try
            {
                transcriptReceiverList = _cache.Get<IEnumerable<InstitutionReceiverModel>>(cacheKey);
            }
            catch (Exception ex)
            {
                throw new CredentialsReceiverListFetchException(ex.Message);
            }

            if (transcriptReceiverList.IsNotNullOrEmpty())
                return transcriptReceiverList;

            transcriptReceiverList = _transcriptProviderAPIService.GetTranscriptReceiverList();

            if (!transcriptReceiverList.IsNotNullOrEmpty())
                throw new CredentialsReceiverListFetchException("Receiver empty list from Credentials using restSharp");

            _cache.Set(cacheKey, transcriptReceiverList);
            return transcriptReceiverList;
        }
        public IEnumerable<InstitutionReceiverModel> GetTranscriptInNetworkReceiverList()
        {
            // (Stephanie - Credentials) Anything that doesn't have an ESSID (even if it has a CRUZID) should be filtered out because it's not in-network
            return GetTranscriptReceiverList().Where(r => r.CruzId != "" && r.CruzId != "1" && r.EssId != "");
        }

        public IEnumerable<InstitutionReceiverResponseModel> GetInstitutionReceiverResponseModel(IEnumerable<InstitutionReceiverModel> list)
        {
            var result = new List<InstitutionReceiverResponseModel>();
            foreach (var e in list)
            {
                if (!result.Any(i => i.InunId == e.CruzId))
                {
                    var row = new InstitutionReceiverResponseModel()
                    {
                        InunId = e.CruzId,
                        Name = e.Name,
                        ReceiverList = new List<InstitutionReceiverOfficeResponseModel>()
                    };
                    foreach (var receiver in list.Where(r => r.CruzId == e.CruzId))
                    {
                        var r = new InstitutionReceiverOfficeResponseModel()
                        {
                            Id = receiver.EssId,
                            City = receiver.City,
                            State = receiver.State
                        };
                        if (!row.ReceiverList.Any(nr => nr.Id == r.Id))
                            row.ReceiverList.Add(r);
                    }
                    result.Add(row);
                }
            }
            return result;
        }

        private void LicenseCheck(SchoolSettingModel schoolSettings)
        {
            if (schoolSettings == null || !schoolSettings.IsTranscriptEnabled)
                throw new UnlicensedSchoolException("A transcript provider action was attempted with a school that is not licensed with the transcript provider.", "SendTranscriptRequest", schoolSettings.SchoolId);
        }
    }
}
