using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Helpers;
using ApplicationPlanner.Transcripts.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface ITranscriptService
    {
        Func<TranscriptImportedViewModel, bool> GetTranscriptImportedSearchPredicate(string query);
        Task<IEnumerable<TranscriptImportedResponseModel>> GetTranscriptImportedResponseModelAsync(IEnumerable<TranscriptImportedViewModel> transcriptImportedLinqedList, int schoolId);
        Func<TranscriptBaseModel, bool> GetTranscriptUnmatchedSearchPredicate(string query);
        Func<StudentTranscriptViewModel, bool> GetStudentTranscriptSearchPredicate(string query);
        Task<IEnumerable<StudentTranscriptResponseModel>> GetStudentTranscriptResponseModelAsync(IEnumerable<StudentTranscriptViewModel> studentTranscriptLinqedList);
        Task<StudentTranscriptResponseModel> GetStudentTranscriptResponseModelAsync(StudentTranscriptViewModel studentTranscript, int portfolioId);
        Task<IEnumerable<TranscriptBaseModel>> GetTranscriptUnmatchedResponseModelAsync(IEnumerable<TranscriptBaseModel> transcriptUnmatchedLinqedList, int schoolId);
        Task<TranscriptModel> GetTranscriptResponseModelAsync(TranscriptModel transcript, int schoolId);
    }
    public class TranscriptService : ITranscriptService
    {
        private readonly IAvatarService _avatarService;
        private readonly ITimeZoneRepository _timeZoneRepository;

        public TranscriptService(IAvatarService avatarService, ITimeZoneRepository timeZoneRepository)
        {
            _avatarService = avatarService;
            _timeZoneRepository = timeZoneRepository;
        }

        public Func<TranscriptImportedViewModel, bool> GetTranscriptImportedSearchPredicate(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return null;

            return x => x.StudentName.ToLower().Contains(query.ToLower()) || x.StudentId.ToLower().Contains(query.ToLower());
        }

        public async Task<IEnumerable<TranscriptImportedResponseModel>> GetTranscriptImportedResponseModelAsync(IEnumerable<TranscriptImportedViewModel> transcriptImportedLinqedList, int schoolId)
        {
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdBySchoolIdAsync(schoolId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            var result = new List<TranscriptImportedResponseModel>();
            foreach (var e in transcriptImportedLinqedList)
            {
                result.Add(new TranscriptImportedResponseModel()
                {
                    Id = e.Id,
                    AvatarUrl = string.IsNullOrWhiteSpace(e.AvatarFileName) ? _avatarService.GetStudentAvatarDefaultUrl() : _avatarService.GetStudentAvatarUrl(e),
                    StudentName = e.StudentName,
                    DateOfBirth = e.DateOfBirth,
                    GradeId = e.GradeId,
                    GradeKey = e.GradeKey,
                    StudentId = e.StudentId,
                    TranscriptId = e.TranscriptId,
                    ImportedDate = DateTimeHelper.GetLocalTime(e.ImportedDate, timeZoneDetail)
                });
            }
            return result;
        }

        public Func<TranscriptBaseModel, bool> GetTranscriptUnmatchedSearchPredicate(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return null;

            return x => x.StudentName.ToLower().Contains(query.ToLower()) || x.StudentNumber.ToLower().Contains(query.ToLower());
        }

        public Func<StudentTranscriptViewModel, bool> GetStudentTranscriptSearchPredicate(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return null;

            return x => x.StudentName.ToLower().Contains(query.ToLower()) || x.StudentId.ToLower().Contains(query.ToLower());
        }

        public async Task<IEnumerable<StudentTranscriptResponseModel>> GetStudentTranscriptResponseModelAsync(IEnumerable<StudentTranscriptViewModel> studentTranscriptLinqedList)
        {
            var result = new List<StudentTranscriptResponseModel>();
            foreach (var e in studentTranscriptLinqedList)
            {
                result.Add(await GetStudentTranscriptResponseModelAsync(e, e.Id));
            }
            return result;
        }

        public async Task<StudentTranscriptResponseModel> GetStudentTranscriptResponseModelAsync(StudentTranscriptViewModel studentTranscript, int portfolioId)
        {
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdByPortfolioIdAsync(portfolioId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            return new StudentTranscriptResponseModel()
            {
                Id = studentTranscript.Id,
                AvatarUrl = string.IsNullOrWhiteSpace(studentTranscript.AvatarFileName) ? _avatarService.GetStudentAvatarDefaultUrl() : _avatarService.GetStudentAvatarUrl(studentTranscript),
                StudentName = studentTranscript.StudentName,
                DateOfBirth = studentTranscript.DateOfBirth,
                GradeId = studentTranscript.GradeId,
                GradeKey = studentTranscript.GradeKey,
                StudentId = studentTranscript.StudentId,
                TranscriptId = studentTranscript.TranscriptId,
                ReceivedDateUtc = studentTranscript.ReceivedDateUtc == null 
                    ? studentTranscript.ReceivedDateUtc 
                    : DateTimeHelper.GetLocalTime(studentTranscript.ReceivedDateUtc ?? default(DateTime), timeZoneDetail)
            };
        }

        public async Task<IEnumerable<TranscriptBaseModel>> GetTranscriptUnmatchedResponseModelAsync(IEnumerable<TranscriptBaseModel> transcriptUnmatchedLinqedList, int schoolId)
        {
            var timeZoneId = await _timeZoneRepository.GeTimeZoneIdBySchoolIdAsync(schoolId);
            var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

            var result = new List<TranscriptBaseModel>();
            foreach (var e in transcriptUnmatchedLinqedList)
            {
                e.ReceivedDateUtc = DateTimeHelper.GetLocalTime(e.ReceivedDateUtc, timeZoneDetail);
                e.DateOfBirth = (DateTime.TryParse(e.DateOfBirth, out DateTime dateValue)) ? e.DateOfBirth : null;
                result.Add(e);
            }
            return result;
        }

        public async Task<TranscriptModel> GetTranscriptResponseModelAsync(TranscriptModel transcript, int schoolId)
        {
            if (transcript != null)
            {
                var timeZoneId = await _timeZoneRepository.GeTimeZoneIdBySchoolIdAsync(schoolId);
                var timeZoneDetail = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timeZoneId);

                transcript.DateOfBirth = (DateTime.TryParse(transcript.DateOfBirth, out DateTime dateValue)) ? transcript.DateOfBirth : null;
                transcript.ReceivedDateUtc = DateTimeHelper.GetLocalTime(transcript.ReceivedDateUtc, timeZoneDetail);
                transcript.LinkApprovedDateUTC = transcript.LinkApprovedDateUTC == null
                    ? transcript.LinkApprovedDateUTC
                    : DateTimeHelper.GetLocalTime(transcript.LinkApprovedDateUTC ?? default(DateTime), timeZoneDetail);
            }
            return transcript;
        }
    }
}
