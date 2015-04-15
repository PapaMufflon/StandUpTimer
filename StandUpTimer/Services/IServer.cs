using System.Security;
using System.Threading.Tasks;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal interface IServer
    {
        void SendDeskState(Status status);
        Task<bool> LogIn(string username, SecureString password);
        Task<bool> LogOut();
    }
}