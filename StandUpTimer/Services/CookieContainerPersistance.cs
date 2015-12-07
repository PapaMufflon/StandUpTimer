using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace StandUpTimer.Services
{
    public static class CookieContainerPersistance
    {
        public static void WriteCookiesToDisk(this CookieContainer cookieJar)
        {
            using (var stream = File.Create(CookieContainerFile))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookieJar);
            }
        }

        public static CookieContainer ReadCookiesFromDisk()
        {
            try
            {
                using (var stream = File.Open(CookieContainerFile, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return new CookieContainer();
            }
        }

        public static void DeleteCookiesFromDisk()
        {
            if (File.Exists(CookieContainerFile))
                File.Delete(CookieContainerFile);
        }

        private static string CookieContainerFile
        {
            get
            {
                var applicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                return Path.Combine(applicationDataFolder, "cookie.dat");
            }
        }
    }
}