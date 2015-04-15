using System.Threading.Tasks;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Services
{
    internal class AuthenticationService
    {
        public bool IsLoggedIn { get; private set; }

        private readonly IServer server;
        private readonly IDialogPresenter dialogPresenter;

        public AuthenticationService(IServer server, IDialogPresenter dialogPresenter)
        {
            this.server = server;
            this.dialogPresenter = dialogPresenter;

            IsLoggedIn = false;
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
                if (await server.LogOut())
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

                if (await server.LogIn(loginViewModel.Username, loginViewModel.Password))
                    IsLoggedIn = true;
                else
                    loginViewModel.ErrorMessage = Properties.Resources.LoginFailed;
            } while (!IsLoggedIn);
        }
    }
}