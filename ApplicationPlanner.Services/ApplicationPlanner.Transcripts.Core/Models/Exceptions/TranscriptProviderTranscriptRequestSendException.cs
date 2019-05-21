using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models.Exceptions
{
    public class TranscriptProviderTranscriptRequestSendException : Exception 
    {
        public TranscriptProviderTranscriptRequestSendException(string message, int transcriptRequestId, string studentId, int transcriptId, int schoolId, string receivingInstitutionCode, string receivingInstitutionName, string receivingInstitutionEmail) : base(message)
        {
            Data.Add("Transcript Request Id", transcriptRequestId);
            Data.Add("Student Id", studentId);
            Data.Add("Transcript Id", transcriptId);
            Data.Add("School Id", schoolId);
            Data.Add("Receiving Institution Code", receivingInstitutionCode);
            Data.Add("Receiving Institution Name", receivingInstitutionName);
            Data.Add("Receiving Institution Email", receivingInstitutionEmail);
        }
    }
}
