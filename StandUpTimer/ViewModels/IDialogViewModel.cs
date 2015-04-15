using System;

namespace StandUpTimer.ViewModels
{
    internal interface IDialogViewModel
    {
        event EventHandler<RequestCloseEventArgs> RequestClose;

        string Title { get; }
    }
}