using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Web.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Schema
{
    public class TranscriptResponseModelType: ObjectGraphType<TranscriptRequestResponseModel>
    {
        public TranscriptResponseModelType()
        {
            Field(o => o.Id);
            Field(o => o.InunId);
            Field(o => o.ReceivingInstitutionCode);
            Field<TranscriptRequestHistoryEventType>("latestHistory",resolve: context => { return context.Source.LatestHistory; });
        }
    }

    public class TranscriptRequestHistoryEventType: ObjectGraphType<TranscriptRequestHistoryEvent>
    {
        public TranscriptRequestHistoryEventType()
        {
            Field<TranscriptRequestStatusEnum>("status");
            Field(o => o.StatusDate, true);
            Field(o => o.IsCreatedByStudent);
            Field<TranscriptRequestTypeEnum>("TranscriptRequestType");

        }
    }

    public class TranscriptRequestStatusEnum : EnumerationGraphType<TranscriptRequestStatus>
    {
    }

    public class TranscriptRequestTypeEnum : EnumerationGraphType<TranscriptRequestType>
    {
    }

}
