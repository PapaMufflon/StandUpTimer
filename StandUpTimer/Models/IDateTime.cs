using System;

namespace StandUpTimer.Models
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}