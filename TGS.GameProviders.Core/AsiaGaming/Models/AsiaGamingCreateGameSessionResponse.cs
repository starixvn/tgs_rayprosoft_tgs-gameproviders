using System.Xml.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    [XmlRoot]
    public record AsiaGamingCreateGameSessionResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string Error { get; init; }
    }
}