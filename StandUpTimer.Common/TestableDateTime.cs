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

        public static DateTime Now
        {
            get { return DateTime.Now; }
        }

        public static DateTime Today
        {
            get { return DateTime.Today; }
        }
    }
}