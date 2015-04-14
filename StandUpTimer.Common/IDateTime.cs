using System;

namespace StandUpTimer.Common
{
    public interface IDateTime
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}