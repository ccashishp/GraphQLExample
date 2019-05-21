namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class SendTranscriptViewModel
    {
        public int TranscriptRequestId { get; set; }
        public int TranscriptRequestTypeId { get; set; }
        public string StudentId { get; set; }
        public int TranscriptId { get; set; }
        public int SchoolId { get; set; }
        public string ReceivingInstitutionCode { get; set; }
        public string ReceivingInstitutionName { get; set; }
    }
}