using System;

namespace GoogleLocationHistory
{
    public class CountryInfo
    {
        public CountryInfo()
        {
            MinAccuracy = Int32.MaxValue;
            MaxAccuracy = Int32.MinValue;
        }

        public string Country { get; set; }

        public string CountryIsoCode { get; set; }

        public DateTime FirstDateUtc { get; set; }

        public DateTime LastDateUtc { get; set; }

        public double Days
        {
            get
            {
                if (LastDateUtc == FirstDateUtc)
                {
                    return 0;
                }

                return (LastDateUtc.Date - FirstDateUtc.Date).TotalDays + 1;
            }
        }

        public int MinAccuracy { get; set; }

        public int MaxAccuracy { get; set; }
    }
}