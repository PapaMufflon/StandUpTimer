using System;

namespace StandUpTimer.Models
{
    internal class DeskStateChangedEventArgs : EventArgs
    {
        public DeskState NewDeskState { get; private set; }

        public DeskStateChangedEventArgs(DeskState newDeskState)
        {
            NewDeskState = newDeskState;
        }
    }
}