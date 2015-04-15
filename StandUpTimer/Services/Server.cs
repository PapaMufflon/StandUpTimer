using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly HttpClient httpClient;
        private readonly CookieContainer cookies;

        public Server()
        {
            cookies = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                UseCookies = true
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }

        public void SendDeskState(Status status)
        {
            var foo = httpClient.PostAsJsonAsync("statistics", status).Result;
            var bar = foo;
        }

        public async Task<bool> LogIn(string username, SecureString password)
        {
            var returnedSite = await httpClient.GetStringAsync("Account/Login");
            const string pattern = @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""(?<token>.*)"" />";
            var token = Regex.Match(returnedSite, pattern).Groups["token"].Value;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("UserName", username),
                new KeyValuePair<string, string>("Password", password.ConvertToUnsecureString()),
                new KeyValuePair<string, string>("__RequestVerificationToken", token)
            });

            var response = await httpClient.PostAsync("Account/Login", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LogOut()
        {
            throw new NotImplementedException();
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