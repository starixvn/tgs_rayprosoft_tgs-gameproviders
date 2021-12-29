using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.AsiaGaming
{
    internal class AsiaGamingUtility : IAsiaGamingUtility
    {
        public string DesEncrypt(string original, string desKey)
        {
            if (string.IsNullOrEmpty(original))
            {
                return original;
            }

            using var des = DES.Create();
            des.IV = Encoding.UTF8.GetBytes(desKey);
            des.Key = Encoding.UTF8.GetBytes(desKey);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            using var ct = des.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = Encoding.UTF8.GetBytes(original);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        public string GetQueryString<TRequest>(TRequest request, string separator)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(request, jsonSerializerOptions);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            var data = items!.Select(pair => $"{pair.Key}={pair.Value}");
            return string.Join(separator, data);
        }

        public string Md5Hash(string original)
        {
            using var hasher = MD5.Create();
            var data = Encoding.UTF8.GetBytes(original);

            var hash = hasher.ComputeHash(data);
            return hash.ToHexString();
        }
    }
}
