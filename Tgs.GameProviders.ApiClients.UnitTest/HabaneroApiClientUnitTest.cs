using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.Habanero.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class HabaneroApiClientUnitTest
{
    private const string ApiKey = "349F896E-DBC8-4A9D-963D-EFC921AD19FC";
    private const string BrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a";
    private readonly Uri _apiUrl = new("https://ws-test.insvr.com");

    [Test]
    public async Task LogoutAsync()
    {
        var request = new HabaneroLogoutRequest
        {
            BrandId = BrandId,
            ApiKey = ApiKey,
            PlayerCode = "4ucmqplxb029"
        };

        var client = Substitute.For<IHabaneroApiClient>();

        _ = await client
            .LogoutAsync(_apiUrl, request)
            .ConfigureAwait(false);
    }
}
