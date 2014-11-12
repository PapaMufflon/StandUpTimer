using System;
using System.Windows;
using Fclp;
using StandUpTimer.Views;

namespace StandUpTimer
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var deskStateTimes = ParseCommandLineArguments(e.Args, new DeskStateTimes
            {
                StandingTime = TimeSpan.FromMinutes(20),
                SittingTime = TimeSpan.FromHours(1)
            });

            new MainWindow(deskStateTimes.SittingTime, deskStateTimes.StandingTime).Show();
        }

        private static DeskStateTimes ParseCommandLineArguments(string[] args, DeskStateTimes deskStateTimes)
        {
            var p = new FluentCommandLineParser();

            p.Setup<int>("stand")
             .Callback(x => deskStateTimes.StandingTime = TimeSpan.FromMilliseconds(x));

            p.Setup<int>("sit")
             .Callback(x => deskStateTimes.SittingTime = TimeSpan.FromMilliseconds(x));

            p.Parse(args);

            return deskStateTimes;
        }

        private struct DeskStateTimes
        {
            public TimeSpan StandingTime { get; set; }
            public TimeSpan SittingTime { get; set; }
        }
    }
}