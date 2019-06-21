using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GoogleLocationHistory
{
    public class GoogleLocationHistoryRepository
    {
        public static IEnumerable<GoogleLocationHistoryEntry> Load(string googleTakeoutFilePath)
        {
            using (FileStream fileStream = new FileStream(googleTakeoutFilePath, FileMode.Open))
            using (ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
            using (Stream zipEntryStream = zipArchive.GetEntry(@"Takeout/Location History/Location History.json").Open())
            using (StreamReader streamReader = new StreamReader(zipEntryStream))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                var locationsObjectPath = new Regex(@"^locations\[\d+\]$");
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject && locationsObjectPath.IsMatch(reader.Path))
                    {
                        var entry = serializer.Deserialize<GoogleLocationHistoryEntry>(reader);
                        yield return entry;
                    }
                }
            }
        }
    }
}