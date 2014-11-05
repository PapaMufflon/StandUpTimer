using System;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Models;

namespace StandUpTimer.UnitTests
{
    [TestFixture]
    public class StandUpModelTests
    {
        [Test]
        public void When_the_timer_ticks_Change_the_desk_state()
        {
            var timer = A.Fake<ITimer>();
            var target = new StandUpModel(timer);

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Sitting));

            timer.Tick += Raise.WithEmpty().Now;

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Standing));

            timer.Tick += Raise.WithEmpty().Now;

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Sitting));
        }

        [Test]
        public void The_current_leg_is_different_for_each_desk_state()
        {
            var timer = A.Fake<ITimer>();
            var target = new StandUpModel(timer);

            Assert.That(target.CurrentLeg, Is.EqualTo(StandUpModel.SittingTime));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.CurrentLeg, Is.EqualTo(StandUpModel.StandingTime));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.CurrentLeg, Is.EqualTo(StandUpModel.SittingTime));
        }

        [Test]
        public void The_change_time_is_the_time_when_the_new_desk_state_ends()
        {
            var now = new DateTime(2014, 5, 11);

            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(now);

            var timer = A.Fake<ITimer>();
            var target = new StandUpModel(timer);

            Assert.That(target.ChangeTime, Is.EqualTo(now.Add(StandUpModel.SittingTime)));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.ChangeTime, Is.EqualTo(now.Add(StandUpModel.StandingTime)));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.ChangeTime, Is.EqualTo(now.Add(StandUpModel.SittingTime)));
        }

        [Test]
        public void Skipping_is_the_same_as_a_timer_tick()
        {
            var target = new StandUpModel(A.Fake<ITimer>());

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Sitting));

            target.Skip();

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Standing));
        }

        [Test]
        public void Raise_an_event_when_the_state_is_changed()
        {
            var target = new StandUpModel(A.Fake<ITimer>());

            var raised = false;
            target.DeskStateChanged += (s, e) => raised = true;

            target.Skip();

            Assert.That(raised, Is.True);
        }
    }
}