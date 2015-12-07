using System;
using System.Security;
using System.Threading.Tasks;

namespace StandUpTimer.Services
{
    internal interface IAuthenticationService : IAuthenticationStatus
    {
        event EventHandler AuthenticationStateChanged;

        Task<CommunicationResult> LogIn(string username, SecureString password);
        Task<CommunicationResult> LogOff();
    }

    internal interface IAuthenticationStatus
    {
        bool IsLoggedIn { get; }
    }
}