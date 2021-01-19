using System;

namespace InvestIn.Finance.Services
{
    public static class DateTimeExtensions
    {
        public static long MillisecondsTimestamp(this DateTime date)
        {
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(date.ToUniversalTime()-baseDate).TotalMilliseconds;
        }
    }
}