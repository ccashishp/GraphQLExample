using CC.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    public interface IAvatarDetail
    {
        int UserAccountId { get; set; }
        string AvatarFileName { get; set; }
        CountryType SchoolCountryType { get; set; }
    }
}
