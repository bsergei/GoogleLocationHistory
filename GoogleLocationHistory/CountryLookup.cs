using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace GoogleLocationHistory
{
    public class CountryLookup
    {
        private static readonly List<(IGeometry g, string country, string countryIsoCode)> CountryGeometries;

        private readonly ThreadLocal<(IGeometry g, string country, string countryIsoCode)?> optimisticCache_;

        static CountryLookup()
        {
            CountryGeometries = GetCountryGeometries();
        }

        public CountryLookup()
        {
            optimisticCache_ = new ThreadLocal<(IGeometry g, string country, string countryIsoCode)?>();
        }

        public (string country, string countryIsoCode)? LookupCountry(double longtitude, double latitude)
        {
            var point = new Point(longtitude, latitude);

            if (optimisticCache_.Value != null)
            {
                var lastValue = optimisticCache_.Value;
                if (lastValue.Value.g.Contains(point))
                {
                    return (lastValue.Value.country, lastValue.Value.countryIsoCode);
                }
            }

            var c2 = CountryGeometries.AsNullable().FirstOrDefault(_ => _.Value.g.Contains(point));
            optimisticCache_.Value = c2;
            if (c2 != null)
            {
                return (c2.Value.country, c2.Value.countryIsoCode);
            }

            return null;
        }

        private static IEnumerable<WorldCountryBoundary> ReadCountriesData()
        {
            using(var sr = File.OpenText("CountryBoundaries.csv"))
            {
                var csv = new CsvReader(sr);
                csv.Configuration.HasHeaderRecord = false;
                return csv.GetRecords<WorldCountryBoundary>().ToArray();
            }
        }

        private static List<(IGeometry g, string country, string countryIsoCode)> GetCountryGeometries()
        {
            var list = new List<(IGeometry g, string country, string countryIsoCode)>();
            var rawData = ReadCountriesData();
            foreach (var rawDataEntry in rawData)
            {
                var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();
                var featureCollection = geoJsonReader.Read<FeatureCollection>(rawDataEntry.Geometry);
                var geometry = featureCollection.Features[0].Geometry;

                if (geometry is GeometryCollection collection)
                {
                    foreach (var ggg in collection.Geometries)
                    {
                        list.Add((ggg, rawDataEntry.Name, rawDataEntry.ISO_2DIGIT));
                    }
                }
                else
                {
                    list.Add((geometry, rawDataEntry.Name, rawDataEntry.ISO_2DIGIT));
                }
            }

            return list;
        }
    }
}