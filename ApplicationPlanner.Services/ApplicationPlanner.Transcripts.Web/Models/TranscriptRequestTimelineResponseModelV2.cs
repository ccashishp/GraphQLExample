using ApplicationPlanner.Transcripts.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptRequestTimelineResponseModelV2
    {
        public IEnumerable<TranscriptRequestHistoryV2> History { get; set; }
        public SavedSchool InstitutionCard { get; set; }
        
    }

    public class TranscriptRequestHistoryV2
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string InunId { get; set; }
        [JsonIgnore]
        public int ReceivingInstitutionCode { get; set; }
        public IEnumerable<TranscriptRequestHistoryEvent> Events { get; set; }
    }

    public class TranscriptRequestHistoryEvent
    {
        public bool IsCreatedByStudent { get; set; }
        public TranscriptRequestStatus Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public TranscriptRequestType TranscriptRequestType { get; set; } // AP-609

    }

    public class InstitutionTranscriptRequestHistoryModel
    {
        public string InunId { get; set; }
        public int ReceivingInstitutionCode { get; set; }
        public IEnumerable<TranscriptRequestHistoryV2> History { get; set; }
    }
}
