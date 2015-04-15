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

        public void ChangeState()
        {
            if (IsLoggedIn)
                TryLogOut();
            else
                TryLogIn();
        }

        private void TryLogOut()
        {
            var tries = 0;

            do
            {
                if (server.LogOut())
                {
                    IsLoggedIn = false;
                    return;
                }

                tries++;
            } while (tries < 5);
        }

        private void TryLogIn()
        {
            var loginViewModel = new LoginViewModel();

            do
            {
                if (loginViewModel.Password != null)
                    loginViewModel.Password.Clear();

                if (dialogPresenter.ShowModal(loginViewModel) != true)
                    return;

                if (server.LogIn(loginViewModel.Username, loginViewModel.Password))
                    IsLoggedIn = true;
            } while (!IsLoggedIn);
        }
    }
}