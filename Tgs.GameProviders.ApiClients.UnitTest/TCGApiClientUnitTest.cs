using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.TCG;
using Tgs.GameProviders.Core.TCG.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class TCGApiClientUnitTest
    {
        private const string DesKey = "EuWEfLac";
        private const string MerchantCode = "pegacnys";
        private const string Sha256Key = "h2f69FzdSmMZzrdE";
        private const string Vnd_DesKey = "EuWEfLac";
        private const string Vnd_MerchantCode = "pegacnys";
        private const string Vnd_Sha256Key = "h2f69FzdSmMZzrdE";
        private readonly Uri _apiUrl = new("http://www.connect6play.com/doBusiness.do");

        [Test]
        public void Confirm_Reserve_API_圈存确认接口()
        {
            var data = "gcvEsDpM%2BnnAf6WLWbp35ja%2FFQMMMEWpxplsvNumnA%2F%2BfDMzS9Gn3PW4FSWYmSlIJUI14VD61hBDuaXFOtRdeVocrr1KEmtX4M9AUc6meyDvnKA6avO4PDFiIRCXM%2BehwqJ1eb3PlIgkIMLvMJRtyv2TKLWJuf56OtG%2BQMesTwu2HnC66HcbF%2BHVm5dVKSdYb5XqUzwfMxWQ%2BR6wlLVkNEqKgCVjDqcRqEddwwyPKVASxMNhkL4ugVw7R%2FapXYtYMmGtqllm3vf58WKlHqYkmXdAWOFU7IsJN%2B2BpI3fqjngyBexZPv2x9lqv4XFZPyCGXeO6X1gw2LQsARxXKTKZUJzN%2FfltD4dbvqaOSjiMre%2BrNvmz0da4pCgn5mAVy4VFGYn606dLFZjjpXuih23fJz7pJFVKxDKDl%2BRYqdQACxFrIMjDxxhCJKmRRna7NPIcChCaDhfvamwsdGpOjAZXn1njCbOBSbrn1pbx1572rDh1ZuXVSknWG%2BV6lM8HzMVH9Z3Orh6g1bN00AXbt1uMahHXcMMjylQEsTDYZC%2BLoFcO0f2qV2LWDJhrapZZt73%2BfFipR6mJJl3QFjhVOyLCTDvfyiHtRVM4MgXsWT79sfZar%2BFxWT8goQ0sEix89rU";
            var decodeSign = HttpUtility.UrlDecode(data);
            var e = DESCDecrypt(decodeSign, "PjmKWpPX");
            Assert.IsNotNull(e);
        }

        [Test]
        public void Credit_API_增加玩家帐户金额接口()
        {
            var data = "HMD91QE8UExQayUwqQY4StVPn8sQTKaHpw%2FSyxGFSQF8ElhEeeWzGf70UpKzjOSOcw%2FnSOcbxcBafPmHtyehLkD0lUESzqAdG6xy5rSX5g6vUgRdgDDowOT16LpqW25vXzo84EW60tOuAhPIguu81lqHZKWWCtlLd0BY4VTsiwmn6IOAjiqxRn9enjC9pauYHVNgExMiJ9GWvrs3mOkivrus1nzKJm3y%2FF2Tg4J8GRs7sNfJhGg0BIqZMpNDFBiwf16eML2lq5g7AD40%2B2yqwIpXsPfHwGaXmbjGxjCTNsCejKME5DF%2BeFk%2FrCeasMGMcdBlgWCcozBu%2Bpo5KOIyt2GWek2ap0i9Mjzjl2abIQ0Fz27rVI1K1KpvQaxH%2F1Jkc0WIExYWo8yPONHc51JQd5fc%2FoMLcMg%2B8HoBnLokHxBKHyfmI8mNkDX2YtgP0FAkNnXnWrPXvAzJJIb2eNjGizTw%2BlDjLCPfuFmGYAE%2B3SyNFncvqgXQcxRmJ%2BtOnSxWY46V7oodt3yc%2B6SRVSsQyg5fkWKnUAAsRayDIw8cYQi4WYZgAT7dLFnxhuLYp1HZsLHRqTowGV5KKoiHM1UmaVYGB4BIRtE69KgVlZ0xGKoPVhXKX56OYWMbp4WAXNQAqFltFIgDXnMoOPdj%2BXbRyG9%2BnpoJogPcqxVVlfigsf35EtM4gvAmCV2FmdCRwIrmrQfXveu7s6yPBbn6MJmYWZnck5VD1Hgf2RRrz%2Bh7c5N4XQy%2FxC7LghVG5YroCNQgrkcOqgwGoBMxQplssTQRw8aFeMIORjb98qXSUfKSyad%2BZoqxgxwGl8cza1rSxRNvEPWQWfR2XFmPd1RvylD3JDFCmWyxNBHDzji1oIxRUGZEt8E5EhBIhcPVqjkgZnIBnGtGMSbM2O37cfR%2FuyyrDJwXxZKjHpQv6h7qaBa4hJnXvoU3KTPUKr9QcF1NJUGaLh3ZGz1ywRB5DW0A3XopwN6QCVMnwi6R2jp%2FROnz3nDHvTuJKnNB4Q%3D%3D";
            var decodeSign = HttpUtility.UrlDecode(data);
            var e = DESCDecrypt(decodeSign, "PjmKWpPX");
            Assert.IsNotNull(e);
        }

        [Test]
        public async Task GetLaunchGameByELOTTOAsync()
        {
            var data = new TCGLaunchGameByELOTTORequest()
            {
                UserName = "4ucmqplxb029",
                Platform = "html5",
                GameMode = "2",
                GameCode = "Lobby",
                Language = "CN",
                LotteryBetMode = "Elott_Traditional",
                View = "Lobby"
            };

            var client = Substitute.For<ITCGApiClient>();

            _ = await client.LaunchGameAsync(_apiUrl, MerchantCode, DesKey, Sha256Key, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLaunchGameByLOTTOVNAsync()
        {
            var data = new TCGLaunchGameByLOTTOVNRequest()
            {
                UserName = "4ucmqplxb029",
                Platform = "WEB",
                GameMode = "420",
                GameCode = "lobby",
                Language = "CN",
                LotteryBetMode = "Elott",
                View = "lobby"
            };

            var client = Substitute.For<ITCGApiClient>();

            _ = await client.LaunchGameAsync(_apiUrl, Vnd_MerchantCode, Vnd_DesKey, Vnd_Sha256Key, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLaunchGameBySEAAsync()
        {
            var data = new TCGLaunchGameBySEARequest()
            {
                UserName = "4ucmqplxb029",
                Platform = "html5",
                GameMode = "384",
                GameCode = "lobby",
                Language = "CN",
                LotteryBetMode = "SEA_Tradition",
                View = "Lobby"
            };

            var client = Substitute.For<ITCGApiClient>();

            _ = await client.LaunchGameAsync(_apiUrl, MerchantCode, DesKey, Sha256Key, data).ConfigureAwait(false);
        }

        [Test]
        public async Task KickOutLottoPlayerAsync()
        {
            var data = new TCGKickOutLottoPlayerRequest()
            {
                UserName = "4ucmqplxb029"
            };

            var client = Substitute.For<ITCGApiClient>();

            _ = await client.KickOutLottoPlayerAsync(_apiUrl, MerchantCode, DesKey, Sha256Key, data).ConfigureAwait(false);
        }

        private static string? DESCDecrypt(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var toEncryptArray = Convert.FromBase64String(data);

            using var dESCrypto = DES.Create();
            dESCrypto.Key = Encoding.UTF8.GetBytes(key);
            dESCrypto.Mode = CipherMode.ECB;
            dESCrypto.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = dESCrypto.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
