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
                server.LogOut();
            else
            {
                var loginViewModel = new LoginViewModel();
                var result = dialogPresenter.ShowModal(loginViewModel);

                if (result == true)
                    server.LogIn(loginViewModel.Username, loginViewModel.Password);
            }
        }
    }
}