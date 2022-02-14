using System;

namespace TGS.GameProviders.Client
{
    internal record GameUrlResponse
    {
        /// <summary>
        /// The URL
        /// </summary>
        public Uri? Url { get; set; }
    }
}
