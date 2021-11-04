using System.Xml.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    [XmlRoot(ElementName = "result")]
    public record AsiaGamingCheckOrCreateAccountResponse
    {
        [XmlAttribute(AttributeName = "info")]
        public string Result { get; init; }

        [XmlAttribute(AttributeName = "msg")]
        public string Message { get; init; }
    }
}