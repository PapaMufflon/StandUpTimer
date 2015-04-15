using System.Security;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Services;
using StandUpTimer.ViewModels;

namespace StandUpTimer.UnitTests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        [Test]
        public void Initially_the_user_is_logged_out()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            var target = new AuthenticationService(server, dialogPresenter);

            Assert.That(target.IsLoggedIn, Is.False);
        }

        [Test]
        public async void Changing_the_state_when_logged_out_logs_in()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);
            A.CallTo(() => server.LogIn(A<string>._, A<SecureString>._)).Returns(true);

            var target = new AuthenticationService(server, dialogPresenter);

            await target.ChangeState();

            Assert.That(target.IsLoggedIn, Is.True);
        }

        [Test]
        public async void You_can_cancel_logging_in()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(false);
            A.CallTo(() => server.LogIn(A<string>._, A<SecureString>._)).Returns(true);

            var target = new AuthenticationService(server, dialogPresenter);

            await target.ChangeState();

            Assert.That(target.IsLoggedIn, Is.False);
        }

        [Test]
        public async void Wrong_credentials_lets_you_retry()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);
            A.CallTo(() => server.LogIn(A<string>._, A<SecureString>._)).ReturnsNextFromSequence(false, true);

            var target = new AuthenticationService(server, dialogPresenter);

            await target.ChangeState();

            Assert.That(target.IsLoggedIn, Is.True);
        }

        [Test]
        public async void Changing_the_state_when_logged_in_logs_out()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);
            A.CallTo(() => server.LogIn(A<string>._, A<SecureString>._)).Returns(true);
            A.CallTo(() => server.LogOut()).Returns(true);

            var target = new AuthenticationService(server, dialogPresenter);

            await target.ChangeState();
            await target.ChangeState();

            Assert.That(target.IsLoggedIn, Is.False);
        }

        [Test]
        public async void Logging_out_is_automatically_retried()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);
            A.CallTo(() => server.LogIn(A<string>._, A<SecureString>._)).Returns(true);
            A.CallTo(() => server.LogOut()).ReturnsNextFromSequence(false, false, true);

            var target = new AuthenticationService(server, dialogPresenter);

            await target.ChangeState();
            await target.ChangeState();

            Assert.That(target.IsLoggedIn, Is.False);
        }

        [Test]
        public async void Logging_out_is_automatically_retried_5_times()
        {
            var server = A.Fake<IServer>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);
            A.CallTo(() => server.LogIn(A<string>._, A<SecureString>._)).Returns(true);
            A.CallTo(() => server.LogOut()).ReturnsNextFromSequence(false, false, false, false, false, true);

            var target = new AuthenticationService(server, dialogPresenter);

            await target.ChangeState();
            await target.ChangeState();

            Assert.That(target.IsLoggedIn, Is.True);
        }
    }
}