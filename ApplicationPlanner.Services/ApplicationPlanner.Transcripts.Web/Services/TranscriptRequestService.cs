using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Helpers;
using ApplicationPlanner.Transcripts.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface ITranscriptRequestService
    {
        Task<IEnumerable<TranscriptRequestResponseModel>> GetTranscriptRequestByPortfolioIdAsync(int portfolioId, int userAccountId);
        Task<IEnumerable<TranscriptRequestTimelineResponseModel>> GetTranscriptRequestsTimelineByPortfolioIdAsync(int portfolioId, int userAccountId, int translationLanguageId, bool includeRequested = true);
        Task<IEnumerable<TranscriptRequestTimelineResponseModelV2>> GetAllHistoryByPortfolioIdAsync(int portfolioId, int userAccountId, int translationLanguageId, bool includeRequested = true);
        Func<TranscriptRequestStudentViewModel, bool> GetLatestRequestsSearchPredicate(string query);
        Task<IEnumerable<TranscriptRequestStudentResponseModel>> GetTranscriptRequestStudentResponseModelAsync(IEnumerable<TranscriptRequestStudentViewModel> transcriptRequestStudentLinqedList, int schoolId);
        Func<TranscriptRequestProgressViewModel, bool> GetTranscriptRequestProgressSearchPredicate(string query);
        Task<IEnumerable<TranscriptRequestProgressResponseModel>> GetTranscriptRequestProgressResponseModelAsync(IEnumerable<TranscriptRequestProgressViewModel> transcriptRequestProgressLinqedList, int schoolId);
        Task SubmitOutOfNetworkAsync(int id, TranscriptRequestType transcriptRequestType, string receivingInstitutionEmail = null, DateTime? dateSentByMailUtc = null);
        Task<bool> IsDateSentByMailInRangeAsync(int id, DateTime dateSentByMailUtc);
        Task<DateTime> GetDateSentByMailUtcAsync(int id, DateTime dateSentByMail, bool skipValidationForDateSentByMail);
        Task<bool> IsSendTranscriptRequestInputValidAsync(SendTranscriptViewModel sendTranscriptInfos, TranscriptSubmitRequestModel submitData);
    }
    public class TranscriptRequestService : ITranscriptRequestService
    {
        private readonly ITranscriptRequestRepository _transcriptRequestRepo;
        private readonly IInstitutionRepository _institutionRepo;
        private readonly IAvatarService _avatarService;
        private readonly ITimeZoneRepository _timeZoneRepository;

        public TranscriptRequestService(ITranscriptRequestRepository transcriptRequestRepo, IInstitutionRepository institutionRepo, IAvatarService avatarService, ITimeZoneRepository timeZoneRepository)
        {
            _transcriptRequestRepo = transcriptRequestRepo;
            _institutionRepo = institutionRepo;
            _avatarService = avatarService;
            _timeZoneRepository = timeZoneRepository;
        }

        public async Task<IEnumerable<TranscriptRequestResponseModel>> GetTranscriptRequestByPortfolioIdAsync(int portfolioId, int userAccountId)
        {
            var requestsDtos = await _transcriptRequestRepo.GetTranscriptRequestByPortfolioIdAsync(portfolioId);
            
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdByPortfolioIdAsync(portfolioId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            var transcriptRequests = new List<TranscriptRequestResponseModel>();
            foreach (var requestDto in requestsDtos)
            {
                var transcriptRequest = new TranscriptRequestResponseModel {
                    Id = requestDto.Id,
                    InunId = requestDto.InunId,
                    ReceivingInstitutionCode = requestDto.ReceivingInstitutionCode,
                };

                transcriptRequest.LatestHistory = new TranscriptRequestHistoryEvent{
                    IsCreatedByStudent = requestDto.LatestHistory.ModifiedById == userAccountId,
                    Status = (TranscriptRequestStatus)requestDto.LatestHistory.TranscriptStatusId,
                    StatusDate = requestDto.LatestHistory.StatusDateUTC == null ?
                                requestDto.LatestHistory.StatusDateUTC : 
                                DateTimeHelper.GetLocalTime(requestDto.LatestHistory.StatusDateUTC ?? default(DateTime), timeZoneDetail),
                    TranscriptRequestType = (TranscriptRequestType)requestDto.LatestHistory.TranscriptRequestTypeId
                };

                transcriptRequests.Add(transcriptRequest);
            }

            return transcriptRequests;
        }

        public async Task<IEnumerable<TranscriptRequestTimelineResponseModelV2>> GetAllHistoryByPortfolioIdAsync(int portfolioId, int userAccountId, int translationLanguageId, bool includeRequested = true)
        {
            var transcriptRequestTimelineDto = await _transcriptRequestRepo.GetTranscriptRequestProgressByPortfolioIdAsyncV2(portfolioId);
            var savedSchools = await _institutionRepo.SavedSchoolsByPortfolioIdAsync(portfolioId, translationLanguageId);
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdByPortfolioIdAsync(portfolioId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            //Group history events.
            IEnumerable<TranscriptRequestHistoryV2> groupedHistoryEvents = GroupTranscriptRequestHistoryEvents
                (transcriptRequestTimelineDto.TranscriptRequestHistoryList, 
                userAccountId, 
                timeZoneDetail);

            //Group by InunId and ReceivingInstitutionId
            var requestHistories = GetHistoryGroupedByInstitution(groupedHistoryEvents);

            IEnumerable<TranscriptRequestTimelineResponseModelV2> requestsTimelines = JoinHistoryWithInstitution
                (requestHistories,
                transcriptRequestTimelineDto.TranscriptRequestInstitutionList,
                savedSchools);

            return requestsTimelines;
        }
        private IEnumerable<TranscriptRequestHistoryV2> GroupTranscriptRequestHistoryEvents(IEnumerable<TranscriptRequestHistoryModelV2> transcriptRequestHistoryList, int userAccountId, TimeZoneDetailModel timeZoneDetail)
        {
            IEnumerable<TranscriptRequestHistoryV2> histories = transcriptRequestHistoryList
               .GroupBy(
               h => new { h.InunId, h.ReceivingInstitutionCode, h.TranscriptRequestId },
               h => new TranscriptRequestHistoryEvent
               {
                   IsCreatedByStudent = h.ModifiedById == userAccountId,
                   Status = (TranscriptRequestStatus)h.TranscriptStatusId,
                   StatusDate = h.StatusDateUTC == null ? h.StatusDateUTC : DateTimeHelper.GetLocalTime(h.StatusDateUTC ?? default(DateTime), timeZoneDetail),
                   TranscriptRequestType = (TranscriptRequestType)h.TranscriptRequestTypeId
               },
               (key, values) =>
               {
                   return new TranscriptRequestHistoryV2
                   {
                       Id = key.TranscriptRequestId,
                       InunId = key.InunId,
                       ReceivingInstitutionCode = key.ReceivingInstitutionCode,
                       Events = values
                   };
               });

            return histories;
        }

        private IEnumerable<InstitutionTranscriptRequestHistoryModel> GetHistoryGroupedByInstitution(IEnumerable<TranscriptRequestHistoryV2> groupedHistoryEvents)
        {
            return groupedHistoryEvents
                .GroupBy(
                h => new { h.InunId, h.ReceivingInstitutionCode },
                h => h,
                (key, institutionRequestHistories) =>
                {
                    return new InstitutionTranscriptRequestHistoryModel
                    {
                        InunId = key.InunId,
                        ReceivingInstitutionCode = key.ReceivingInstitutionCode,
                        History = institutionRequestHistories
                    };
                });
        }

        private IEnumerable<TranscriptRequestTimelineResponseModelV2> JoinHistoryWithInstitution( 
            IEnumerable<InstitutionTranscriptRequestHistoryModel> requestHistories,
            IEnumerable<TranscriptInstitutionModel> transcriptRequestInstitutionList,
            IEnumerable<TranscriptInstitutionModel> savedSchools)
        {
            IList<TranscriptRequestTimelineResponseModelV2> requestsTimelines = new List<TranscriptRequestTimelineResponseModelV2>();

            foreach (var requestHistory in requestHistories)
            {
                var institution = transcriptRequestInstitutionList
                                    .Where(i => i.InunId == requestHistory.InunId
                                    && i.ReceivingInstitutionCode == requestHistory.ReceivingInstitutionCode
                                    ).FirstOrDefault();
                var requstsTimeline = new TranscriptRequestTimelineResponseModelV2
                {
                    InstitutionCard = new SavedSchool
                    {
                        Institution = institution,
                        IsSavedSchool = savedSchools.Where(i => institution != null && i.InunId == institution.InunId).Count() > 0
                    },
                    History = requestHistory.History
                };

                requestsTimelines.Add(requstsTimeline);
            }

            return requestsTimelines;
        }
        public async Task<IEnumerable<TranscriptRequestTimelineResponseModel>> GetTranscriptRequestsTimelineByPortfolioIdAsync(int portfolioId, int userAccountId, int translationLanguageId, bool includeRequested = true)
        {
            var institutionHistories = await GetAllHistoryByPortfolioIdAsync(portfolioId, userAccountId, translationLanguageId);

            var transcriptRequestTimeline = new List<TranscriptRequestTimelineResponseModel>();
            foreach (var institutionHistory in institutionHistories)
            {
                //history is orderd Ascending, so for latest history get lastone.
                TranscriptRequestHistoryV2 history = !includeRequested ?
                    institutionHistory.History.LastOrDefault(h => h.Events.Count() > 1)
                    : institutionHistory.History.LastOrDefault() ;
                if (history == null)
                    continue;

                var timelineItem = new TranscriptRequestTimelineResponseModel
                {
                    Id = history.Id,
                    InstitutionCard = institutionHistory.InstitutionCard,
                    History = history.Events,
                    AllRequests = institutionHistory.History
                };

                transcriptRequestTimeline.Add(timelineItem);
            }
            return transcriptRequestTimeline;
        }
        public Func<TranscriptRequestStudentViewModel, bool> GetLatestRequestsSearchPredicate(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return null;

            return x => x.StudentName.ToLower().Contains(query.ToLower()) || x.StudentId.ToLower().Contains(query.ToLower());
        }

        public async Task<IEnumerable<TranscriptRequestStudentResponseModel>> GetTranscriptRequestStudentResponseModelAsync(IEnumerable<TranscriptRequestStudentViewModel> transcriptRequestStudentLinqedList, int schoolId)
        {
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdBySchoolIdAsync(schoolId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            var result = new List<TranscriptRequestStudentResponseModel>();
            foreach (var e in transcriptRequestStudentLinqedList)
            {
                result.Add(new TranscriptRequestStudentResponseModel()
                {
                    Id = e.Id,
                    AvatarUrl = string.IsNullOrWhiteSpace(e.AvatarFileName) ? _avatarService.GetStudentAvatarDefaultUrl() : _avatarService.GetStudentAvatarUrl(e),
                    StudentName = e.StudentName,
                    DateOfBirth = e.DateOfBirth,
                    GradeId = e.GradeId,
                    GradeKey = e.GradeKey,
                    StudentId = e.StudentId,
                    TranscriptRequestId = e.TranscriptRequestId,
                    InunId = e.InunId,
                    ReceivingInstitutionCode = e.ReceivingInstitutionCode,
                    ReceivingInstitutionName = e.ReceivingInstitutionName,
                    ReceivingInstitutionCity = e.ReceivingInstitutionCity,
                    ReceivingInstitutionStateCode = e.ReceivingInstitutionStateCode,
                    TranscriptRequestType = e.TranscriptRequestType,
                    TranscriptRequestTypeKey = e.TranscriptRequestTypeKey,
                    RequestedDate = DateTimeHelper.GetLocalTime(e.RequestedDate, timeZoneDetail),
                    ImportedDate = e.ImportedDate == null ? e.ImportedDate : DateTimeHelper.GetLocalTime(e.ImportedDate ?? default(DateTime), timeZoneDetail)
                });
            }
            return result;
        }

        public Func<TranscriptRequestProgressViewModel, bool> GetTranscriptRequestProgressSearchPredicate(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return null;
            return x => x.StudentName.ToLower().Contains(query.ToLower()) || x.StudentId.ToLower().Contains(query.ToLower());
        }

        public async Task<IEnumerable<TranscriptRequestProgressResponseModel>> GetTranscriptRequestProgressResponseModelAsync(IEnumerable<TranscriptRequestProgressViewModel> transcriptRequestProgressLinqedList, int schoolId)
        {
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdBySchoolIdAsync(schoolId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            var result = new List<TranscriptRequestProgressResponseModel>();
            foreach (var e in transcriptRequestProgressLinqedList)
            {
                result.Add(new TranscriptRequestProgressResponseModel()
                {
                    Id = e.Id,
                    AvatarUrl = string.IsNullOrWhiteSpace(e.AvatarFileName) ? _avatarService.GetStudentAvatarDefaultUrl() : _avatarService.GetStudentAvatarUrl(e),
                    StudentName = e.StudentName,
                    DateOfBirth = e.DateOfBirth,
                    GradeId = e.GradeId,
                    GradeKey = e.GradeKey,
                    StudentId = e.StudentId,
                    TranscriptRequestId = e.TranscriptRequestId,
                    InunId = e.InunId,
                    ReceivingInstitutionCode = e.ReceivingInstitutionCode,
                    ReceivingInstitutionName = e.ReceivingInstitutionName,
                    ReceivingInstitutionCity = e.ReceivingInstitutionCity,
                    ReceivingInstitutionStateCode = e.ReceivingInstitutionStateCode,
                    RequestedDate = DateTimeHelper.GetLocalTime(e.RequestedDate, timeZoneDetail),
                    TranscriptStatus = e.TranscriptStatus,
                    TranscriptStatusKey = e.TranscriptStatusKey,
                    TranscriptStatusDate = DateTimeHelper.GetLocalTime(e.TranscriptStatusDate, timeZoneDetail)
                });
            }
            return result;
        }

        public async Task SubmitOutOfNetworkAsync(int id, TranscriptRequestType transcriptRequestType, string receivingInstitutionEmail = null, DateTime? dateSentByMailUtc = null)
        {
            await _transcriptRequestRepo.UpdateTypeAsync(id, (int)transcriptRequestType);
            await _transcriptRequestRepo.AddTranscriptRequestSubmittedAsync(id, receivingInstitutionEmail, dateSentByMailUtc);
        }

        // Check if the TranscriptRequestSentByMail date is between the requested date and today 
        public async Task<bool> IsDateSentByMailInRangeAsync(int id, DateTime dateSentByMailUtc)
        {
            var requestedDateUtc = await _transcriptRequestRepo.GetRequestedDateByIdAsync(id);
            return requestedDateUtc <= dateSentByMailUtc && dateSentByMailUtc <= DateTime.UtcNow;
        }

        public async Task<DateTime> GetDateSentByMailUtcAsync(int id, DateTime dateSentByMail, bool skipValidationForDateSentByMail)
        {
            var dateSentByMailUtc = new DateTime(dateSentByMail.Year, dateSentByMail.Month, dateSentByMail.Day, 12, 0, 0); // FE sends UTC day but not time, set noon as default time so when we convert from utc to localt time we are still on the same day 
            if (skipValidationForDateSentByMail)
                return dateSentByMailUtc;

            var minRangeDateUtc = await _transcriptRequestRepo.GetRequestedDateByIdAsync(id);
            var maxRangeDateUtc = DateTime.UtcNow;

            // Same date as requested date (ignore time sent by FE) => Use requested date + 5ms
            if (minRangeDateUtc.Year == dateSentByMailUtc.Year && minRangeDateUtc.Month == dateSentByMailUtc.Month && minRangeDateUtc.Day == dateSentByMailUtc.Day)
                dateSentByMailUtc = minRangeDateUtc.AddMilliseconds(5);
            // Same date as now date (ignore time sent by FE) => Use now date - 5ms
            else if (maxRangeDateUtc.Year == dateSentByMailUtc.Year && maxRangeDateUtc.Month == dateSentByMailUtc.Month && maxRangeDateUtc.Day == dateSentByMailUtc.Day)
                dateSentByMailUtc = maxRangeDateUtc.AddMilliseconds(-5);

            return dateSentByMailUtc;
        }

        public async Task<bool> IsSendTranscriptRequestInputValidAsync(SendTranscriptViewModel sendTranscriptInfos, TranscriptSubmitRequestModel submitData)
        {
            if (sendTranscriptInfos == null || submitData == null)
                return false;

            // Make sure FE has provided the correct request type
            if (!Enum.IsDefined(typeof(TranscriptRequestType), submitData.TranscriptRequestType))
                return false;

            if (submitData.TranscriptRequestType == TranscriptRequestType.Email && String.IsNullOrWhiteSpace(submitData.ReceivingInstitutionEmail))
                return false;

            var dateSentByMailUtc = await GetDateSentByMailUtcAsync(sendTranscriptInfos.TranscriptRequestId, submitData.DateSentByMail ?? DateTime.Now, submitData.SkipValidationForDateSentByMail);
            if (submitData.TranscriptRequestType == TranscriptRequestType.Mail && !submitData.SkipValidationForDateSentByMail && (submitData.DateSentByMail == null || !await IsDateSentByMailInRangeAsync(sendTranscriptInfos.TranscriptRequestId, dateSentByMailUtc)))
                return false;

            return true;
        }
    }
}
