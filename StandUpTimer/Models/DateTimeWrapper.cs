using System;

namespace StandUpTimer.Models
{
    public class DateTimeWrapper : IDateTime
    {
        public DateTime Now { get { return DateTime.Now; } }
    }
}