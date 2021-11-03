using System;
using System.Security.Cryptography;
using System.Text;
using TGS.GameProviders.Core.TCG;

namespace TGS.GameProviders.ApiClients.TCG
{
    internal class TCGUtility : ITCGUtility
    {
        public string DESCDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;

            Byte[] toEncryptArray = Convert.FromBase64String(str);

            using (var dESCrypto = new DESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform cTransform = dESCrypto.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }

        public string DESCEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            using (var dESCrypto = new DESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform cTransform = dESCrypto.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        public string GetHash256(string datas, string hash_key)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();

            byte[] inputBytes = Encoding.UTF8.GetBytes(datas + hash_key);
            byte[] outputBytes = sha256.ComputeHash(inputBytes);
            return BitConverter.ToString(outputBytes).Replace("-", "").ToLower();
        }
    }
}
