namespace ApplicationPlanner.Transcripts.Core.Models
{
    // ApplicationPlanner.SchoolSetting Table
    public class SchoolSettingModel
    {
        public int SchoolSettingId { get; set; }
        public int SchoolId { get; set; }
        public string TranscriptProviderId { get; set; } // Id from the Credentials to be used within any communication via API with Credentials
        public bool IsTranscriptEnabled { get; set; }
    }
}
