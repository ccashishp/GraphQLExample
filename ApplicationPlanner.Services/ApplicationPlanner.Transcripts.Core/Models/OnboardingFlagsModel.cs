using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class OnboardingFlagsModel : Dictionary<OnboardingFlagsKeyName, bool>
    {
        public OnboardingFlagsModel(IDictionary<OnboardingFlagsKeyName, bool> dictionary) : base(dictionary)
        { }
    }

    public enum OnboardingFlagsKeyName
    {
        HasSeenCartTooltipForTranscriptsInSavedSchoolsMode,
        HasSeenCartTooltipForTranscriptsInSearchMode
    }
}
