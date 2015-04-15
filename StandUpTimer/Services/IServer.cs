using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal interface IServer
    {
        void SendDeskState(Status status);
        void LogIn(string username, string password);
        void LogOut();
    }
}