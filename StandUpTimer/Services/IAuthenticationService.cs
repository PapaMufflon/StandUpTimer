using System;
using System.Threading.Tasks;

namespace StandUpTimer.Services
{
    internal interface IAuthenticationService : IAuthenticationStatus
    {
        event EventHandler AuthenticationStateChanged;

        Task ChangeState();
    }

    internal interface IAuthenticationStatus
    {
        bool IsLoggedIn { get; }
    }
}