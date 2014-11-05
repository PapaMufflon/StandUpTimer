using System;
using System.Diagnostics.Contracts;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Models
{
    internal class StandUpModel : ICanSkip
    {
        public event EventHandler DeskStateChanged;

        public DeskState DeskState { get; set; }
        public DateTime ChangeTime { get; set; }
        public TimeSpan CurrentLeg { get { return changeTimer.Interval; } }

        internal static readonly TimeSpan StandingTime = TimeSpan.FromMinutes(20);
        internal static readonly TimeSpan SittingTime = TimeSpan.FromHours(1);

        private readonly ITimer changeTimer;

        public StandUpModel(ITimer timer)
        {
            Contract.Requires(timer != null);

            DeskState = DeskState.Sitting;

            ChangeTime = TestableDateTime.Now.Add(SittingTime);

            changeTimer = timer;
            changeTimer.Interval = SittingTime;
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
            ChangeTime = TestableDateTime.Now.Add(changeTimer.Interval);
            changeTimer.Start();
        }

        protected virtual void OnDeskStateChanged()
        {
            var handler = DeskStateChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}