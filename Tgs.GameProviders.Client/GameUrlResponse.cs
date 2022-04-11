using System;

namespace Tgs.GameProviders.Client
{
    internal record GameUrlResponse
    {
        /// <summary>
        /// The URL
        /// </summary>
        public Uri? Url { get; set; }
    }
}
