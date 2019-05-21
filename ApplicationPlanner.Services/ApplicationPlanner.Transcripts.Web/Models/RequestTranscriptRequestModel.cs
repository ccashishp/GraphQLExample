using ApplicationPlanner.Transcripts.Core.Models;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class RequestTranscriptRequestModel
    {
        public string InunId { get; set; }
        public TranscriptRequestType TranscriptRequestTypeId { get; set; }
        public int? ReceivingInstitutionCode { get; set; }
        public string ReceivingInstitutionName { get; set; }
        public string ReceivingInstitutionCity { get; set; }
        public string ReceivingInstitutionStateCode { get; set; }
        public int? PortfolioId { get; set; } // optional as it's used only when educator sends on behalf of a student
    }
}
