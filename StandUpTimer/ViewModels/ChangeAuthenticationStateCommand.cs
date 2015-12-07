using System;
using System.Threading.Tasks;
using System.Windows.Input;
using StandUpTimer.Services;

namespace StandUpTimer.ViewModels
{
    internal class ChangeAuthenticationStateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IAuthenticationService authenticationService;
        private readonly IDialogPresenter dialogPresenter;
        private readonly LoginViewModel loginViewModel;

        public ChangeAuthenticationStateCommand(IAuthenticationService authenticationService, IDialogPresenter dialogPresenter, LoginViewModel loginViewModel)
        {
            this.authenticationService = authenticationService;
            this.dialogPresenter = dialogPresenter;
            this.loginViewModel = loginViewModel;
        }

        public async void Execute(object parameter)
        {
            if (authenticationService.IsLoggedIn)
                await TryLogOut();
            else
                await TryLogIn();
        }

        private async Task TryLogIn()
        {
            do
            {
                loginViewModel.Password?.Clear();

                if (dialogPresenter.ShowModal(loginViewModel) != true)
                    return;

                var communicationResult = await authenticationService.LogIn(loginViewModel.Username, loginViewModel.Password);

                if (!communicationResult.Success)
                    loginViewModel.ErrorMessage = communicationResult.Message;
            } while (!authenticationService.IsLoggedIn);
        }

        private async Task TryLogOut()
        {
            var tries = 0;

            do
            {
                if ((await authenticationService.LogOff()).Success)
                    return;

                tries++;
            } while (tries < 5);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}