using System;

namespace StandUpTimer.Models
{
    internal interface ITimer
    {
        event EventHandler Tick;

        TimeSpan Interval { get; set; }

        void Start();
        void Stop();
    }
}