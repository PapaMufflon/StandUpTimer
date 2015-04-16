using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using Fclp;
using StandUpTimer.Models;
using StandUpTimer.Services;
using StandUpTimer.ViewModels;
using StandUpTimer.Views;

namespace StandUpTimer
{
    public partial class App : IBringToForeground
    {
        private StatusPublisher statusPublisher;
        private Updater updater;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Bootstrap(e.Args);
        }

        private void Bootstrap(string[] args)
        {
            var server = BootstrapServer();
            statusPublisher = new StatusPublisher(server);

            var deskStateTimes = ParseCommandLineArguments(args, new DeskStateTimes
            {
                StandingTime = TimeSpan.FromMinutes(20),
                SittingTime = TimeSpan.FromHours(1)
            });

            var standUpModel = new StandUpModel(new DispatcherTimerWrapper(), deskStateTimes.SittingTime, deskStateTimes.StandingTime);
            standUpModel.DeskStateChanged += (s, f) => statusPublisher.PublishChangedDeskState(f.NewDeskState);

            var standUpViewModel = new StandUpViewModel(standUpModel, new AuthenticationService(server, new DialogPresenter()), this);
            standUpViewModel.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName.Equals("AuthenticationStatus"))
                    statusPublisher.PublishChangedDeskState(standUpModel.DeskState);
            };

            MainWindow = new MainWindow(standUpViewModel);
            MainWindow.Show();

            updater = new Updater(MainWindow.Close);
        }

        private static Server BootstrapServer()
        {
            var cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:54776/")
            };

            return new Server(httpClient, cookieContainer);
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

        public void Now()
        {
            MainWindow.Activate();
            MainWindow.Topmost = true;
            MainWindow.Topmost = false;
        }
    }
}