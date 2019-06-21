using System;

namespace GoogleLocationHistory
{
    public class LocationRepository
    {
        private static readonly CountryLookup countryLookup_;

        static LocationRepository()
        {
            countryLookup_ = new CountryLookup();
        }

        public static Location CreateLocation(GoogleLocationHistoryEntry locationHistoryEntry)
        {
            if (!Int64.TryParse(locationHistoryEntry.timestampMs, out var ts))
            {
                return null;
            }

            var timestampUtc = DateTimeHelper.FromUnixEpochTime(ts);
            var longtitude = locationHistoryEntry.longitudeE7 / 10000000.0;
            var latitude = locationHistoryEntry.latitudeE7 / 10000000.0;

            var c = countryLookup_.LookupCountry(longtitude, latitude);

            var location = new Location()
            {
                TimestampUtc = timestampUtc,
                Longitude = longtitude,
                Latitude = latitude,
                Accuracy = locationHistoryEntry.accuracy,
                Country = c?.country,
                CountryIsoCode = c?.countryIsoCode
            };
            return location;
        }
    }
}