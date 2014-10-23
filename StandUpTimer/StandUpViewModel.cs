using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;
using StandUpTimer.Annotations;

namespace StandUpTimer
{
    public class StandUpViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer updateTimer;
        private string currentImage;
        private Visibility exitButtonVisibility;
        private Visibility okButtonVisibility;
        private bool shake;
        private readonly StandUpModel model;
        private OkCommand okCommand;
        private SkipCommand skipCommand;

        public StandUpViewModel(IBringToForeground bringToForeground)
        {
            model = new StandUpModel();
            model.DeskStateChanged += (sender, args) =>
            {
                SetImageAccordingToDeskState();

                OkButtonVisibility = Visibility.Visible;
                Shake = true;

                bringToForeground.Now();
            };

            SetImageAccordingToDeskState();
            ExitButtonVisibility = Visibility.Hidden;
            OkButtonVisibility = Visibility.Collapsed;

            updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            updateTimer.Tick += OnUpdateTimerTicked;
            updateTimer.Start();
        }

        private void SetImageAccordingToDeskState()
        {
            switch (model.DeskState)
            {
                case DeskState.Sitting:
                    CurrentImage = "Images\\sitting.png";
                    break;
                case DeskState.Standing:
                    CurrentImage = "Images\\standing.png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnUpdateTimerTicked(object sender, EventArgs e)
        {
            OnPropertyChanged("RemainingTimeToChangeAsString");
            OnPropertyChanged("RemainingTimeToChangeInPercent");
            OnPropertyChanged("TimeOfLegInFraction");
        }

        public void StartTimer()
        {
            model.NewDeskStateStarted();
        }

        public string RemainingTimeToChangeAsString
        {
            get
            {
                var remainingTimeToChange = model.ChangeTime.Subtract(DateTime.Now);

                if (remainingTimeToChange <= TimeSpan.Zero)
                    return string.Empty;

                if (remainingTimeToChange < TimeSpan.FromMinutes(1))
                    return string.Format("{0:0}\nsec", remainingTimeToChange.TotalSeconds);

                return string.Format("{0:0}\nmin", remainingTimeToChange.TotalMinutes);
            }
        }

        public double RemainingTimeToChangeInPercent
        {
            get
            {
                var remainingTimeToChange = model.ChangeTime.Subtract(DateTime.Now);

                return remainingTimeToChange > TimeSpan.Zero
                           ? remainingTimeToChange.TotalSeconds / model.CurrentLeg.TotalSeconds * 100.0
                           : 0.0;
            }
        }

        public double TimeOfLegInFraction
        {
            get
            {
                var remainingTimeToChange = model.ChangeTime.Subtract(DateTime.Now);

                return model.CurrentLeg.Subtract(remainingTimeToChange).TotalSeconds / model.CurrentLeg.TotalSeconds;
            }
        }
        

        public string CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }

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

        public bool Shake
        {
            get { return shake; }
            set
            {
                shake = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand
        {
            get
            {
                return okCommand ?? (okCommand = new OkCommand(this));
            }
        }

        public ICommand SkipCommand
        {
            get
            {
                return skipCommand ?? (skipCommand = new SkipCommand(model, OkCommand));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}