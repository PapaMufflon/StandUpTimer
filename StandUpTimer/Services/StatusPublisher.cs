using System;
using System.Threading.Tasks;
using StandUpTimer.Common;
using StandUpTimer.Web.Contract;
using DeskState = StandUpTimer.Models.DeskState;

namespace StandUpTimer.Services
{
    internal class StatusPublisher
    {
        private readonly IServer server;
        private readonly IAuthenticationStatus authenticationStatus;

        public StatusPublisher(IServer server, IAuthenticationStatus authenticationStatus)
        {
            this.server = server;
            this.authenticationStatus = authenticationStatus;

            if (authenticationStatus.IsLoggedIn)
            {
                Task.Run(async () => await SendDeskState());
            }
        }

        public async Task PublishChangedDeskState(DeskState newDeskState)
        {
            await SendDeskState(newDeskState.ToWebContract());
        }

        private async Task SendDeskState(Web.Contract.DeskState deskState = Web.Contract.DeskState.Sitting)
        {
            if (authenticationStatus.IsLoggedIn)
            {
                await server.SendDeskState(
                    new Status
                    {
                        DateTime = TestableDateTime.Now.ToString(Status.DateTimeFormat),
                        DeskState = deskState
                    });
            }
        }

        public async Task PublishEndOfSession()
        {
            await SendDeskState(Web.Contract.DeskState.Inactive);
        }
    }

    public static class DeskStateConversionExtension
    {
        public static Web.Contract.DeskState ToWebContract(this DeskState deskState)
        {
            switch (deskState)
            {
                case DeskState.Sitting:
                    return Web.Contract.DeskState.Sitting;
                case DeskState.Standing:
                    return Web.Contract.DeskState.Standing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deskState));
            }
        }
    }
}