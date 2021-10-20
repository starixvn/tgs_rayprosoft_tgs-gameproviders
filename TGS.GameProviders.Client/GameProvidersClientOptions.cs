using System;
using System.Text.Json;

namespace TGS.GameProviders.Client
{
    public class GameProvidersClientOptions
    {
        public Uri BaseUrl { get; set; } = default!;

        public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    }
}
