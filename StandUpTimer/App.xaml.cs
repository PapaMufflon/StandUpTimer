using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using Fclp;
using StandUpTimer.Models;
using StandUpTimer.Properties;
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

            var commandLineArguments = ParseCommandLineArguments(e.Args);

            Bootstrap(commandLineArguments);
        }

        private static CommandLineArguments ParseCommandLineArguments(string[] args)
        {
            var result = new CommandLineArguments
            {
                DeskStateTimes = new DeskStateTimes
                {
                    StandingTime = TimeSpan.FromMinutes(20),
                    SittingTime = TimeSpan.FromHours(1)
                },
                Update = true,
                BaseUrl = Settings.Default.BaseUrl
            };

            var p = new FluentCommandLineParser();

            p.Setup<int>("stand")
             .Callback(x => result.DeskStateTimes.StandingTime = TimeSpan.FromMilliseconds(x));

            p.Setup<int>("sit")
             .Callback(x => result.DeskStateTimes.SittingTime = TimeSpan.FromMilliseconds(x));

            p.Setup<bool>("noUpdate")
                .Callback(x => result.Update = false);

            p.Setup<string>("baseUrl")
                .Callback(x => result.BaseUrl = x);

            p.Parse(args);

            return result;
        }

        private class DeskStateTimes
        {
            public TimeSpan StandingTime { get; set; }
            public TimeSpan SittingTime { get; set; }
        }

        private class CommandLineArguments
        {
            public DeskStateTimes DeskStateTimes { get; set; }
            public bool Update { get; set; }
            public string BaseUrl { get; set; }
        }

        private void Bootstrap(CommandLineArguments commandLineArguments)
        {
            var server = BootstrapServer(commandLineArguments.BaseUrl);
            statusPublisher = new StatusPublisher(server);

            var standUpModel = new StandUpModel(new DispatcherTimerWrapper(), commandLineArguments.DeskStateTimes.SittingTime, commandLineArguments.DeskStateTimes.StandingTime);
            standUpModel.DeskStateChanged += (s, f) => statusPublisher.PublishChangedDeskState(f.NewDeskState);

            var standUpViewModel = new StandUpViewModel(standUpModel, new AuthenticationService(server, new DialogPresenter()), this);
            standUpViewModel.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName.Equals("AuthenticationStatus"))
#pragma warning disable 4014
                    // fire and forget
                    statusPublisher.PublishChangedDeskState(standUpModel.DeskState);
#pragma warning restore 4014
            };

            MainWindow = new MainWindow(standUpViewModel);
            MainWindow.Show();

            if (commandLineArguments.Update)
                updater = new Updater(MainWindow.Close);
        }

        private static Server BootstrapServer(string baseUrl)
        {
            var cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            return new Server(httpClient, cookieContainer);
        }

        public void Now()
        {
            MainWindow.Activate();
            MainWindow.Topmost = true;
            MainWindow.Topmost = false;
        }

        private async void App_OnExit(object sender, ExitEventArgs e)
        {
            await statusPublisher.PublishEndOfSession();
        }
    }
}