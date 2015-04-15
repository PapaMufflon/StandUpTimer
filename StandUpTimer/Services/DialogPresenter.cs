using StandUpTimer.ViewModels;
using StandUpTimer.Views;

namespace StandUpTimer.Services
{
    internal class DialogPresenter : IDialogPresenter
    {
        public bool? ShowModal(IDialogViewModel dialogViewModel)
        {
            return new Dialog(dialogViewModel).ShowDialog();
        }
    }
}