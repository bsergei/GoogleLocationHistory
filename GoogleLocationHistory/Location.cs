using System;

namespace GoogleLocationHistory
{
    public class Location
    {
        public DateTime TimestampUtc { get; set; }

        public string CountryIsoCode { get; set; }

        public string Country { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Accuracy { get; set; }
    }
}