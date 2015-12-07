using System.Security;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Services;
using StandUpTimer.ViewModels;

namespace StandUpTimer.UnitTests.Services
{
    [TestFixture]
    public class ChangeAuthenticationStateCommandTests
    {
        [Test]
        public void Changing_the_state_when_logged_out_logs_in()
        {
            var authenticationService = A.Fake<IAuthenticationService>();
            var dialogPresenter = A.Fake<IDialogPresenter>();
            var loggedIn = false;

            A.CallTo(() => authenticationService.IsLoggedIn).ReturnsLazily(() => loggedIn);
            A.CallTo(() => authenticationService.LogIn(A<string>._, A<SecureString>._))
                .Invokes(() =>
                {
                    loggedIn = true;
                })
                .Returns(Task.FromResult(new CommunicationResult { Success = true }));

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);

            var target = new ChangeAuthenticationStateCommand(authenticationService, dialogPresenter, new LoginViewModel());

            target.Execute(null);

            Assert.That(loggedIn, Is.True);
        }

        [Test]
        public void You_can_cancel_logging_in()
        {
            var authenticationService = A.Fake<IAuthenticationService>();
            var dialogPresenter = A.Fake<IDialogPresenter>();

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(false);
            A.CallTo(() => authenticationService.IsLoggedIn).Returns(false);
            A.CallTo(() => authenticationService.LogIn(A<string>._, A<SecureString>._)).Returns(Task.FromResult(new CommunicationResult { Success = true }));

            var target = new ChangeAuthenticationStateCommand(authenticationService, dialogPresenter, new LoginViewModel());

            target.Execute(null);

            Assert.Pass();
        }

        [Test]
        public void Wrong_credentials_lets_you_retry()
        {
            var authenticationService = A.Fake<IAuthenticationService>();
            var dialogPresenter = A.Fake<IDialogPresenter>();
            var loginViewModel = new LoginViewModel { Username = "user" };
            var timesInvoked = 0;
            var loggedIn = false;

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._))
                .Invokes(() =>
                {
                    timesInvoked++;

                    loginViewModel.Password = (timesInvoked > 1
                        ? "correctPassword"
                        : "wrongPassword").GetSecureString(false);
                })
                .Returns(true);
            A.CallTo(() => authenticationService.IsLoggedIn).ReturnsLazily(() => loggedIn);
            A.CallTo(() => authenticationService.LogIn("user", A<SecureString>.That.Matches(x => x.Length == 13))).Returns(Task.FromResult(new CommunicationResult { Success = false }));
            A.CallTo(() => authenticationService.LogIn("user", A<SecureString>.That.Matches(x => x.Length == 15))) // cannot compare two SecureStrings...
                .Invokes(() =>
                {
                    loggedIn = true;
                })
                .Returns(Task.FromResult(new CommunicationResult { Success = true }));

            var target = new ChangeAuthenticationStateCommand(authenticationService, dialogPresenter, loginViewModel);

            target.Execute(null);

            Assert.That(loggedIn, Is.True);
        }

        [Test]
        public void Changing_the_state_when_logged_in_logs_out()
        {
            var authenticationService = A.Fake<IAuthenticationService>();
            var dialogPresenter = A.Fake<IDialogPresenter>();
            var loggedIn = true;

            A.CallTo(() => authenticationService.IsLoggedIn).ReturnsLazily(() => loggedIn);
            A.CallTo(() => authenticationService.LogOff())
                .Invokes(() =>
                {
                    loggedIn = false;
                })
                .Returns(Task.FromResult(new CommunicationResult { Success = true }));

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);

            var target = new ChangeAuthenticationStateCommand(authenticationService, dialogPresenter, new LoginViewModel());

            target.Execute(null);

            Assert.That(loggedIn, Is.False);
        }

        [Test]
        public void Logging_out_is_automatically_retried()
        {
            var authenticationService = A.Fake<IAuthenticationService>();
            var dialogPresenter = A.Fake<IDialogPresenter>();
            var timesInvoked = 0;
            var loggedIn = true;

            A.CallTo(() => authenticationService.IsLoggedIn).ReturnsLazily(() => loggedIn);
            A.CallTo(() => authenticationService.LogOff())
                .Invokes(() =>
                {
                    timesInvoked++;

                    if (timesInvoked > 2)
                        loggedIn = false;
                })
                .ReturnsNextFromSequence(
                    Task.FromResult(new CommunicationResult {Success = false}),
                    Task.FromResult(new CommunicationResult {Success = false}),
                    Task.FromResult(new CommunicationResult {Success = true}));

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);

            var target = new ChangeAuthenticationStateCommand(authenticationService, dialogPresenter, new LoginViewModel());

            target.Execute(null);

            Assert.That(loggedIn, Is.False);
        }

        [Test]
        public void Logging_out_is_automatically_retried_only_5_times()
        {
            var authenticationService = A.Fake<IAuthenticationService>();
            var dialogPresenter = A.Fake<IDialogPresenter>();
            var timesInvoked = 0;
            var loggedIn = true;

            A.CallTo(() => authenticationService.IsLoggedIn).ReturnsLazily(() => loggedIn);
            A.CallTo(() => authenticationService.LogOff())
                .Invokes(() =>
                {
                    timesInvoked++;

                    if (timesInvoked > 5)
                        loggedIn = false;
                })
                .ReturnsNextFromSequence(
                    Task.FromResult(new CommunicationResult { Success = false }),
                    Task.FromResult(new CommunicationResult { Success = false }),
                    Task.FromResult(new CommunicationResult { Success = false }),
                    Task.FromResult(new CommunicationResult { Success = false }),
                    Task.FromResult(new CommunicationResult { Success = false }),
                    Task.FromResult(new CommunicationResult { Success = true }));

            A.CallTo(() => dialogPresenter.ShowModal(A<IDialogViewModel>._)).Returns(true);

            var target = new ChangeAuthenticationStateCommand(authenticationService, dialogPresenter, new LoginViewModel());

            target.Execute(null);

            Assert.That(loggedIn, Is.True);
        }
    }
}