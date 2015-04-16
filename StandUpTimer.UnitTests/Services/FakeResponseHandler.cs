using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace StandUpTimer.UnitTests.Services
{
    // taken from http://stackoverflow.com/a/22264503/453024
    public class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            fakeResponses.Add(uri, responseMessage);
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return fakeResponses.ContainsKey(request.RequestUri)
                ? fakeResponses[request.RequestUri]
                : new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
        }
    }
}