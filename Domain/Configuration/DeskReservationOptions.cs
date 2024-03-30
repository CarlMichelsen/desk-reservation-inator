namespace Domain.Configuration;

public class DeskReservationOptions
{
    public const string SectionName = "DeskReservation";

    public required string Email { get; init; }
    
    public required string Password { get; init; }

    public required string MydeskUrl { get; init; }

    public required string Location { get; init; }

    public required string Floorplan { get; init; }

    public required int MaxWaitTimeSeconds { get; init; }

    /// <summary>
    /// Gets list of seat numbers. If a seat has a lower index it has higher priority.
    /// </summary>
    /// <value>Ordered list of seat numbers.</value>
    public required List<string> SeatPriorityList { get; init; }
}
