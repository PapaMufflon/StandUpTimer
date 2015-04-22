using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal class Server : IServer
    {
        private const string AccountLoginUrl = "Account/Login";

        private readonly HttpClient httpClient;
        private readonly CookieContainer cookieContainer;

        public Server(HttpClient httpClient, CookieContainer cookieContainer)
        {
            Contract.Requires(httpClient != null);
            Contract.Requires(cookieContainer != null);

            this.httpClient = httpClient;
            this.cookieContainer = cookieContainer;
        }

        public async Task SendDeskState(Status status)
        {
            await httpClient.PostAsJsonAsync("statistics", status);
        }

        public async Task<CommunicationResult> LogIn(string username, SecureString password)
        {
            var result = await TryGetAntiForgeryToken();

            if (!result.Success)
                return new CommunicationResult
                {
                    Success = false,
                    Message = Properties.Resources.CommunicationFailed
                };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", username),
                new KeyValuePair<string, string>("Password", password.ConvertToUnsecureString()),
                new KeyValuePair<string, string>("__RequestVerificationToken", result.AccountToken)
            });

            try
            {
                await httpClient.PostAsync(AccountLoginUrl, content);
            }
            catch (HttpRequestException)
            {
                return new CommunicationResult
                {
                    Success = false,
                    Message = Properties.Resources.CommunicationFailed
                };
            }

            return cookieContainer.GetCookies(httpClient.BaseAddress).Cast<Cookie>().Any(x => x.Name.Equals(".AspNet.ApplicationCookie"))
                ? new CommunicationResult { Success = true }
                : new CommunicationResult { Success = false, Message = Properties.Resources.LoginFailed };
        }

        public async Task<CommunicationResult> LogOut()
        {
            var result = await TryGetAntiForgeryToken();

            if (!result.Success)
                return new CommunicationResult
                {
                    Success = false,
                    Message = Properties.Resources.CommunicationFailed
                };

            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("__RequestVerificationToken", result.AccountToken) });
            var response = await httpClient.PostAsync("Account/LogOff", content);

            return response.IsSuccessStatusCode
                ? new CommunicationResult { Success = true }
                : new CommunicationResult { Success = false, Message = Properties.Resources.CommunicationFailed };
        }

        private async Task<AccountTokenResult> TryGetAntiForgeryToken()
        {
            string returnedSite;

            try
            {
                returnedSite = await httpClient.GetStringAsync(AccountLoginUrl);
            }
            catch (HttpRequestException)
            {
                return new AccountTokenResult();
            }

            const string pattern = @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""(?<token>.*)"" />";
            var token = Regex.Match(returnedSite, pattern).Groups["token"].Value;

            return new AccountTokenResult
            {
                Success = true,
                AccountToken = token
            };
        }

        private class AccountTokenResult
        {
            public bool Success { get; set; }
            public string AccountToken { get; set; }
        }
    }
}