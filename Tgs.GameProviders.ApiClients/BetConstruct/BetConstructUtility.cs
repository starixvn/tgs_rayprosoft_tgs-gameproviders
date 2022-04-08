using System;
using System.Security.Cryptography;
using System.Text;
using Tgs.GameProviders.Core.BetConstruct;

namespace Tgs.GameProviders.ApiClients.BetConstruct
{
    internal class BetConstructUtility : IBetConstructUtility
    {
        private static readonly MD5 _Md5;

        static BetConstructUtility()
        {
            _Md5 = MD5.Create();
        }

        public string MD5Hash(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = _Md5.ComputeHash(data);

            return Convert.ToHexString(hash).ToLower();
        }
    }
}
