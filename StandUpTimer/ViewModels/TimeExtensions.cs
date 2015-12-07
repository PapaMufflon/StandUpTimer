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
                return $"{remainingTime.TotalSeconds:0}\nsec";

            return $"{remainingTime.TotalMinutes:0}\nmin";
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