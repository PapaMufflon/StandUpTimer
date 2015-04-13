using System;
using System.Windows;
using Fclp;
using StandUpTimer.Models;
using StandUpTimer.Services;
using StandUpTimer.ViewModels;
using StandUpTimer.Views;

namespace StandUpTimer
{
    public partial class App
    {
        private StatusPublisher statusPublisher;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            statusPublisher = new StatusPublisher();

            var deskStateTimes = ParseCommandLineArguments(e.Args, new DeskStateTimes
            {
                StandingTime = TimeSpan.FromMinutes(20),
                SittingTime = TimeSpan.FromHours(1)
            });

            var standUpModel = new StandUpModel(new DispatcherTimerWrapper(), deskStateTimes.SittingTime, deskStateTimes.StandingTime);
            standUpModel.DeskStateChanged += (s, f) => statusPublisher.PublishChangedDeskState(f.NewDeskState);

            new MainWindow(standUpModel).Show();
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

        protected override void OnExit(ExitEventArgs e)
        {
            statusPublisher.PublishEndOfSession();

            base.OnExit(e);
        }
    }
}