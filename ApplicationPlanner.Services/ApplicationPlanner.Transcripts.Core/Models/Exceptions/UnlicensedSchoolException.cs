using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models.Exceptions
{
    public class UnlicensedSchoolException : Exception 
    {
        public UnlicensedSchoolException(string message, string action, int schoolId) : base(message)
        {
            base.Data.Add("Action", action);
            base.Data.Add("SchoolId", schoolId);
        }
    }
}
