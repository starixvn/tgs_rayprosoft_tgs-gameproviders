using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.VR;
using Tgs.GameProviders.Core.VR.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class VRApiClientUnitTest
{
    private const string Deskey = "PLFNNVZ68DF4V88XZ8B0688660Z2LBP2";
    private const string MerchantId = "WGWG";
    private const string Version = "1.0";
    private readonly Uri _apiUrl = new("https://pmws.vrbetapi.com/");

    [Theory]
    [InlineData("4ucmqplxb029", "397FE4DF0A2FF0E2820367F0E29CFCAC8F86", 1)]
    [InlineData("4ucmqplxb029", "397FE4DF0A2FF0E2820367F0E29CFCAC8F87", 0)]
    public async Task FundCheckingAsync(string playerCode, string orderId, int totalRecords)
    {
        var data = new VRFundCheckingRequest()
        {
            StartTime = DateTimeOffset.UtcNow.AddDays(-1).ToOffset(TimeSpan.FromHours(+8)).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            EndTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            SerialNumber = orderId,
            PlayerCode = playerCode,
            RecordPage = 0,
            RecordCountPerPage = 1
        };

        var client = Substitute.For<IVRApiClient>();

        _ = client.FundCheckingAsync(_apiUrl, MerchantId, Version, Deskey, data).Returns(ValueTask.FromResult<VRFundCheckingResponse>(new() { TotalRecords = totalRecords }));
        var response = await client.FundCheckingAsync(_apiUrl, MerchantId, Version, Deskey, data);

        Assert.Equal(totalRecords, response.TotalRecords);
    }

    /// <summary>
    /// Funds the in asynchronous.(GP->TGS)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FundInAsync()
    {
        var data = new VRFnudRequest
        {
            SerialNumber = Guid.NewGuid().ToString("N"),
            Type = 1,
            PlayerCode = "4ucmqplxb029",
            Amount = 1,
            CreateTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        var client = Substitute.For<IVRApiClient>();

        _ = await client.FundAsync(_apiUrl, MerchantId, Version, Deskey, VRAction.FundIn, data);
    }

    /// <summary>
    /// Funds the out asynchronous.(TGS->GP)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FundOutAsync()
    {
        var data = new VRFnudRequest
        {
            SerialNumber = Guid.NewGuid().ToString("N"),
            Type = 0,
            PlayerCode = "4ucmqplxb029",
            Amount = 1,
            CreateTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        var client = Substitute.For<IVRApiClient>();

        _ = await client.FundAsync(_apiUrl, MerchantId, Version, Deskey, VRAction.FundOut, data);
    }

    [Fact]
    public async Task GetBalanceAsync()
    {
        var data = new VRGetBalanceRequest
        {
            PlayerCode = "4ucmqplxb029"
        };

        var client = Substitute.For<IVRApiClient>();

        _ = await client.GetBalanceAsync(_apiUrl, MerchantId, Version, Deskey, data);
    }

    [Fact]
    public async Task GetGameRecordAsync()
    {
        var date = DateTimeOffset.UtcNow;

        var data = new VRGameRecordRequest
        {
            StartTime = date.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            EndTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            IsUpdateTime = true,
            Page = 0,
            Rows = 500
        };

        var client = Substitute.For<IVRApiClient>();

        _ = await client.GetGameRecordAsync(_apiUrl, MerchantId, Version, Deskey, data);
    }

    [Fact]
    public void GetGameURL()
    {
        var data = new VRGameUrlRequest()
        {
            PlayerCode = "4ucmqplxb029",
            LoginTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            IsCreateUser = "true",
            ChannelId = 0,
            ChannelType = 0
        };

        var client = Substitute.For<IVRApiClient>();

        _ = client.GetGameURL(_apiUrl, MerchantId, Version, Deskey, data);
    }

    [Fact]
    public async Task KickAsync()
    {
        var data = new VRKickRequest
        {
            PlayerCode = "4ucmqplxb029"
        };

        var client = Substitute.For<IVRApiClient>();

        _ = await client.KickAsync(_apiUrl, MerchantId, Version, Deskey, data);
    }
}
