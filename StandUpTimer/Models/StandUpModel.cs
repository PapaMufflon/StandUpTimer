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

        private readonly ITimer changeTimer;
        private readonly TimeSpan sittingTime;
        private readonly TimeSpan standingTime;

        public StandUpModel(ITimer timer, TimeSpan sittingTime, TimeSpan standingTime)
        {
            Contract.Requires(timer != null);
            Contract.Requires(sittingTime != TimeSpan.Zero);
            Contract.Requires(standingTime != TimeSpan.Zero);

            DeskState = DeskState.Sitting;

            ChangeTime = TestableDateTime.Now.Add(sittingTime);

            changeTimer = timer;
            this.sittingTime = sittingTime;
            this.standingTime = standingTime;
            changeTimer.Interval = sittingTime;
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
                    changeTimer.Interval = standingTime;
                    break;

                case DeskState.Standing:
                    DeskState = DeskState.Sitting;
                    changeTimer.Interval = sittingTime;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnDeskStateChanged();
        }

        protected virtual void OnDeskStateChanged()
        {
            var handler = DeskStateChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void NewDeskStateStarted()
        {
            ChangeTime = TestableDateTime.Now.Add(changeTimer.Interval);
            changeTimer.Start();
        }
    }
}