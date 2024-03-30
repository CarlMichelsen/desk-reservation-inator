using System.Text.Json.Serialization;

namespace Domain.Dto.Discord;

public class DiscordWebhookPayload
{
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; init; } = "https://cdn.discordapp.com/avatars/118336122832158720/c5db031af3292048af8497519af19c8c.webp";

    [JsonPropertyName("embed")]
    public DiscordEmbed? Embed { get; init; }
}
