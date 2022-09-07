using System.Xml.Serialization;

namespace Tgs.GameProviders.Core.AsiaGaming.Models;

[XmlRoot(ElementName = "result")]
public record AsiaGamingCheckOrCreateAccountResponse
{
    [XmlAttribute(AttributeName = "info")]
    public string Result { get; init; } = default!;

    [XmlAttribute(AttributeName = "msg")]
    public string Message { get; init; } = default!;
}
