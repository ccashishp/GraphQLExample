using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface IOnboardingFlagsService
    {
        Task<OnboardingFlagsModel> GetByPortfolioIdAsync(int portfolioId);
        Task<OnboardingFlagsModel> GetByEducatorIdAsync(int educatorId);
        Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(int portfolioId);
        Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(int educatorId);
        Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(int portfolioId);
        Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(int educatorId);
    }
    public class OnboardingFlagsService : IOnboardingFlagsService
    {
        private readonly IOnboardingFlagsRepository _onboardingFlagsRepository;
        public readonly Dictionary<OnboardingFlagsKeyName, bool> _defaultOnboardingFlags = new Dictionary<OnboardingFlagsKeyName, bool>()
        {
            { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, false },
            { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, false }
        };

        public OnboardingFlagsService(IOnboardingFlagsRepository onboardingFlagsRepository)
        {
            _onboardingFlagsRepository = onboardingFlagsRepository;
        }

        public async Task<OnboardingFlagsModel> GetByPortfolioIdAsync(int portfolioId)
        {
            var result = await _onboardingFlagsRepository.GetByPortfolioIdAsync(portfolioId);
            return result == null ? new OnboardingFlagsModel(_defaultOnboardingFlags) :
                new OnboardingFlagsModel(
                    new Dictionary<OnboardingFlagsKeyName, bool>()
                    {
                        { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, result.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode },
                        { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, result.HasSeenCartTooltipForTranscriptsInSearchMode }
                    }
               );
        }

        public async Task<OnboardingFlagsModel> GetByEducatorIdAsync(int educatorId)
        {
            var result = await _onboardingFlagsRepository.GetByEducatorIdAsync(educatorId);
            var hasSeenCartTooltipForTranscriptsInSavedSchoolsMode = result.FirstOrDefault(x => x.KeyName == OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode.ToString());
            var hasSeenCartTooltipForTranscriptsInSearchMode = result.FirstOrDefault(x => x.KeyName == OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode.ToString());
            return result == null ? new OnboardingFlagsModel(_defaultOnboardingFlags) : 
                new OnboardingFlagsModel(
                    new Dictionary<OnboardingFlagsKeyName, bool>()
                    {
                        { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode, hasSeenCartTooltipForTranscriptsInSavedSchoolsMode == null ? false : hasSeenCartTooltipForTranscriptsInSavedSchoolsMode.Displayed },
                        { OnboardingFlagsKeyName.HasSeenCartTooltipForTranscriptsInSearchMode, hasSeenCartTooltipForTranscriptsInSearchMode == null ? false : hasSeenCartTooltipForTranscriptsInSearchMode.Displayed }
                    }
               );
        }

        public async Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(int portfolioId)
        {
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(portfolioId);
            return result == null ? new OnboardingFlagsModel(_defaultOnboardingFlags) : await GetByPortfolioIdAsync(portfolioId);
        }

        public async Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(int educatorId)
        {
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(educatorId);
            if (result > 0)
                return await GetByEducatorIdAsync(educatorId);
            else
                return new OnboardingFlagsModel(_defaultOnboardingFlags);
        }

        public async Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(int portfolioId)
        {
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(portfolioId);
            return result == null ? new OnboardingFlagsModel(_defaultOnboardingFlags) : await GetByPortfolioIdAsync(portfolioId);
        }

        public async Task<OnboardingFlagsModel> SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(int educatorId)
        {
            var result = await _onboardingFlagsRepository.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(educatorId);
            if (result > 0)
                return await GetByEducatorIdAsync(educatorId);
            else
                return new OnboardingFlagsModel(_defaultOnboardingFlags);
        }
    }
}
