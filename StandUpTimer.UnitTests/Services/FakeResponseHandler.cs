using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StandUpTimer.UnitTests.Services
{
    public class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<Predicate<HttpRequestMessage>, HttpResponseMessage> fakeResponses = new Dictionary<Predicate<HttpRequestMessage>, HttpResponseMessage>();

        public void AddFakeResponse(Predicate<HttpRequestMessage> predicate, HttpResponseMessage responseMessage)
        {
            fakeResponses.Add(predicate, responseMessage);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Run(() => !fakeResponses.Any(x => x.Key(request))
                ? new HttpResponseMessage(HttpStatusCode.NotFound)
                : fakeResponses.Single(x => x.Key(request)).Value);
        }
    }
}