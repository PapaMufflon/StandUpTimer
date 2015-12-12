using System;

namespace StandUpTimer.Common
{
    public static class TestableDateTime
    {
        public static IDateTime DateTime { get; set; }

        static TestableDateTime()
        {
            DateTime = new DateTimeWrapper();
        }

        public static DateTime Now => DateTime.Now;

        public static DateTime Today => DateTime.Today;
    }
}