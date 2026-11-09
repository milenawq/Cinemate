using System.Text.Json.Serialization;

namespace Filmoteka.Models;

public class GenreListResponse
{
    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; } = new();
}
