namespace GoogleLocationHistory
{
    public class GoogleLocationHistoryEntry
    {
        public string timestampMs { get; set; }

        public double latitudeE7 { get; set; }

        public double longitudeE7 { get; set; }

        public int accuracy { get; set; }
    }
}