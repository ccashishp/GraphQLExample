using ApplicationPlanner.Transcripts.Core.Models;
using System;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptRequestStudentResponseModel: StudentResponseModel
    {
        public int TranscriptRequestId { get; set; }
        public string InunId { get; set; }
        public int ReceivingInstitutionCode { get; set; }
        public string ReceivingInstitutionName { get; set; }
        public string ReceivingInstitutionCity { get; set; }
        public string ReceivingInstitutionStateCode { get; set; }
        public TranscriptRequestType TranscriptRequestType { get; set; }
        public string TranscriptRequestTypeKey { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ImportedDate { get; set; }
    }
}
