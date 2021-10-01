using System.Text.Json;

namespace TGS.GameProviders.ApiClients.VR
{
    public interface IVRUtility
    {
        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        public string AesDecrypt(string str, string key, bool isDecodeUrl = true);

        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        public string AesEncrypt(string str, string key, bool isEncodeUrl = true);

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public string GetAction(VRAction action);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null);
    }
}
