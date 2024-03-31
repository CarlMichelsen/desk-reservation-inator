using Domain.Abstractions;
using Domain.Dto.Discord;
using Domain.Model.DeskReservation;
using Implementation.Map;
using Interface.Handler;
using Interface.Service;

namespace Implementation.Handler;

public class DeskReservationHandler : IDeskReservationHandler
{
    private readonly IDiscordWebhookService discordWebhookService;
    private readonly IDeskReservationService deskReservationService;

    public DeskReservationHandler(
        IDiscordWebhookService discordWebhookService,
        IDeskReservationService deskReservationService)
    {
        this.discordWebhookService = discordWebhookService;
        this.deskReservationService = deskReservationService;
    }

    public async Task<Result<List<CompletedReservation>>> ReserveAvaliableDeskSpots()
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var config = new ReservationConfiguration
        {
            ReserveFromThisDateInclusive = startDate,
            LatestReservationDateInclusive = startDate.AddDays(15),
            IncludeWeekends = false,
        };

        try
        {
            var result = await this.deskReservationService.ReserveAvailableDesks(config);
            await this.LogResult(result, config);
            return result;
        }
        catch (Exception e)
        {
            Result<List<CompletedReservation>> err = new Error(
                "DeskReservationHandler.Exception",
                e.Message);

            await this.LogResult(err);
            return err;
        }
    }

    private async Task<Result<bool>> LogResult(Result<List<CompletedReservation>> result, ReservationConfiguration? config = null)
    {
        DiscordWebhookPayload payload;
        if (result.IsSuccess)
        {
            payload = DiscordReservationMapper.Map(result.Unwrap(), config!);
        }
        else
        {
            payload = new DiscordWebhookPayload
            {
                Username = "DeskReservationLog",
                Content = $"{result.Error!.Code}\n{result.Error!.Description}",
            };
        }

        return await this.discordWebhookService.LogMessage(payload);
    }
}
