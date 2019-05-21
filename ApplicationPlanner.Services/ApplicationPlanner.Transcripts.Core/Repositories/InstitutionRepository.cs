using ApplicationPlanner.Transcripts.Core.Models;
using CC.Cache;
using CC.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface IInstitutionRepository : IRepository
    {
        Task<IEnumerable<TranscriptInstitutionModel>> SavedSchoolsByPortfolioIdAsync(int portfolioId, int translationLanguageId);
        Task<InstitutionModel> GetDefaultInstitutionByEducatorIdAsync(int educatorId);
    }

    public class InstitutionRepository : Repository, IInstitutionRepository
    {
        private readonly ISql _sql;

        public InstitutionRepository(ISql sql, ICache cache)
          : base(sql, cache)
        {
            _sql = sql;
        }

        public async Task<IEnumerable<TranscriptInstitutionModel>> SavedSchoolsByPortfolioIdAsync(int portfolioId, int translationLanguageId)
        {
            var savedSchools = await _sql.QueryAsync<TranscriptInstitutionModel>(
                sql: "[ApplicationPlanner].[SavedSchoolsGetByPortfolioId]",
                param: new
                {
                    portfolioId,
                    translationLanguageId
                },
                commandType: CommandType.StoredProcedure);
            return savedSchools;
        }

        public async Task<InstitutionModel> GetDefaultInstitutionByEducatorIdAsync(int educatorId)
        {
            var defaultInstitution = await _sql.QueryAsync<InstitutionModel>(
                sql: "[School].[EducatorProfileGetDefaultInstitutionType]",
                param: new
                {
                    educatorId
                },
                commandType: CommandType.StoredProcedure);
            return defaultInstitution.SingleOrDefault();
        }
    }
}
