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
        }

        this.hostApplicationLifetime.StopApplication();
    }
}