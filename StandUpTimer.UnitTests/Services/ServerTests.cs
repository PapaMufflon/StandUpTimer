using System.Net.Http;
using NUnit.Framework;

namespace StandUpTimer.UnitTests.Services
{
    [TestFixture]
    public class ServerTests
    {
        [Test]
        public void Foo()
        {
            var client = new HttpClient(new FakeResponseHandler());
        }
    }
}