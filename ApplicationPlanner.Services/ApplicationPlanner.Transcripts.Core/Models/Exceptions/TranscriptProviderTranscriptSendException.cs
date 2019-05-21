using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models.Exceptions
{
    public class TranscriptProviderTranscriptSendException : Exception 
    {
        public TranscriptProviderTranscriptSendException(string message, int schoolId, string fileType, int bytes) : base(message)
        {
            base.Data.Add("School Id", schoolId);
            base.Data.Add("File Type", fileType);
            base.Data.Add("File Size (Bytes)", bytes);
        }
    }
}
