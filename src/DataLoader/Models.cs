using System.Text;
using System.Text.Json.Serialization;

namespace DataLoader;

public class SpellData
{
    [JsonPropertyName("format")]
    public int Format { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("nameSpace")]
    public string NameSpace { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("spells")]
    public List<Spell> Spells { get; set; }
}

public class Spell
{
    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("lvl")]
    public int Level { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("book")]
    public string Book { get; set; }

    [JsonPropertyName("schools")]
    public string Schools { get; set; }

    [JsonPropertyName("spheres")]
    public string Spheres { get; set; }

    [JsonPropertyName("rng")]
    public string Range { get; set; }

    [JsonPropertyName("compo")]
    public string Components { get; set; }

    [JsonPropertyName("dur")]
    public string Duration { get; set; }

    [JsonPropertyName("castime")]
    public string CastingTime { get; set; }

    [JsonPropertyName("aoe")]
    public string AreaOfEffect { get; set; }

    [JsonPropertyName("saving")]
    public string SavingThrow { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Level: {Level}");
        sb.AppendLine($"Type: {Type}");
        sb.AppendLine($"Book: {Book}");
        sb.AppendLine($"Schools: {Schools}");
        sb.AppendLine($"Spheres: {Spheres}");
        sb.AppendLine($"Range: {Range}");
        sb.AppendLine($"Components: {Components}");
        sb.AppendLine($"Duration: {Duration}");
        sb.AppendLine($"Casting Time: {CastingTime}");
        sb.AppendLine($"Area of Effect: {AreaOfEffect}");
        sb.AppendLine($"Saving Throw: {SavingThrow}");
        sb.AppendLine();
        sb.AppendLine("Description:");
        sb.AppendLine(StripHtml(Body));
        return sb.ToString();
    }

    private string StripHtml(string html)
    {
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty)
            .Replace("&nbsp;", " ")
            .Replace("&quot;", "\"")
            .Replace("&#39;", "'")
            .Replace("&amp;", "&");
    }
}
