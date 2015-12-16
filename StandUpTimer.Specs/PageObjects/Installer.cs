using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace StandUpTimer.Specs.PageObjects
{
    public class Installer
    {
        private static string StandUpTimerFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StandUpTimer");
        private static string StandUpTimerUpdateExe => Path.Combine(StandUpTimerFolder, "Update.exe");
        public static string StandUpTimerExecutable => Directory.GetFiles(StandUpTimerFolder, "StandUpTimer.exe", SearchOption.AllDirectories).Single();

        public void TryUninstall()
        {
            var standUpTimerUpdateExe = StandUpTimerUpdateExe;

            if (!File.Exists(standUpTimerUpdateExe))
                return;

            var process = Process.Start(standUpTimerUpdateExe, "--uninstall");
            process.WaitForExit();
        }

        public void Install()
        {
            var setupExe = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");
            var webClient = new WebClient();
            webClient.DownloadFileTaskAsync("https://mufflonosoft.blob.core.windows.net/standuptimer/Setup.exe", setupExe).Wait();

            var process = Process.Start(setupExe);
            process.WaitForExit();
        }

        public void SetUpdateLocation(string updateLocation)
        {
            var configPath = StandUpTimerExecutable + ".config";

            var content = File.ReadAllText(configPath);
            content = content.Replace("http://mufflonosoft.blob.core.windows.net/standuptimer", updateLocation);

            File.WriteAllText(configPath, content);
        }
    }
}