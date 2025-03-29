using System.Text.Json.Serialization;
using System.Text.Json;

namespace DndTest.Services;

public class TikaResponse
{
    [JsonPropertyName("Content-Type")]
    public string ContentType { get; set; }

    [JsonPropertyName("X-TIKA:content")]
    public string Content { get; set; }

    [JsonPropertyName("dc:creator")]
    public string Creator { get; set; }

    [JsonPropertyName("meta:last-author")]
    public string LastAuthor { get; set; }

    [JsonPropertyName("dcterms:created")]
    [JsonConverter(typeof(TikaDateTimeConverter))]
    public DateTime? Created { get; set; }

    [JsonPropertyName("dcterms:modified")]
    [JsonConverter(typeof(TikaDateTimeConverter))]
    public DateTime? Modified { get; set; }

    [JsonPropertyName("meta:print-date")]
    [JsonConverter(typeof(TikaDateTimeConverter))]
    public DateTime? PrintDate { get; set; }

    [JsonPropertyName("meta:page-count")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? PageCount { get; set; }

    [JsonPropertyName("meta:word-count")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? WordCount { get; set; }

    [JsonPropertyName("meta:character-count")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? CharacterCount { get; set; }

    [JsonPropertyName("meta:character-count-with-spaces")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? CharacterCountWithSpaces { get; set; }

    [JsonPropertyName("meta:line-count")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? LineCount { get; set; }

    [JsonPropertyName("meta:paragraph-count")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? ParagraphCount { get; set; }

    [JsonPropertyName("extended-properties:Application")]
    public string Application { get; set; }

    [JsonPropertyName("extended-properties:AppVersion")]
    public string AppVersion { get; set; }

    [JsonPropertyName("cp:revision")]
    public string Revision { get; set; }

    [JsonPropertyName("extended-properties:Template")]
    public string Template { get; set; }

    [JsonPropertyName("extended-properties:DocSecurityString")]
    public string DocSecurityString { get; set; }

    [JsonPropertyName("extended-properties:TotalTime")]
    [JsonConverter(typeof(TikaIntConverter))]
    public int? TotalTime { get; set; }

    [JsonPropertyName("X-TIKA:Parsed-By")]
    public List<string> ParsedBy { get; set; }

    [JsonPropertyName("dc:publisher")]
    public string Publisher { get; set; }

    [JsonPropertyName("Content-Length")]
    [JsonConverter(typeof(TikaLongConverter))]
    public long? ContentLength { get; set; }

    // Store all other properties that aren't explicitly mapped
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalData { get; set; }
}

// Custom converters for Tika's string-encoded values
public class TikaIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (int.TryParse(stringValue, out var intValue))
            {
                return intValue;
            }
            return null;
        }
        return reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

public class TikaLongConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (long.TryParse(stringValue, out var longValue))
            {
                return longValue;
            }
            return null;
        }
        return reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

public class TikaDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        var stringValue = reader.GetString();
        if (DateTime.TryParse(stringValue, out var dateValue))
        {
            return dateValue;
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("o"));
        else
            writer.WriteNullValue();
    }
}