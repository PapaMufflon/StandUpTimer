using System;
using System.Net.Http;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal class Server : IServer
    {
        private readonly HttpClient httpClient;

        public Server()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:54776") };
        }

        public void SendDeskState(Status status)
        {
            httpClient.PostAsJsonAsync("statistics", status);
        }

        public void LogIn(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void LogOut()
        {
            throw new NotImplementedException();
        }
    }
}