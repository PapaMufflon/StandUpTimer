using System;
using System.Windows.Threading;

namespace StandUpTimer
{
    internal class StandUpModel : ICanSkip
    {
        public event EventHandler DeskStateChanged;

        public DeskState DeskState { get; set; }
        public DateTime ChangeTime { get; set; }
        public TimeSpan CurrentLeg { get { return changeTimer.Interval; } }

        private static readonly TimeSpan StandingTime = TimeSpan.FromSeconds(8);
        private static readonly TimeSpan SittingTime = TimeSpan.FromSeconds(8);

        //private static readonly TimeSpan StandingTime = TimeSpan.FromMinutes(20);
        //private static readonly TimeSpan SittingTime = TimeSpan.FromHours(1);

        private readonly DispatcherTimer changeTimer;

        public StandUpModel()
        {
            DeskState = DeskState.Sitting;

            ChangeTime = DateTime.Now.Add(SittingTime);

            changeTimer = new DispatcherTimer { Interval = SittingTime };
            changeTimer.Tick += OnChangeTimerTicked;
            changeTimer.Start();
        }

        private void OnChangeTimerTicked(object sender, EventArgs e)
        {
            GoToNextDeskState();

            changeTimer.Stop();
        }

        public void Skip()
        {
            OnChangeTimerTicked(this, EventArgs.Empty);
        }

        private void GoToNextDeskState()
        {
            switch (DeskState)
            {
                case DeskState.Sitting:
                    DeskState = DeskState.Standing;
                    changeTimer.Interval = StandingTime;
                    break;

                case DeskState.Standing:
                    DeskState = DeskState.Sitting;
                    changeTimer.Interval = SittingTime;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnDeskStateChanged();
        }

        public void NewDeskStateStarted()
        {
            ChangeTime = DateTime.Now.Add(changeTimer.Interval);
            changeTimer.Start();
        }

        protected virtual void OnDeskStateChanged()
        {
            var handler = DeskStateChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}