using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models;

internal class IMESportsResultStatusConverter : JsonConverter<IMESportsResultStatus>
{
    public override IMESportsResultStatus
        Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return IMESportsResultStatus.None;
        }

        return Enum.TryParse<IMESportsResultStatus>(value, out var result) ? result : IMESportsResultStatus.Unknown;
    }

    public override void Write(
        Utf8JsonWriter writer,
        IMESportsResultStatus value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
