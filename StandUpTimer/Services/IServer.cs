using System.Diagnostics.Contracts;
using System.Security;
using System.Threading.Tasks;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    [ContractClass(typeof(ContractForIServer))]
    internal interface IServer
    {
        Task SendDeskState(Status status);
        Task<CommunicationResult> LogIn(string username, SecureString password);
        Task<CommunicationResult> LogOut();
    }

    [ContractClassFor(typeof(IServer))]
    internal abstract class ContractForIServer : IServer
    {
        public Task SendDeskState(Status status)
        {
            Contract.Requires(status != null);

            throw new System.NotImplementedException();
        }

        public Task<CommunicationResult> LogIn(string username, SecureString password)
        {
            Contract.Requires(!string.IsNullOrEmpty(username));
            Contract.Requires(password != null);

            throw new System.NotImplementedException();
        }

        public Task<CommunicationResult> LogOut()
        {
            throw new System.NotImplementedException();
        }
    }
}