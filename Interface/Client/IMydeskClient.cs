using Domain.Abstractions;
using Domain.Dto.Mydesk;

namespace Interface.Client;

public interface IMydeskClient
{
    Task<Result<List<Reservation>>> GetReservations(DateOnly from, DateOnly to);

    Task<Result<List<Location>>> GetLocations(DateOnly date);

    Task<Result<List<Area>>> GetAreas(ulong locationId, DateOnly startDate, DateOnly endDate);

    Task<Result<List<Seat>>> GetSeats(ulong areaId, DateOnly date);

    Task<Result<bool>> CreateReservation(CreateReservationPayload createReservationPayload);
}
