using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.DreamGame;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class DreamGameApiClientUnitTest
    {
        private readonly string _agentName = "DGTE0105CO";
        private readonly string _apiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae";
        private readonly Uri _apiUrl = new("http://api.dg99web.com");

        public async Task Actual_Login()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.LoginAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", PlatformType.Desktop);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(0, actual.CodeId);
        }

        public async Task Actual_SignUp()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.SignUpAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", "CNY");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual);
        }

        [Test]
        public async Task Login()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            var expected = new LoginResult(true, 0, new("https://www.google.com/"));
            _ = client.LoginAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", PlatformType.Desktop).Returns(ValueTask.FromResult(expected));
            var actual = await client.LoginAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", PlatformType.Desktop);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.CodeId, actual.CodeId);
            Assert.AreEqual(expected.Url, actual.Url);
        }

        [Test]
        public async Task SignUp()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            _ = client.SignUpAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", "CNY").Returns(ValueTask.FromResult(0));
            var actual = await client.SignUpAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", "CNY");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual);
        }
    }
}
