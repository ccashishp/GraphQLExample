using System;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptImportedResponseModel : StudentResponseModel
    {
        public int TranscriptId { get; set; }
        public DateTime ImportedDate { get; set; }
    }
}
