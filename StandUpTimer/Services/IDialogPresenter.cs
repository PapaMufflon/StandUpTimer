using StandUpTimer.ViewModels;

namespace StandUpTimer.Services
{
    internal interface IDialogPresenter
    {
        bool? ShowModal(IDialogViewModel dialogViewModel);
    }
}