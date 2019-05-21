using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models.Exceptions
{
    public class CredentialsReceiverListFetchException : Exception
    {
        public CredentialsReceiverListFetchException(string message) : base(message)
        {
        }
    }
}
