using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Services;

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
    }
}