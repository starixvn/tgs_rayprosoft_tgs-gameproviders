using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models
{
    public class SBONumberToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var value)
                ? value.ToString(CultureInfo.InvariantCulture)
                : reader.GetString();

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }
}
