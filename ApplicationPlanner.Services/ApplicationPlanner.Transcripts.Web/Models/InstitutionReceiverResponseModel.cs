using System;
using System.Collections.Generic;
namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class InstitutionReceiverResponseModel
    {
        public string InunId { get; set; } // CredentialsAPI.CruzId
        public string Name { get; set; }
        public List<InstitutionReceiverOfficeResponseModel> ReceiverList { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is InstitutionReceiverResponseModel))
                return false;
            else
            {
                InstitutionReceiverResponseModel rm = (InstitutionReceiverResponseModel)obj;
                return InunId == rm.InunId && Name == rm.Name;
            }
        }
        public override int GetHashCode()
        {
            return InunId.GetHashCode();
        }
    }
    public class InstitutionReceiverOfficeResponseModel
    {
        public string Id { get; set; } // CredentialsAPI.EssId
        public string City { get; set; }
        public string State { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is InstitutionReceiverOfficeResponseModel))
                return false;
            else
            {
                InstitutionReceiverOfficeResponseModel rm = (InstitutionReceiverOfficeResponseModel)obj;
                return Id == rm.Id && City == rm.City && State == rm.State;
            }
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}