using System.Security.Cryptography;
using System.Text;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.BigGaming
{
    internal class BigGamingUtility : IBigGamingUtility
    {
        public string GetHashMD5(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                var hash = cryptoServiceProvider.ComputeHash(data);
                return hash.ToHexString().ToUpper();
            }
        }

        public string GetSecretCode(string agentLoginIdPassword)
        {
            var data = Encoding.UTF8.GetBytes(agentLoginIdPassword);
            using (var cryptoServiceProvider = new SHA1CryptoServiceProvider())
            {
                var hash = cryptoServiceProvider.ComputeHash(data);
                return hash.ToBase64String();
            }
        }
    }
}
