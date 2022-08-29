using System.Xml.Serialization;

namespace Tgs.GameProviders.Core.AsiaGaming.Models;

[XmlRoot(elementName: "PlayerTicketResponse")]
public record AsiaGamingCreateGameSessionResponse
{
    [XmlElement(ElementName = "ResponseCode")]
    public string Error { get; init; } = default!;
}
