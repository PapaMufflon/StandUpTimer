using System;

namespace StandUpTimer.Models
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
    }
}