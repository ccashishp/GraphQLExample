using ApplicationPlanner.Transcripts.Core.Models;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class AuthenticatedStudentModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string AvatarUrl { get; set; }
        public bool HasAccessToTranscripts { get; set; }
        public GlobalSettingModel globalSetting { get; set; }
    }
}
