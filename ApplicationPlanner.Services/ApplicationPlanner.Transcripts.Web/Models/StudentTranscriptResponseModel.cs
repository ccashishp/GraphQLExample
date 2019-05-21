using System;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class StudentTranscriptResponseModel : StudentResponseModel
    {
        public int? TranscriptId { get; set; }
        public DateTime? ReceivedDateUtc { get; set; }
    }
}
