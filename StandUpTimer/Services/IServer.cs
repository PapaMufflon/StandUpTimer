using System.Security;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal interface IServer
    {
        void SendDeskState(Status status);
        bool LogIn(string username, SecureString password);
        bool LogOut();
    }
}