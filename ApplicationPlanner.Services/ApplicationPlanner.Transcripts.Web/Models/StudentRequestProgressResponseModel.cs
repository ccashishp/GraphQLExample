using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class StudentRequestProgressResponseModel
    {
        public StudentTranscriptResponseModel Student { get; set; }
        public IEnumerable<TranscriptRequestTimelineResponseModel> Progress { get; set; }
    }
}
