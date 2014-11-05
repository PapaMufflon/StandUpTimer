using System;
using System.Windows.Threading;
using StandUpTimer.Models;

namespace StandUpTimer.ViewModels
{
    public class DispatcherTimerWrapper : ITimer
    {
        public event EventHandler Tick;

        private readonly DispatcherTimer internalTimer;

        public DispatcherTimerWrapper()
        {
            internalTimer = new DispatcherTimer();
            internalTimer.Tick += (s, e) => OnTick();
        }

        protected virtual void OnTick()
        {
            var handler = Tick;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public TimeSpan Interval
        {
            get { return internalTimer.Interval; }
            set { internalTimer.Interval = value; }
        }

        public void Start()
        {
            internalTimer.Start();
        }

        public void Stop()
        {
            internalTimer.Stop();
        }
    }
}