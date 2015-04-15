namespace StandUpTimer.Services
{
    internal interface IDialogPresenter
    {
        bool? ShowModal(object loginViewModel);
    }
}