using System;
using System.IO;
using System.Reflection;
using Castle.Core.Logging;
using StandUpTimer.Web;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Screenshots;

namespace StandUpTimer.Specs.PageObjects
{
    public static class Host
    {
        public static readonly SelenoHost Instance = new SelenoHost();
        public static readonly FileCamera Camera = new FileCamera(GetScreenShotPath());

        private static string GetScreenShotPath()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            if (string.IsNullOrEmpty(directoryName))
                throw new Exception("Cannot get the directory for storing images.");

            var localPath = new Uri(directoryName).LocalPath;

            return Path.Combine(localPath, "results", "StandUpTimer", "Specs");
        }

        static Host()
        {
            Instance.Run("StandUpTimer.Web", 12346, c => c
                .UsingLoggerFactory(new ConsoleFactory())
                // If you are using MVC then do this where RouteConfig is the class that registers your routes in the "Name.Of.Your.Web.Project" project
                // If you aren't using MVC then don't include this line
                .WithRouteConfig(RouteConfig.RegisterRoutes)
                .UsingCamera(Camera)
                );
        }
    }
}