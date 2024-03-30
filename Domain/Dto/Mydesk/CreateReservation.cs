namespace Domain.Dto.Mydesk;

public class CreateReservation
{
    public required DateTime Date { get; init; }

    public required ulong AreaId { get; init; }

    public required ulong SeatId { get; init; }

    public required bool AllDay { get; init; }

    public List<ulong> LunchIds { get; init; } = new List<ulong>();

    public required double XCoord { get; init; }

    public required double YCoord { get; init; }

    public ulong TimeEnd { get; init; } = 0; // Set these two to 0 to reserve all day

    public ulong TimeStart { get; init; } = 0; // Set these two to 0 to reserve all day
}
