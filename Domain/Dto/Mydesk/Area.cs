namespace Domain.Dto.Mydesk;

public class Area
{
    public required ulong Id { get; init; }

    public required string Name { get; init; }

    public required int Capacity { get; init; }

    public required Location Location { get; init; }

    public required bool Active { get; init; }

    public required bool AllowQueue { get; init; }

    public string? Note { get; init; }

    public required bool UserHasConflictingReservations { get; init; }

    public List<object> Facilities { get; init; } = new List<object>();

    public required bool SpecifySeats { get; init; }
}