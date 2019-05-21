using ApplicationPlanner.Transcripts.Core.Models;
using System;
namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptRequestProgressResponseModel : StudentResponseModel
    {
        public int TranscriptRequestId { get; set; }
        public string InunId { get; set; }
        public int ReceivingInstitutionCode { get; set; }
        public string ReceivingInstitutionName { get; set; }
        public string ReceivingInstitutionCity { get; set; }
        public string ReceivingInstitutionStateCode { get; set; }
        public DateTime RequestedDate { get; set; }
        public TranscriptRequestStatus TranscriptStatus { get; set; }
        public string TranscriptStatusKey { get; set; }
        public DateTime TranscriptStatusDate { get; set; }
    }
}