using Domain.Abstractions;
using Domain.Dto.Discord;

namespace Interface.Service;

public interface IDiscordWebhookService
{
    Task<Result<bool>> LogMessage(DiscordWebhookPayload payload);
}
