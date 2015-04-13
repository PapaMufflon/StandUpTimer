using System;
using System.Net.Http;
using StandUpTimer.Web.Contract;
using DeskState = StandUpTimer.Models.DeskState;

namespace StandUpTimer.Services
{
    internal class StatusPublisher
    {
        private readonly HttpClient httpClient;

        public StatusPublisher()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:54776/statistics") };
            SendDeskState();
        }

        public void PublishChangedDeskState(DeskState newDeskState)
        {
            SendDeskState(newDeskState.ToWebContract());
        }

        private void SendDeskState(Web.Contract.DeskState deskState = Web.Contract.DeskState.Standing)
        {
            httpClient.PostAsJsonAsync(string.Empty, new
            {
                DateTime = DateTime.Now.ToString(Status.DateTimeFormat),
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