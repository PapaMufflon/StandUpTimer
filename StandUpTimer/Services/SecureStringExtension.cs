using System;
using System.Runtime.InteropServices;
using System.Security;

namespace StandUpTimer.Services
{
    public static class SecureStringExtension
    {
        // taken from http://blogs.msdn.com/b/fpintos/archive/2009/06/12/how-to-properly-convert-securestring-to-string.aspx
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
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

        public static SecureString GetSecureString(this string password, bool readOnly = true)
        {
            var secureString = new SecureString();

            foreach (var c in password)
            {
                secureString.AppendChar(c);
            }

            if (readOnly)
                secureString.MakeReadOnly();

            return secureString;
        }
    }
}