using ApplicationPlanner.Transcripts.Core.Models;
using System;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptSubmitRequestModel
    {
        public TranscriptRequestType TranscriptRequestType { get; set; }
        public string ReceivingInstitutionEmail { get; set; }
        public DateTime? DateSentByMail { get; set; }
        public bool SkipValidationForDateSentByMail { get; set; }
    }
}
