using ApplicationPlanner.Transcripts.Core.Models;
using CC.Cache;
using CC.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface ITranscriptRepository : IRepository
    {
        Task<IEnumerable<TranscriptImportedViewModel>> GetTranscriptImportedBySchoolIdAsync(int schoolId);
        Task<IEnumerable<TranscriptBaseModel>> GetTranscriptUnmatchedBySchoolIdAsync(int schoolId);
        Task DeleteByIdAsync(int id);
        Task DeleteUndoByIdAsync(int id);
        Task<IEnumerable<StudentTranscriptViewModel>> GetStudentTranscriptBySchoolIdAsync(int schoolId);
        Task MatchTranscriptToStudentAsync(int transcriptId, int portfolioId, int educatorId);
        Task<StudentTranscriptViewModel> GetStudentTranscriptByPortfolioIdAsync(int portfolioId);
        Task<TranscriptModel> GetByIdAsync(int id);
    }

    public class TranscriptRepository : Repository, ITranscriptRepository
    {
        private readonly ISql _sql;

        public TranscriptRepository(ISql sql, ICache cache)
          : base(sql, cache)
        {
            _sql = sql;
        }

        public async Task<IEnumerable<TranscriptImportedViewModel>> GetTranscriptImportedBySchoolIdAsync(int schoolId)
        {
            return await _sql.QueryAsync<TranscriptImportedViewModel>(
                sql: "[ApplicationPlanner].[TranscriptImportedGetBySchoolId]",
                param: new
                {
                    schoolId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TranscriptBaseModel>> GetTranscriptUnmatchedBySchoolIdAsync(int schoolId)
        {
            return await _sql.QueryAsync<TranscriptBaseModel>(
                sql: "[ApplicationPlanner].[TranscriptUnmatchedGetBySchoolId]",
                param: new
                {
                    schoolId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptDelete]",
                param: new
                {
                    TranscriptId = id
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteUndoByIdAsync(int id)
        {
            await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptDeleteUndo]",
                param: new
                {
                    TranscriptId = id
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StudentTranscriptViewModel>> GetStudentTranscriptBySchoolIdAsync(int schoolId)
        {
            return await _sql.QueryAsync<StudentTranscriptViewModel>(
                sql: "[ApplicationPlanner].[StudentTranscriptGetBySchoolId]",
                param: new
                {
                    schoolId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task MatchTranscriptToStudentAsync(int transcriptId, int portfolioId, int educatorId)
        {
            await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[MatchTranscript]",
                param: new
                {
                    transcriptId,
                    portfolioId,
                    IsAutoLink = false,
                    educatorId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<StudentTranscriptViewModel> GetStudentTranscriptByPortfolioIdAsync(int portfolioId)
        {
            var result =  await _sql.QueryAsync<StudentTranscriptViewModel>(
                sql: "[ApplicationPlanner].[StudentTranscriptGetByPortfolioId]",
                param: new
                {
                    portfolioId
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<TranscriptModel> GetByIdAsync(int id)
        {
            var result = await _sql.QueryAsync<TranscriptModel>(
                sql: "[ApplicationPlanner].[TranscriptGetById]",
                param: new
                {
                    TranscriptId = id
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
    }
}
