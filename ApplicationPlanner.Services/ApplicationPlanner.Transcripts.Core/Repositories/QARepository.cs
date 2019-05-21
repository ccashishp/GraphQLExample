using ApplicationPlanner.Transcripts.Core.Models;
using CC.Data;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface IQARepository : IRepository
    {
        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSavedSchoolsMode flag to false for a specific student
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(int portfolioId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSavedSchoolsMode flag to false for a specific educator
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        Task<int> ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(int educatorId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSearchMode flag to false for a specif student
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> ResetHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(int portfolioId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSearchMode flag to false for a specif educator
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        Task<int> ResetHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(int educatorId);

        /// <summary>
        /// Delete all the transcripts requets for a specifi student
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task DeleteTranscriptRequestAsync(int portfolioId);

        /// <summary>
        /// Update the status date of a specifi transcript request and specific status
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task UpdateStatusDateUtcAsync(int id, TranscriptRequestStatus transcriptStatusId, DateTime statusDateUTC);

        /// <summary>
        /// Simulate adding a transcript in our db after the Credentails has parsed the pdf and call our webservice to add transcript
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <param name="schoolId"></param>
        /// <param name="studentNumber"></param>
        /// <param name="studentName"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        Task ImportTranscriptAsync(int portfolioId, int schoolId, string studentNumber, string studentName, string dateOfBirth, string emailAddress);

        /// <summary>
        /// Simulate updating a transcript in our db after the Credentials has parsed the pdf and call our webservice to update transcript
        /// </summary>
        /// <param name="transcriptId"></param>
        /// <returns></returns>
        Task UpdateTranscriptAsync(int portranscriptIdtfolioId);

        /// <summary>
        /// Returns student infos by portfolioId
        /// </summary>
        /// <param name="id">portfolioId</param>
        /// <returns></returns>
        Task<StudentModel> GetStudentByPortfolioIdAsync(int id);

        /// <summary>
        /// Simulate receiving a transcript in our db after the Credentails has submitted it and the receiving institution has acknowledged receiving it and call our webservice to update
        /// </summary>
        /// <param name="transcriptRequestId"></param>
        /// <param name="modifiedById"></param>
        /// <returns></returns>
        Task ReceivedTranscriptAsync(int transcriptRequestId, int modifiedById);

        /// <summary>
        /// Simulate expiring sent transcript in our db after the Credentails has sent it and the receiving institution hasn't open it in time and call our webservice to update
        /// </summary>
        /// <param name="transcriptRequestId"></param>
        /// <param name="modifiedById"></param>
        /// <returns></returns>
        Task ExpiredTranscriptAsync(int transcriptRequestId, int modifiedById);
    }

    public class QARepository : Repository, IQARepository
    {
        private readonly ISql _sql;

        public QARepository(ISql sql) : base(sql)
        {
            _sql = sql;
        }

        public async Task<GlobalSettingModel> ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Utility].[GlobalSettingHasSeenCartTooltipForTranscriptsInSavedSchoolsModeReset]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<int> ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(int educatorId)
        {
            var result = await _sql.QueryAsync<int>("[School].[EducatorHelpOverlayUpdateInsert]",
                                   new { educatorId,
                                       KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString(),
                                       Displayed = false
                                   },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<GlobalSettingModel> ResetHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Utility].[GlobalSettingHasSeenCartTooltipForTranscriptsInSearchModeReset]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<int> ResetHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(int educatorId)
        {
            var result = await _sql.QueryAsync<int>("[School].[EducatorHelpOverlayUpdateInsert]",
                                   new
                                   {
                                       educatorId,
                                       KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString(),
                                       Displayed = false
                                   },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task DeleteTranscriptRequestAsync(int portfolioId)
        {
            await _sql.ExecuteAsync(
                sql: "[Utility].[TranscriptRequestDelete]",
                param: new
                {
                    portfolioId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateStatusDateUtcAsync(int id, TranscriptRequestStatus transcriptStatusId, DateTime statusDateUtc)
        {
            await _sql.ExecuteAsync(
                sql: "[Utility].[TranscriptRequestHistoryStatusDateUTCUpdate]",
                param: new
                {
                    TranscriptRequestId = id,
                    TranscriptStatusId = (int)transcriptStatusId,
                    StatusDateUTC = statusDateUtc
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task ImportTranscriptAsync(int portfolioId, int schoolId, string studentNumber, string studentName, string dateOfBirth, string emailAddress)
        {
            var result = _sql.Query<int?>("SELECT MAX(TranscriptId) FROM ApplicationPlanner.Transcript").FirstOrDefault();
            var transcriptId = result == null ? 1000000 : result + 1;

            await _sql.ExecuteAsync(
                sql: "[ApplicationPlanner].[TranscriptInsert]",
                param: new
                {
                    TranscriptId = transcriptId,
                    SchoolId = schoolId,
                    StudentNumber = studentNumber,
                    StudentName = studentName,
                    DateOfBirth = dateOfBirth,
                    EmailAddress = emailAddress
                },
                commandType: CommandType.StoredProcedure);

            await _sql.ExecuteAsync(
                sql: "[ApplicationPlanner].[TranscriptLogInsert]",
                param: new
                {
                    TranscriptId = transcriptId,
                    SchoolId = schoolId,
                    StudentNumber = studentNumber,
                    StudentName = studentName,
                    DateOfBirth = dateOfBirth,
                    EmailAddress = emailAddress
                },
                commandType: CommandType.StoredProcedure);

            if(portfolioId > 0)
            {
                await _sql.ExecuteAsync(
                sql: "[ApplicationPlanner].[MatchTranscript]",
                param: new
                {
                    PortfolioId = portfolioId,
                    TranscriptId = transcriptId,
                    IsAutoLink = 1
                },
                commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateTranscriptAsync(int transcriptId)
        {
            var existingTranscript = (await _sql.QueryAsync<TranscriptModel>(
                sql: "[ApplicationPlanner].[TranscriptGetById]",
                param: new { transcriptId },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        
            await _sql.ExecuteAsync(
                sql: "[ApplicationPlanner].[TranscriptUpdate]",
                param: new
                {
                    transcriptId,
                    existingTranscript.SchoolId,
                    existingTranscript.StudentNumber,
                    existingTranscript.StudentName,
                    existingTranscript.DateOfBirth,
                    existingTranscript.EmailAddress
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<StudentModel> GetStudentByPortfolioIdAsync(int id)
        {
            var result = await _sql.QueryAsync<StudentModel>(
                sql: "[Student].[GetByPortfolioId]",
                param: new
                {
                    PortfolioId = id
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task ReceivedTranscriptAsync(int transcriptRequestId, int modifiedById)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestHistoryAppend]",
                param: new
                {
                    transcriptRequestId,
                    statusId = (int)TranscriptRequestStatus.Received,
                    modifiedById
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task ExpiredTranscriptAsync(int transcriptRequestId, int modifiedById)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestHistoryAppend]",
                param: new
                {
                    transcriptRequestId,
                    statusId = (int)TranscriptRequestStatus.Expired,
                    modifiedById
                },
                commandType: CommandType.StoredProcedure);
        }
    }

    public class StudentModel
    {
        public int PortfolioId { get; set; }
        public int SchoolId { get; set; }
    }
    public class ImportTranscriptModel
    {
        public int? PortfolioId { get; set; }
        public int SchoolId { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
    }
}
