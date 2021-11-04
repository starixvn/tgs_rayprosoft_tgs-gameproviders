using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.AsiaGaming
{
    internal class AsiaGamingUtility : IAsiaGamingUtility
    {
        public string Md5Hash(string original)
        {
            using var provider = new MD5CryptoServiceProvider();
            var data = Encoding.UTF8.GetBytes(original);

            var hash = provider.ComputeHash(data);
            return hash.ToHexString();
        }

        public string DesEncrypt(string original, string desKey)
        {
            if (string.IsNullOrEmpty(original))
            {
                return original;
            }

            using var des = new DESCryptoServiceProvider
            {
                IV = Encoding.UTF8.GetBytes(desKey),
                Key = Encoding.UTF8.GetBytes(desKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            using var ct = des.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = Encoding.UTF8.GetBytes(original);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}