using System.Xml.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    [XmlRoot(elementName: "PlayerTicketResponse")]
    public record AsiaGamingCreateGameSessionResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string Error { get; init; } = default!;
    }
}
