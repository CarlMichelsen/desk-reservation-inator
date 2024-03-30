namespace Domain.Dto.Mydesk;

public class SeatResponse
{
    public required bool SpecifySeats { get; init; }

    public required List<Seat> Seats { get; init; }
}
