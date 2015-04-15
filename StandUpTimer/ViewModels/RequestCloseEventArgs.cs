using System;

namespace StandUpTimer.ViewModels
{
    internal class RequestCloseEventArgs : EventArgs
    {
        public bool? DialogResult { get; set; }
    }
}