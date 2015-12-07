using System.Collections.Generic;
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
        private const string StatisticsRoute = "statistics";

        private readonly HttpClient httpClient;
        private readonly CookieContainer cookieContainer;

        public Server(HttpClient httpClient, CookieContainer cookieContainer)
        {
            this.httpClient = httpClient;
            this.cookieContainer = cookieContainer;
        }

        public async Task<string> GetStatisticsPage()
        {
            var response = await httpClient.GetAsync(StatisticsRoute);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }

        public async Task SendDeskState(Status status)
        {
            await httpClient.PostAsJsonAsync(StatisticsRoute, status);
        }

        public async Task<bool> TrySendCredentials(string username, SecureString password, string accountToken)
        {
            var content = CompileContent(username, password, accountToken);

            try
            {
                await httpClient.PostAsync(AccountLoginUrl, content);
            }
            catch (HttpRequestException)
            {
                return false;
            }

            return true;
        }

        private static FormUrlEncodedContent CompileContent(string username, SecureString password, string accountToken)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", username),
                new KeyValuePair<string, string>("Password", password.ConvertToUnsecureString()),
                new KeyValuePair<string, string>("__RequestVerificationToken", accountToken)
            });

            return content;
        }

        public bool ContainsCookie(string cookieName)
        {
            return cookieContainer.GetCookies(httpClient.BaseAddress).Cast<Cookie>().Any(x => x.Name.Equals(cookieName));
        }

        public void WriteCookiesToDisk()
        {
            cookieContainer.WriteCookiesToDisk();
        }

        public async Task<bool> TryLogOff(string accountToken)
        {
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("__RequestVerificationToken", accountToken) });
            var response = await httpClient.PostAsync("Account/LogOff", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<AccountTokenResult> TryGetAntiForgeryToken()
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
    }
}