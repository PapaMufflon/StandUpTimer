using System;

namespace StandUpTimer.ViewModels
{
    internal static class TimeExtensions
    {
        public static string FormatRemainingTime(this TimeSpan remainingTime)
        {
            if (remainingTime <= TimeSpan.Zero)
                return string.Empty;

            if (remainingTime < TimeSpan.FromMinutes(1))
                return string.Format("{0:0}\nsec", remainingTime.TotalSeconds);

            return string.Format("{0:0}\nmin", remainingTime.TotalMinutes);
        }

        public static double PercentageTo(this TimeSpan remainingTime, TimeSpan totalTime)
        {
            return remainingTime > TimeSpan.Zero
                       ? remainingTime.TotalSeconds / totalTime.TotalSeconds * 100.0
                       : 0.0;
        }

        public static double FractionTo(this TimeSpan remainingTime, TimeSpan totalTime)
        {
            return totalTime.Subtract(remainingTime).TotalSeconds / totalTime.TotalSeconds;
        }
    }
}