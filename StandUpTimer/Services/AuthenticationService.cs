using System;
using System.Security;
using System.Threading.Tasks;
using StandUpTimer.Properties;

namespace StandUpTimer.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        public event EventHandler AuthenticationStateChanged;

        private readonly IServer server;
        private bool isLoggedIn;

        public AuthenticationService(IServer server)
        {
            this.server = server;

            Task.Run(async () =>
            {
                IsLoggedIn = await IsCurrentlyLoggedIn();
            });
        }

        private async Task<bool> IsCurrentlyLoggedIn()
        {
            var content = await server.GetStatisticsPage();

            return !content.Contains("<title>Log in");
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

        public async Task<CommunicationResult> LogIn(string username, SecureString password)
        {
            var antiForgeryResult = await server.TryGetAntiForgeryToken();

            if (!antiForgeryResult.Success)
                return new CommunicationResult
                {
                    Success = false,
                    Message = Resources.CommunicationFailed
                };

            var credentialsSendSuccessfully = await server.TrySendCredentials(username, password, antiForgeryResult.AccountToken);

            if (!credentialsSendSuccessfully)
                return new CommunicationResult
                {
                    Success = false,
                    Message = Resources.CommunicationFailed
                };

            if (!server.ContainsCookie(".AspNet.ApplicationCookie"))
            {
                return new CommunicationResult
                {
                    Success = false,
                    Message = Resources.LoginFailed
                };
            }

            server.WriteCookiesToDisk();
            IsLoggedIn = true;

            return new CommunicationResult { Success = true };
        }

        public async Task<CommunicationResult> LogOff()
        {
            var antiForgeryResult = await server.TryGetAntiForgeryToken();

            if (!antiForgeryResult.Success)
                return new CommunicationResult
                {
                    Success = false,
                    Message = Resources.CommunicationFailed
                };

            var loggedOffSuccessfully = await server.TryLogOff(antiForgeryResult.AccountToken);

            if (!loggedOffSuccessfully)
            {
                return new CommunicationResult
                {
                    Success = false,
                    Message = Resources.CommunicationFailed
                };
            }

            IsLoggedIn = false;

            return new CommunicationResult { Success = true };
        }

        protected virtual void OnAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}