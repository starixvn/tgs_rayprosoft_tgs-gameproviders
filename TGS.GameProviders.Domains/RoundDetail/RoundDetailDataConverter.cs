using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    internal class RoundDetailDataConverter : JsonConverter<RoundDetailDataBase>
    {
        private static readonly Dictionary<int, Type> _TypeMapper = new()
        {
            { (int)RoundDetailDataType.Url, typeof(RoundDetailDataUrl) },
            { (int)RoundDetailDataType.Table, typeof(RoundDetailDataTable) },
            { (int)RoundDetailDataType.Text, typeof(RoundDetailDataText) },
            { (int)RoundDetailDataType.Json, typeof(RoundDetailDataJson) },
            { (int)RoundDetailDataType.Xml, typeof(RoundDetailDataXml) }
        };

        public override RoundDetailDataBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var dataType = root.GetProperty("type").GetInt32();
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
                    JsonSerializer.Serialize(writer, (RoundDetailDataBase?)null, options);
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
