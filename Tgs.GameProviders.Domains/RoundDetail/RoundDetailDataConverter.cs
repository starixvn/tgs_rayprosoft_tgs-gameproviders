using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.RoundDetail
{
    public class RoundDetailDataConverter : JsonConverter<RoundDetailDataBase>
    {
        private static readonly Dictionary<string, Type> _TypeMapper = new()
        {
            { RoundDetailDataType.Url.ToString(), typeof(RoundDetailDataUrl) },
            { RoundDetailDataType.Table.ToString(), typeof(RoundDetailDataTable) },
            { RoundDetailDataType.Text.ToString(), typeof(RoundDetailDataText) },
            { RoundDetailDataType.Json.ToString(), typeof(RoundDetailDataJson) },
            { RoundDetailDataType.Xml.ToString(), typeof(RoundDetailDataXml) }
        };

        public override RoundDetailDataBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var dataType = root.GetProperty("type").ToString();
            if (!_TypeMapper.TryGetValue(dataType, out var type))
            {
                return null;
            }

            var o = JsonSerializer.Deserialize(root.GetRawText(), type, options);
            return (RoundDetailDataBase?)o;
        }

        public override void Write(Utf8JsonWriter writer, RoundDetailDataBase value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (RoundDetailDataBase)null, options);
                    break;

                default:
                    {
                        var type = value.GetType();
                        JsonSerializer.Serialize(writer, value, type, options);
                        break;
                    }
            }
        }
    }
}
