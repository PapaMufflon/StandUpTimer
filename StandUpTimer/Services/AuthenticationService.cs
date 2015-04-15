namespace StandUpTimer.Services
{
    internal class AuthenticationService
    {
        public bool IsLoggedIn { get; private set; }

        private readonly IServer server;

        public AuthenticationService(IServer server)
        {
            this.server = server;

            IsLoggedIn = false;
        }

        public void ChangeState()
        {
            if (IsLoggedIn)
                server.LogOut();
            else
            {
                
                server.LogIn(credentials.Username, credentials.Password);
            }
        }
    }
}