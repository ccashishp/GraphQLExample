using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptRequestResponseModel
    {
        public int Id { get; set; }
        public string InunId { get; set; }
        public int ReceivingInstitutionCode { get; set; }
        public TranscriptRequestHistoryEvent LatestHistory { get; set; }
    }
}
