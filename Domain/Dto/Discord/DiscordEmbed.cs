using System.Text.Json.Serialization;

namespace Domain.Dto.Discord;

public class DiscordEmbed
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("color")]
    public int Color { get; set; }

    [JsonPropertyName("fields")]
    public List<DiscordEmbedField> Fields { get; set; } = new List<DiscordEmbedField>();

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
