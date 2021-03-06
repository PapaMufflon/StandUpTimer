﻿using System;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Common;
using StandUpTimer.Models;

namespace StandUpTimer.UnitTests.Models
{
    [TestFixture]
    public class StandUpModelTests
    {
        public static readonly TimeSpan StandingTime = TimeSpan.FromMinutes(20);
        public static readonly TimeSpan SittingTime = TimeSpan.FromHours(1);

        [Test]
        public void When_the_timer_ticks_Change_the_desk_state()
        {
            var timer = A.Fake<ITimer>();
            var target = new StandUpModel(timer, SittingTime, StandingTime);

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
            var target = new StandUpModel(timer, SittingTime, StandingTime);

            Assert.That(target.CurrentLeg, Is.EqualTo(SittingTime));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.CurrentLeg, Is.EqualTo(StandingTime));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.CurrentLeg, Is.EqualTo(SittingTime));
        }

        [Test]
        public void The_change_time_is_the_time_when_the_new_desk_state_ends()
        {
            var now = new DateTime(2014, 5, 11);

            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(now);

            var timer = A.Fake<ITimer>();
            var target = new StandUpModel(timer, SittingTime, StandingTime);

            Assert.That(target.ChangeTime, Is.EqualTo(now.Add(SittingTime)));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.ChangeTime, Is.EqualTo(now.Add(StandingTime)));

            timer.Tick += Raise.WithEmpty().Now;
            target.NewDeskStateStarted();

            Assert.That(target.ChangeTime, Is.EqualTo(now.Add(SittingTime)));
        }

        [Test]
        public void Skipping_is_the_same_as_a_timer_tick()
        {
            var target = new StandUpModel(A.Fake<ITimer>(), SittingTime, StandingTime);

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Sitting));

            target.Skip();

            Assert.That(target.DeskState, Is.EqualTo(DeskState.Standing));
        }

        [Test]
        public void Raise_an_event_when_the_state_is_changed()
        {
            var target = new StandUpModel(A.Fake<ITimer>(), SittingTime, StandingTime);

            var raised = false;
            target.DeskStateChanged += (s, e) => raised = true;

            target.Skip();

            Assert.That(raised, Is.True);
        }
    }
}