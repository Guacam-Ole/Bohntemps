namespace BohnTemps.BeansApi
{
    public static class Helpers
    {
        public static DateTime GetTodayUtc()
        {
            var now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day);
        }

        public static long GetTimestamp(this DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

     
    }
}