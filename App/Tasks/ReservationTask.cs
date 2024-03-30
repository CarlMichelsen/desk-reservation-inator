using Domain.Dto.Discord;
using Implementation.Map;
using Interface.Handler;
using Interface.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Tasks;

public class ReservationTask : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    public ReservationTask(IServiceScopeFactory scopeFactory, IHostApplicationLifetime hostApplicationLifetime)
    {
        this.scopeFactory = scopeFactory;
        this.hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = this.scopeFactory.CreateScope())
        {
            var deskReservationHandler = scope.ServiceProvider.GetRequiredService<IDeskReservationHandler>();
            var reservationResult = await deskReservationHandler.ReserveAvaliableDeskSpots();

            DiscordWebhookPayload payload;
            if (reservationResult.IsSuccess)
            {
                payload = DiscordReservationMapper.Map(reservationResult.Unwrap());
            }
            else
            {
                payload = new DiscordWebhookPayload
                {
                    Username = "DeskReservationLog",
                    Content = $"{reservationResult.Error!.Code}\n{reservationResult.Error!.Description}",
                };
            }

            var discordWebhookService = scope.ServiceProvider.GetRequiredService<IDiscordWebhookService>();
            await discordWebhookService.LogMessage(payload);
        }

        this.hostApplicationLifetime.StopApplication();
    }
}