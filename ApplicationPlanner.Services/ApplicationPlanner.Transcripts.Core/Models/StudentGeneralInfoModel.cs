using CC.Common.Enum;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class StudentGeneralInfoModel: IAvatarDetail
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public int GradeNumber { get; set; }
        public string FirstName { get; set; }
        public string AvatarFileName { get; set; }
        public CountryType SchoolCountryType { get; set; }
        public bool HasSeenCartTooltipForTranscriptsInSavedSchoolsMode { get; set; }
        public bool HasSeenCartTooltipForTranscriptsInSearchMode { get; set; }
    }
}
