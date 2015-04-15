using StandUpTimer.Views;

namespace StandUpTimer.Services
{
    internal class DialogPresenter : IDialogPresenter
    {
        public bool? ShowModal(object loginViewModel)
        {
            return new Dialog { DataContext = loginViewModel }.ShowDialog();
        }
    }
}