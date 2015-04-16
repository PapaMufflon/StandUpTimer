using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        public void You_can_send_the_current_desk_state_although_the_user_is_not_logged_in()
        {
            var client = new HttpClient(new FakeResponseHandler()) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, new CookieContainer());

            Assert.DoesNotThrow(() => target.SendDeskState(new Status()));
        }

        [Test]
        public async void To_log_in_you_need_the_AntiForgeryToken_first()
        {
            var fakeResponseHandler = new FakeResponseHandler();
            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, new CookieContainer());

            var result = await target.LogIn("username", "password".GetSecureString());

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void When_receiving_the_application_cookie_you_are_logged_in()
        {
            var cookieContainer = new CookieContainer();

            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(r => r.Method == HttpMethod.Get && r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")), new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />") });
            fakeResponseHandler.AddFakeResponse(r =>
                {
                    var formUrlEncodedContent = r.Content as FormUrlEncodedContent;

                    if (formUrlEncodedContent == null)
                        return false;

                    var queryString = formUrlEncodedContent.ReadAsStringAsync().Result;

                    if (r.Method == HttpMethod.Post &&
                        r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")) &&
                        queryString.Equals("Email=username&Password=password&__RequestVerificationToken=antiForgeryToken"))
                    {
                        cookieContainer.Add(new Cookie(".AspNet.ApplicationCookie", "whatever", "/", "localhost"));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                },
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />")
                });

            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, cookieContainer);

            var result = await target.LogIn("username", "password".GetSecureString());

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async void Wrong_credentials_do_not_log_you_in()
        {
            var cookieContainer = new CookieContainer();

            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(r => r.Method == HttpMethod.Get && r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")), new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />") });
            fakeResponseHandler.AddFakeResponse(r =>
            {
                var formUrlEncodedContent = r.Content as FormUrlEncodedContent;

                if (formUrlEncodedContent == null)
                    return false;

                var queryString = formUrlEncodedContent.ReadAsStringAsync().Result;

                if (r.Method == HttpMethod.Post &&
                    r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")) &&
                    queryString.Equals("Email=username&Password=password&__RequestVerificationToken=antiForgeryToken"))
                {
                    cookieContainer.Add(new Cookie(".AspNet.ApplicationCookie", "whatever", "/", "localhost"));
                    return true;
                }
                else
                {
                    return false;
                }
            },
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />")
                });

            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, cookieContainer);

            var result = await target.LogIn("wrongUsername", "wrongPassword".GetSecureString());

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void You_are_not_logged_in_when_the_server_returns_not_found()
        {
            var cookieContainer = new CookieContainer();

            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(r => r.Method == HttpMethod.Get && r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")), new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />") });

            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, cookieContainer);

            var result = await target.LogIn("wrongUsername", "wrongPassword".GetSecureString());

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async void For_logging_out_you_also_need_an_anti_forgery_token()
        {
            var cookieContainer = new CookieContainer();

            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(r => r.Method == HttpMethod.Get && r.RequestUri.Equals(new Uri(BaseUrl + "Account/Login")), new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""antiForgeryToken"" />") });
            fakeResponseHandler.AddFakeResponse(r => r.Method == HttpMethod.Post && r.RequestUri.Equals(new Uri(BaseUrl + "Account/LogOff")), new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) });

            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, cookieContainer);

            var result = await target.LogOut();

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async void Without_an_anti_forgery_token_you_cannot_log_out()
        {
            var cookieContainer = new CookieContainer();

            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(r => r.Method == HttpMethod.Post && r.RequestUri.Equals(new Uri(BaseUrl + "Account/LogOff")), new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) });

            var client = new HttpClient(fakeResponseHandler) { BaseAddress = new Uri(BaseUrl) };
            var target = new Server(client, cookieContainer);

            var result = await target.LogOut();

            Assert.That(result.Success, Is.False);
        }

        private class FakeResponseHandler : DelegatingHandler
        {
            private readonly Dictionary<Predicate<HttpRequestMessage>, HttpResponseMessage> fakeResponses = new Dictionary<Predicate<HttpRequestMessage>, HttpResponseMessage>();

            public void AddFakeResponse(Predicate<HttpRequestMessage> predicate, HttpResponseMessage responseMessage)
            {
                fakeResponses.Add(predicate, responseMessage);
            }

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return !fakeResponses.Any(x => x.Key(request))
                    ? new HttpResponseMessage(HttpStatusCode.NotFound)
                    : fakeResponses.Single(x => x.Key(request)).Value;
            }
        }
    }
}