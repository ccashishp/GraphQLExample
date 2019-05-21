using ApplicationPlanner.Transcripts.Core.Models;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class SavedSchool
    {
        public SavedSchool()
        {
            Institution = new TranscriptInstitutionModel();
        }
        public TranscriptInstitutionModel Institution { get; set; }
        public bool IsSavedSchool { get; set; }
    }
}
