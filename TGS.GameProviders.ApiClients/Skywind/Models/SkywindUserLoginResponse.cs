using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Skywind.Models
{
    public record SkywindUserLoginResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("lastPasswordUpdate")]
        public DateTime LastPasswordUpdate { get; set; }
    }
}
