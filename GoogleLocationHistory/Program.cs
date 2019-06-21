using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleLocationHistory
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 && !File.Exists(args[0]))
            {
                throw new ApplicationException("File name should be specified as an single argument.");
            }

            var arr = GoogleLocationHistoryRepository.Load(args[0])
                .AsParallel()
                .Select(LocationRepository.CreateLocation)
                .Where(_ => _ != null)
                .OrderBy(_ => _.TimestampUtc)
                .ToArray();

            var result = AggregateWithoutZeroDays(AggregateLocations(arr));

            int num = 0;
            foreach (var aggLoc in result)
            {
                Console.WriteLine(FormatCsv(aggLoc, num++));
            }
        }

        private static string FormatCsv(CountryInfo countryInfo, int num)
        {
            return $"{num},{countryInfo.CountryIsoCode ?? ""},{countryInfo.Country ?? ""},{Math.Ceiling(countryInfo.Days)},{countryInfo.FirstDateUtc:O},{countryInfo.LastDateUtc:O},{countryInfo.MinAccuracy},{countryInfo.MaxAccuracy}";
        }

        private static IEnumerable<CountryInfo> AggregateWithoutZeroDays(IEnumerable<CountryInfo> arr)
        {
            CountryInfo prev = null;
            foreach (var item in arr.Where(_ => _.Days > 0))
            {
                if (prev == null)
                {
                    prev = item;
                    continue;
                }

                if (item.Country == prev.Country)
                {
                    prev = new CountryInfo
                    {
                        Country = item.Country,
                        CountryIsoCode = item.CountryIsoCode,
                        FirstDateUtc = prev.FirstDateUtc,
                        LastDateUtc = item.LastDateUtc,
                        MaxAccuracy = Math.Max(Math.Max(0, item.MaxAccuracy), Math.Max(0, prev.MaxAccuracy)),
                        MinAccuracy = Math.Min(Math.Max(0, item.MinAccuracy), Math.Max(0, prev.MinAccuracy)),
                    };
                }
                else
                {
                    yield return prev;
                    prev = item;
                }
            }

            if (prev != null)
            {
                yield return prev;
            }
        }

        private static IEnumerable<CountryInfo> AggregateLocations(IEnumerable<Location> arr)
        {
            CountryInfo curr = null;
            foreach (Location item in arr)
            {
                if (item.Accuracy > 1700 || item.Country == null)
                {
                    continue;
                }

                if (curr != null && curr.Country == item.Country)
                {
                    curr.LastDateUtc = item.TimestampUtc;
                }
                else
                {
                    if (curr != null)
                    {
                        yield return curr;
                    }

                    curr = new CountryInfo
                    {
                        FirstDateUtc = item.TimestampUtc,
                        LastDateUtc = item.TimestampUtc,
                        Country = item.Country,
                        CountryIsoCode = item.CountryIsoCode
                    };
                }

                if (item.Accuracy > curr.MaxAccuracy)
                {
                    curr.MaxAccuracy = Math.Max(0, item.Accuracy);
                }

                if (item.Accuracy < curr.MinAccuracy)
                {
                    curr.MinAccuracy = Math.Max(0, item.Accuracy);
                }
            }

            if (curr != null)
            {
                yield return curr;
            }
        }
    }
}
