using System;

namespace GoogleLocationHistory
{
    public static class DateTimeHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixEpochTime(long unixEpochTime)
        {
            return UnixEpoch.AddMilliseconds(unixEpochTime);
        }
    }
}