using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models.Exceptions
{
    public class TranscriptProviderTranscriptDeleteException : Exception 
    {
        public TranscriptProviderTranscriptDeleteException(string message, int schoolId, string studentId, int transcriptId) : base(message)
        {
            base.Data.Add("School Id", schoolId);
            base.Data.Add("Student Id", studentId);
            base.Data.Add("Transcript Id", transcriptId);
        }
    }
}
