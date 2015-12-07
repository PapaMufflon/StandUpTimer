using System.Security;
using System.Threading.Tasks;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal interface IServer
    {
        Task<string> GetStatisticsPage();
        Task SendDeskState(Status status);

        Task<AccountTokenResult> TryGetAntiForgeryToken();
        Task<bool> TrySendCredentials(string username, SecureString password, string accountToken);
        bool ContainsCookie(string cookieName);
        void WriteCookiesToDisk();
        Task<bool> TryLogOff(string accountToken);
    }
}