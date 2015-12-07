using System;
using StandUpTimer.Common;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Models
{
    internal class StandUpModel : ICanSkip
    {
        public event EventHandler<DeskStateChangedEventArgs> DeskStateChanged;

        public DeskState DeskState { get; set; }
        public DateTime ChangeTime { get; set; }
        public TimeSpan CurrentLeg => changeTimer.Interval;

        private readonly ITimer changeTimer;
        private readonly TimeSpan sittingTime;
        private readonly TimeSpan standingTime;

        public StandUpModel(ITimer timer, TimeSpan sittingTime, TimeSpan standingTime)
        {
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
            DeskStateChanged?.Invoke(this, new DeskStateChangedEventArgs(DeskState));
        }

        public void NewDeskStateStarted()
        {
            ChangeTime = TestableDateTime.Now.Add(changeTimer.Interval);
            changeTimer.Start();
        }
    }
}