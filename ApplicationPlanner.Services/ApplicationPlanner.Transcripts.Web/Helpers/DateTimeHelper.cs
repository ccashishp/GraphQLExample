using ApplicationPlanner.Transcripts.Core.Models;
using System;

namespace ApplicationPlanner.Transcripts.Web.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime GetLocalTime(DateTime utcDateTime, TimeZoneDetailModel timeZoneDetail)
        {
            TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneDetail.SQLKey);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, destinationTimeZone);
        }

        public static DateTime GetUtcTime(DateTime localDateTime, TimeZoneDetailModel timeZoneDetail)
        {
            DateTime givenDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
            TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneDetail.SQLKey);
            return TimeZoneInfo.ConvertTimeToUtc(givenDateTime, sourceTimeZone);
        }
    }
}
