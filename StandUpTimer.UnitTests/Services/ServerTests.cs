using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using StandUpTimer.Services;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.UnitTests.Services
{
    [TestFixture]
    public class ServerTests
    {
        private const string BaseUrl = "http://localhost/";

        [Test]
        public void You_can_send_the_current_desk_state_although_the_user_is_not_logged_in() // because the server object does not know if the user is logged in, it has no state.
        {
            var client = new HttpClient(new FakeResponseHandler()) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, new CookieContainer());

            Assert.DoesNotThrow(async () => await target.SendDeskState(new Status()));
        }

        [Test]
        public async void You_can_get_an_anti_forgery_token()
        {
            var cookieContainer = new CookieContainer();

            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(
                r => r.Method == HttpMethod.Get && r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")),
                new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />") });

            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, cookieContainer);

            var result = await target.TryGetAntiForgeryToken();

            Assert.That(result.AccountToken, Is.EqualTo("antiForgeryToken"));
        }
    }
}