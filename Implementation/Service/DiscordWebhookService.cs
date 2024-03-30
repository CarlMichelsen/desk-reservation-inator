using System.Text;
using System.Text.Json;
using Domain.Abstractions;
using Domain.Configuration;
using Domain.Dto.Discord;
using Interface.Service;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class DiscordWebhookService : IDiscordWebhookService
{
    private readonly HttpClient httpClient;
    private readonly IOptions<DiscordWebhookOptions> discordWebhookOptions;

    public DiscordWebhookService(
        HttpClient httpClient,
        IOptions<DiscordWebhookOptions> discordWebhookOptions)
    {
        this.httpClient = httpClient;
        this.discordWebhookOptions = discordWebhookOptions;
    }

    public async Task<Result<bool>> LogMessage(DiscordWebhookPayload payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await this.httpClient
                .PostAsync(this.discordWebhookOptions.Value.Url, content);
            
            return res.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            return new Error(
                "DiscordWebhookService.LogMessageException",
                e.Message);
        }
    }
}