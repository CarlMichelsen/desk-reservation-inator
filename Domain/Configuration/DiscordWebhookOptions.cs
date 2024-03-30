namespace Domain.Configuration;

public class DiscordWebhookOptions
{
    public const string SectionName = "DiscordWebhook";

    public required string Url { get; init; }
}
