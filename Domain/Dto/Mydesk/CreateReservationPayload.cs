namespace Domain.Dto.Mydesk;

public class CreateReservationPayload
{
    public required List<CreateReservation> Reservations { get; init; }
}
