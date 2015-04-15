using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
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

        public bool LogIn(string username, SecureString password)
        {
            throw new NotImplementedException();
        }

        public bool LogOut()
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