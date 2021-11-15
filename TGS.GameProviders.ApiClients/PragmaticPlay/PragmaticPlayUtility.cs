using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.PragmaticPlay;

namespace TGS.GameProviders.ApiClients.PragmaticPlay
{
    internal class PragmaticPlayUtility : IPragmaticPlayUtility
    {
        public string Md5Hash(IDictionary<string, string> parameters, string md5Key)
        {
            var sortedParameters = new SortedDictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase);
            var values = sortedParameters.Select(p => $"{p.Key}={p.Value}");

            var original = $"{string.Join("&", values)}{md5Key}";

            using var provider = new MD5CryptoServiceProvider();
            var data = Encoding.UTF8.GetBytes(original);

            var hash = provider.ComputeHash(data);
            return hash.ToHexString();
        }
    }
}
