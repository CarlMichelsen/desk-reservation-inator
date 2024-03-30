using Domain.Abstractions;
using Domain.Model.DeskReservation;
using Interface.Handler;
using Interface.Service;

namespace Implementation.Handler;

public class DeskReservationHandler : IDeskReservationHandler
{
    private readonly IDeskReservationService deskReservationService;

    public DeskReservationHandler(
        IDeskReservationService deskReservationService)
    {
        this.deskReservationService = deskReservationService;
    }

    public async Task<Result<List<CompletedReservation>>> ReserveAvaliableDeskSpots()
    {
        try
        {
            return await this.Run();
        }
        catch (Exception e)
        {
            return new Error(
                "DeskReservationHandler.Exception",
                e.Message);
        }
    }

    private async Task<Result<List<CompletedReservation>>> Run()
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var config = new ReservationConfiguration
        {
            ReserveFromThisDateInclusive = startDate,
            LatestReservationDateInclusive = startDate.AddDays(15),
            IncludeWeekends = false,
        };

        return await this.deskReservationService
            .ReserveAvailableDesks(config);
    }
}
