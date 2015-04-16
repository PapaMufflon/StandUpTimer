using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Services
{
    internal class Server : IServer
    {
        private const string BaseUrl = "http://localhost:54776";
        private const string AccountLoginUrl = "Account/Login";

        private readonly HttpClient httpClient;

        public Server()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }

        public void SendDeskState(Status status)
        {
            httpClient.PostAsJsonAsync("statistics", status);
        }

        public async Task<CommunicationResult> LogIn(string username, SecureString password)
        {
            var result = await TryGetAccountToken();

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

            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync(AccountLoginUrl, content);
            }
            catch (HttpRequestException)
            {
                return new CommunicationResult
                {
                    Success = false,
                    Message = Properties.Resources.CommunicationFailed
                };
            }

            return response.IsSuccessStatusCode
                ? new CommunicationResult { Success = true }
                : new CommunicationResult { Success = false, Message = Properties.Resources.LoginFailed };
        }

        public async Task<CommunicationResult> LogOut()
        {
            var result = await TryGetAccountToken();

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

        private async Task<AccountTokenResult> TryGetAccountToken()
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

    public static class SecureStringExtension
    {
        // taken from http://blogs.msdn.com/b/fpintos/archive/2009/06/12/how-to-properly-convert-securestring-to-string.aspx
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}