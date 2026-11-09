using System.Text.Json.Serialization;

namespace Filmoteka.Models;

public class Video
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("key")]
    public string Key { get; set; } = "";

    [JsonPropertyName("site")]
    public string Site { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("official")]
    public bool Official { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
}
