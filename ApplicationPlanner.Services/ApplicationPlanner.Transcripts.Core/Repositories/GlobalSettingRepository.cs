using ApplicationPlanner.Transcripts.Core.Models;
using CC.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface IGlobalSettingRepository : IRepository
    {
        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSavedSchoolsMode flag to true
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeAsync(int portfolioId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSearchMode flag to true
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeAsync(int portfolioId);
    }

    public class GlobalSettingRepository : Repository, IGlobalSettingRepository
    {
        private readonly ISql _sql;

        public GlobalSettingRepository(ISql sql): base(sql)
        {
            _sql = sql;
        }

        public async Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Student].[GlobalSettingHasSeenCartTooltipForTranscriptsInSavedSchoolsModeSave]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Student].[GlobalSettingHasSeenCartTooltipForTranscriptsInSearchModeSave]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }
    }
}
