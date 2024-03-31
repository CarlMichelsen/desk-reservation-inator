using Domain.Dto.Mydesk;

namespace Domain.Model.DeskReservation;

public class CompletedReservation
{
    public required DateOnly Date { get; init; }

    public required CreateReservation Request { get; init; }

    public required Location Location { get; init; }

    public required Area Area { get; init; }

    public required Seat Seat { get; init; }
}
