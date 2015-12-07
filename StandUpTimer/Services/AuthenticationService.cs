using System;
using System.Threading.Tasks;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        public event EventHandler AuthenticationStateChanged;

        private readonly IServer server;
        private readonly IDialogPresenter dialogPresenter;
        private bool isLoggedIn;

        public AuthenticationService(IServer server, IDialogPresenter dialogPresenter)
        {
            this.server = server;
            this.dialogPresenter = dialogPresenter;

            Task.Run(async () =>
            {
                IsLoggedIn = await server.IsLoggedIn();
            });
        }

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            private set
            {
                isLoggedIn = value;
                OnAuthenticationStateChanged();
            }
        }

        public async Task ChangeState()
        {
            if (IsLoggedIn)
                await TryLogOut();
            else
                await TryLogIn();
        }

        private async Task TryLogOut()
        {
            var tries = 0;

            do
            {
                if ((await server.LogOut()).Success)
                {
                    IsLoggedIn = false;
                    return;
                }

                tries++;
            } while (tries < 5);
        }

        private async Task TryLogIn()
        {
            var loginViewModel = new LoginViewModel();

            do
            {
                if (loginViewModel.Password != null)
                    loginViewModel.Password.Clear();

                if (dialogPresenter.ShowModal(loginViewModel) != true)
                    return;

                var communicationResult = await server.LogIn(loginViewModel.Username, loginViewModel.Password);

                if (communicationResult.Success)
                    IsLoggedIn = true;
                else
                    loginViewModel.ErrorMessage = communicationResult.Message;
            } while (!IsLoggedIn);
        }

        protected virtual void OnAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}