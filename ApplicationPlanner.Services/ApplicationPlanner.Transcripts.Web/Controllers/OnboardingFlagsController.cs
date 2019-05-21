using Microsoft.AspNetCore.Mvc;
using ApplicationPlanner.Transcripts.Web.Services;
using CC3.AuthServices.Token.Entities;
using System.Threading.Tasks;
using CC.Common.Enum;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [Route("transcripts/onboarding-flags")]
    public class OnboardingFlagsController: BaseController
    {
        private readonly IOnboardingFlagsService _onboardingFlagsService;

        public OnboardingFlagsController(IOnboardingFlagsService onboardingFlagsService)
        {
            _onboardingFlagsService = onboardingFlagsService;
        }

        /// <summary>
        /// Get all one-time flags and their values for the current user
        /// </summary>
        /// <returns></returns>
        // GET api/transcripts/onboarding-flags
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userType = GetClaim<string>(CcClaimType.UserType);
            if (userType == UserType.Student.ToString())
            {
                var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
                var result = await _onboardingFlagsService.GetByPortfolioIdAsync(portfolioId);
                return Ok(result);
            }
            if (userType == UserType.Educator.ToString())
            {
                var educatorId = GetClaim<int>(CcClaimType.EducatorId);
                var result = await _onboardingFlagsService.GetByEducatorIdAsync(educatorId);
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode
        /// </summary>
        /// <returns></returns>
        // POST api/transcripts/onboarding-flags/seen-cart-tt-saved-schools
        [Route("seen-cart-tt-saved-schools")]
        [HttpPost]
        public async Task<IActionResult> SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsMode()
        {
            var userType = GetClaim<string>(CcClaimType.UserType);
            if(userType == UserType.Student.ToString())
            {
                var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
                var result = await _onboardingFlagsService.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(portfolioId);
                return Ok(result);
            }
            if (userType == UserType.Educator.ToString())
            {
                var educatorId = GetClaim<int>(CcClaimType.EducatorId);
                var result = await _onboardingFlagsService.SaveHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(educatorId);
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// SaveHasSeenCartTooltipForTranscriptsInSearchModeMode
        /// </summary>
        /// <returns></returns>
        // POST api/transcripts/onboarding-flags/seen-cart-tt-search
        [Route("seen-cart-tt-search")]
        [HttpPost]
        public async Task<IActionResult> SaveHasSeenCartTooltipForTranscriptsInSearchMode()
        {
            var userType = GetClaim<string>(CcClaimType.UserType);
            if (userType == UserType.Student.ToString())
            {
                var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
                var result = await _onboardingFlagsService.SaveHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(portfolioId);
                return Ok(result);
            }
            if (userType == UserType.Educator.ToString())
            {
                var educatorId = GetClaim<int>(CcClaimType.EducatorId);
                var result = await _onboardingFlagsService.SaveHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(educatorId);
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
