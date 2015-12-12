using System;

namespace StandUpTimer.Common
{
    public class DateTimeWrapper : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => DateTime.Today;
    }
}