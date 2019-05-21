using ApplicationPlanner.Transcripts.Core.Models;
using CC.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface IOnboardingFlagsRepository : IRepository
    {
        /// <summary>
        /// Get Flags for Student
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> GetByPortfolioIdAsync(int portfolioId);

        /// <summary>
        /// Get Flags for Educator
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        Task<IEnumerable<EducatorHelpOverlayModel>> GetByEducatorIdAsync(int educatorId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSavedSchoolsMode flag to true for a student
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(int portfolioId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSavedSchoolsMode flag to true for an educator
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        Task<int> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(int educatorId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSearchMode flag to true for a student
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(int portfolioId);

        /// <summary>
        /// Set HasSeenCartTooltipForTranscriptsInSearchMode flag to true for an educator
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        Task<int> SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(int educatorId);
    }

    public class OnboardingFlagsRepository : Repository, IOnboardingFlagsRepository
    {
        private readonly ISql _sql;

        public OnboardingFlagsRepository(ISql sql): base(sql)
        {
            _sql = sql;
        }

        public async Task<GlobalSettingModel> GetByPortfolioIdAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Student].[GlobalSettingGet]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<EducatorHelpOverlayModel>> GetByEducatorIdAsync(int educatorId)
        {
            return await _sql.QueryAsync<EducatorHelpOverlayModel>("[School].[EducatorHelpOverlayGetByEducatorId]",
                                   new { educatorId },
                                   commandType: CommandType.StoredProcedure);
        }

        public async Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Student].[GlobalSettingHasSeenCartTooltipForTranscriptsInSavedSchoolsModeSave]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<int> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(int educatorId)
        {
            var result = await _sql.QueryAsync<int>("[School].[EducatorHelpOverlayUpdateInsert]",
                                   new { educatorId, KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString(), Displayed = true },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<GlobalSettingModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<GlobalSettingModel>("[Student].[GlobalSettingHasSeenCartTooltipForTranscriptsInSearchModeSave]",
                                   new { portfolioId },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<int> SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(int educatorId)
        {
            var result = await _sql.QueryAsync<int>("[School].[EducatorHelpOverlayUpdateInsert]",
                                   new { educatorId, KeyName = OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString(), Displayed = true },
                                   commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }
    }
}
