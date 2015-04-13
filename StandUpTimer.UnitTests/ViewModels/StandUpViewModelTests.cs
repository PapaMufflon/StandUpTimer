using System;
using System.Windows;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Models;
using StandUpTimer.UnitTests.Models;
using StandUpTimer.ViewModels;

namespace StandUpTimer.UnitTests.ViewModels
{
    [TestFixture]
    public class StandUpViewModelTests
    {
        [Test]
        public void When_the_desk_state_of_the_model_changes_Set_the_according_image()
        {
            var model = Model;
            var target = new StandUpViewModel(model, A.Fake<IBringToForeground>());

            Assert.That(target.CurrentImage, Is.StringContaining("sitting"));

            model.Skip();

            Assert.That(target.CurrentImage, Is.StringContaining("standing"));
        }

        [Test]
        public void When_the_desk_state_ended_Bring_the_app_into_the_foreground()
        {
            var bringToForeground = A.Fake<IBringToForeground>();
            var target = new StandUpViewModel(Model, bringToForeground);

            target.DeskStateEnded();

            A.CallTo(() => bringToForeground.Now()).MustHaveHappened();
        }

        [Test]
        public void When_the_desk_state_ended_Show_the_OK_button()
        {
            var target = new StandUpViewModel(Model, A.Fake<IBringToForeground>());

            Assert.That(target.OkButtonVisibility, Is.EqualTo(Visibility.Collapsed));

            target.DeskStateEnded();

            Assert.That(target.OkButtonVisibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void When_a_new_desk_state_started_Hide_the_OK_button()
        {
            var target = new StandUpViewModel(Model, A.Fake<IBringToForeground>());

            target.DeskStateEnded();
            target.DeskStateStarted();

            Assert.That(target.OkButtonVisibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void You_cannot_end_a_desk_state_without_starting_it_first()
        {
            var target = new StandUpViewModel(Model, A.Fake<IBringToForeground>());

            target.DeskStateEnded();

            Assert.Throws<InvalidOperationException>(target.DeskStateEnded);
        }

        [Test]
        public void You_cannot_start_a_desk_state_when_it_already_runs()
        {
            var target = new StandUpViewModel(Model, A.Fake<IBringToForeground>());

            Assert.Throws<InvalidOperationException>(target.DeskStateStarted);
        }

        [Test]
        public void Skipping_a_desk_state_ends_it_and_starts_the_new_leg()
        {
            var target = new StandUpViewModel(Model, A.Fake<IBringToForeground>());

            Assert.That(target.CurrentImage, Is.StringContaining("sitting"));

            target.SkipCommand.Execute(null);

            Assert.That(target.CurrentImage, Is.StringContaining("standing"));
            Assert.That(target.OkButtonVisibility, Is.EqualTo(Visibility.Collapsed));
        }

        private static StandUpModel Model
        {
            get { return new StandUpModel(A.Fake<ITimer>(), StandUpModelTests.SittingTime, StandUpModelTests.StandingTime); }
        }
    }
}