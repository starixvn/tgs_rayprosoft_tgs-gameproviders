using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.PragmaticPlay;

namespace Tgs.GameProviders.ApiClients.PragmaticPlay
{
    internal class PragmaticPlayUtility : IPragmaticPlayUtility
    {
        public string Md5Hash(IDictionary<string, string> parameters, string md5Key)
        {
            var sortedParameters = new SortedDictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase);
            var values = sortedParameters.Select(p => $"{p.Key}={p.Value}");

            var original = $"{string.Join("&", values)}{md5Key}";

            using var hasher = MD5.Create();
            var data = Encoding.UTF8.GetBytes(original);

            var hash = hasher.ComputeHash(data);
            return hash.ToHexString();
        }
    }
}
