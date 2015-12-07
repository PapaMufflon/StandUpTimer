using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using StandUpTimer.Annotations;
using StandUpTimer.Common;
using StandUpTimer.Models;
using StandUpTimer.Properties;
using StandUpTimer.Services;

namespace StandUpTimer.ViewModels
{
    internal class StandUpViewModel : INotifyPropertyChanged, ICanHandleDeskStateStarts
    {
        private readonly DispatcherTimer updateTimer;
        private string currentImage;
        private Visibility exitButtonVisibility;
        private Visibility okButtonVisibility;
        private Visibility creativeCommonsVisibility;
        private bool shake;
        private readonly StandUpModel model;
        private readonly IAuthenticationService authenticationService;
        private readonly IBringToForeground bringToForeground;
        private ICommand okCommand;
        private ICommand skipCommand;
        private ICommand changeAuthenticationStateCommand;

        public StandUpViewModel(StandUpModel model, IAuthenticationService authenticationService, IBringToForeground bringToForeground)
        {
            this.model = model;
            this.authenticationService = authenticationService;
            this.bringToForeground = bringToForeground;

            model.DeskStateChanged += (sender, args) => DeskStateEnded();

            SetImageAccordingToDeskState();
            ExitButtonVisibility = Visibility.Hidden;
            OkButtonVisibility = Visibility.Collapsed;
            CreativeCommonsVisibility = Visibility.Hidden;

            authenticationService.AuthenticationStateChanged += (sender, args) => OnPropertyChanged(() => AuthenticationStatus);

            updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            updateTimer.Tick += OnUpdateTimerTicked;
            updateTimer.Start();
        }

        public void DeskStateEnded()
        {
            Contract.Requires<InvalidOperationException>(Shake == false);

            SetImageAccordingToDeskState();

            OkButtonVisibility = Visibility.Visible;
            Shake = true;

            bringToForeground.Now();
        }

        private void SetImageAccordingToDeskState()
        {
            switch (model.DeskState)
            {
                case DeskState.Sitting:
                    CurrentImage = @"..\Images\sittingTobi.png";
                    break;
                case DeskState.Standing:
                    CurrentImage = @"..\Images\standingTobi.png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnUpdateTimerTicked(object sender, EventArgs e)
        {
            OnPropertyChanged(() => RemainingTimeToChangeAsString);
            OnPropertyChanged(() => RemainingTimeToChangeInPercent);
            OnPropertyChanged(() => TimeOfLegInFraction);
        }

        public void DeskStateStarted()
        {
            Contract.Requires<InvalidOperationException>(Shake == true);

            Shake = false;
            OkButtonVisibility = Visibility.Collapsed;

            model.NewDeskStateStarted();
        }

        public string RemainingTimeToChangeAsString => model.ChangeTime.Subtract(TestableDateTime.Now).FormatRemainingTime();

        public double RemainingTimeToChangeInPercent => model.ChangeTime.Subtract(TestableDateTime.Now).PercentageTo(model.CurrentLeg);

        public double TimeOfLegInFraction => model.ChangeTime.Subtract(TestableDateTime.Now).FractionTo(model.CurrentLeg);

        public string CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }

        public string AuthenticationStatus => authenticationService.IsLoggedIn ? @"..\Images\loggedInButton.png" : @"..\Images\loginButton.png";

        public string AuthenticationStatusText => authenticationService.IsLoggedIn ? Resources.Logout : Resources.Login;

        public Visibility ExitButtonVisibility
        {
            get { return exitButtonVisibility; }
            set
            {
                exitButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility OkButtonVisibility
        {
            get { return okButtonVisibility; }
            set
            {
                okButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CreativeCommonsVisibility
        {
            get { return creativeCommonsVisibility; }
            set
            {
                creativeCommonsVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool Shake
        {
            get { return shake; }
            set
            {
                shake = value;
                OnPropertyChanged();
            }
        }

        public string VersionNumber => "V" + Assembly.GetExecutingAssembly().GetName().Version;

        public ICommand OkCommand => okCommand ?? (okCommand = new RelayCommand(_ => DeskStateStarted()));

        public ICommand SkipCommand => skipCommand ?? (skipCommand = new RelayCommand(_ =>
        {
            model.Skip();
            DeskStateStarted();
        }));

        public ICommand ChangeAuthenticationStateCommand => changeAuthenticationStateCommand
            ?? (changeAuthenticationStateCommand = new ChangeAuthenticationStateCommand(authenticationService, new DialogPresenter(), new LoginViewModel()));

        public List<Artist> Artists => new List<Artist>
        {
            new Artist("Convoy", new Uri("http://www.thenounproject.com/convoy"), "Skip"),
            new Artist("Alex S. Lakas", new Uri("http://www.thenounproject.com/alex.s.lakas"), "Close"),
            new Artist("Austin Condiff", new Uri("http://www.thenounproject.com/acondiff"), "Creative Commons"),
            new Artist("Ricardo Augusto Cherem", new Uri("http://www.thenounproject.com/ricardo.cherem"), "Check Mark"),
            new Artist("Simon", new Uri("http://www.thenounproject.com/simon.david"), "User Mark"),
        };

        internal class Artist
        {
            public string Name { get; private set; }
            public Uri Profile { get; private set; }
            public string Item { get; private set; }

            public Artist(string name, Uri profile, string item)
            {
                Name = name;
                Profile = profile;
                Item = item;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged<T>(Expression<Func<T>> exp)
        {
            var memberExpression = (MemberExpression)exp.Body;
            var propertyName = memberExpression.Member.Name;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}