using System.Security;
using System.Threading;
using System.Threading.Tasks;
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
        public void The_authentication_status_is_queried_at_creation_time()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.GetStatisticsPage()).Returns("logged in");

            var target = new AuthenticationService(server);

            while (!target.IsLoggedIn)
            {
                // uh - oh, it's asynchronous...
                Thread.Sleep(100);
            }

            Assert.That(target.IsLoggedIn, Is.True);
        }

        [Test]
        public async void Without_an_anti_forgery_token_You_cannot_log_in()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.TryGetAntiForgeryToken()).Returns(Task.FromResult(new AccountTokenResult { Success = false }));

            var target = new AuthenticationService(server);

            var result = await target.LogIn("username", "password".GetSecureString());

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void You_cannot_log_in_when_there_are_communication_difficulties()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.TryGetAntiForgeryToken()).Returns(Task.FromResult(new AccountTokenResult { Success = true}));
            A.CallTo(() => server.TrySendCredentials(A<string>._, A<SecureString>._, A<string>._)).Returns(Task.FromResult(false));

            var target = new AuthenticationService(server);

            var result = await target.LogIn("username", "password".GetSecureString());

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void You_are_loggeed_in_when_you_got_the_application_cookie()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.TryGetAntiForgeryToken()).Returns(Task.FromResult(new AccountTokenResult { Success = true }));
            A.CallTo(() => server.TrySendCredentials(A<string>._, A<SecureString>._, A<string>._)).Returns(Task.FromResult(true));
            A.CallTo(() => server.ContainsCookie(A<string>._)).Returns(true);

            var target = new AuthenticationService(server);

            Assert.That(target.IsLoggedIn, Is.False);

            var result = await target.LogIn("username", "password".GetSecureString());

            Assert.That(result.Success, Is.True);
            Assert.That(target.IsLoggedIn, Is.True);
            A.CallTo(() => server.WriteCookiesToDisk()).MustHaveHappened();
        }

        [Test]
        public async void Without_an_anti_forgery_token_You_cannot_log_off()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.TryGetAntiForgeryToken()).Returns(Task.FromResult(new AccountTokenResult { Success = false }));

            var target = new AuthenticationService(server);

            var result = await target.LogOff();

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void You_cannot_log_off_when_there_are_communication_difficulties()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.TryGetAntiForgeryToken()).Returns(Task.FromResult(new AccountTokenResult { Success = true }));
            A.CallTo(() => server.TryLogOff(A<string>._)).Returns(Task.FromResult(false));

            var target = new AuthenticationService(server);

            var result = await target.LogOff();

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void Logging_off_changes_the_corresponding_property()
        {
            var server = A.Fake<IServer>();

            A.CallTo(() => server.GetStatisticsPage()).Returns("logged in");

            A.CallTo(() => server.TryGetAntiForgeryToken()).Returns(Task.FromResult(new AccountTokenResult { Success = true }));
            A.CallTo(() => server.TryLogOff(A<string>._)).Returns(Task.FromResult(true));

            var target = new AuthenticationService(server);

            while (!target.IsLoggedIn)
                Thread.Sleep(100);

            Assert.That(target.IsLoggedIn, Is.True);

            var result = await target.LogOff();

            Assert.That(result.Success, Is.True);
            Assert.That(target.IsLoggedIn, Is.False);
        }
    }
}