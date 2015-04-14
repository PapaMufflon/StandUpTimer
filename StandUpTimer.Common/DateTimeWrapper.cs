using System;

namespace StandUpTimer.Common
{
    public class DateTimeWrapper : IDateTime
    {
        public DateTime Now { get { return DateTime.Now; } }
        public DateTime Today { get { return DateTime.Today; } }
    }
}