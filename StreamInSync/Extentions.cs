using System;

namespace StreamInSync
{
    public static class Extentions
    {
        public static string ToMinsAndSecsTime(this TimeSpan timespan)
        {
            return $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
        }

        public static string ToMinsAndSecsText(this TimeSpan timespan)
        {
            var text = $"{(int)timespan.TotalMinutes} minutes";
            if (timespan.Seconds > 0)
            {
                text += $"{timespan.Seconds:00}";
            }
            return text;
        }
    }
}