using System;
using System.Diagnostics.Contracts;
using StandUpTimer.Common;
using StandUpTimer.Web.Contract;
using DeskState = StandUpTimer.Models.DeskState;

namespace StandUpTimer.Services
{
    internal class StatusPublisher
    {
        private readonly IServer server;

        public StatusPublisher(IServer server)
        {
            Contract.Requires(server != null);

            this.server = server;

            SendDeskState();
        }

        public void PublishChangedDeskState(DeskState newDeskState)
        {
            SendDeskState(newDeskState.ToWebContract());
        }

        private void SendDeskState(Web.Contract.DeskState deskState = Web.Contract.DeskState.Sitting)
        {
            server.SendDeskState(new Status
            {
                DateTime = TestableDateTime.Now.ToString(Status.DateTimeFormat),
                DeskState = deskState
            });
        }

        public void PublishEndOfSession()
        {
            SendDeskState(Web.Contract.DeskState.Inactive);
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
                    throw new ArgumentOutOfRangeException("deskState");
            }
        }
    }
}