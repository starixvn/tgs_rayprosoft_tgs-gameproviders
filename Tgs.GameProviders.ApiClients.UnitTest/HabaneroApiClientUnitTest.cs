using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.Habanero;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.Habanero.Models;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class HabaneroApiClientUnitTest
{
    private readonly string _apiKey = "349F896E-DBC8-4A9D-963D-EFC921AD19FC";
    private readonly Uri _apiUrl = new Uri("https://ws-test.insvr.com");
    private readonly string _brandId = "a5df0ada-7b6a-eb11-9889-00155db5435a";
    private readonly IHabaneroApiClient _client;

    public HabaneroApiClientUnitTest()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<HabaneroApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        //_client = new HabaneroApiClient(httpClient, logger, tgsLog);
        _client = Substitute.For<IHabaneroApiClient>();
    }

    [Test]
    public async Task LogoutAsync()
    {
        var request = new HabaneroLogoutRequest
        {
            BrandId = _brandId,
            ApiKey = _apiKey,
            PlayerCode = "4ucmqplxb029"
        };

        var response = await _client
            .LogoutAsync(_apiUrl, request)
            .ConfigureAwait(false);
    }
}
