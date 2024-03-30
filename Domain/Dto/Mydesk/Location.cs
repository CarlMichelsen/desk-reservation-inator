namespace Domain.Dto.Mydesk;

public class Location
{
    public required ulong Id { get; init; }

    public required string Name { get; init; }

    public required bool Active { get; init; }

    public string? Note { get; init; }

    public required bool CheckInRequired { get; init; }

    public required bool CheckOutEnabled { get; init; }

    public required int CheckInCloseHour { get; init; }

    public required int CheckInCloseDaysBefore { get; init; }

    public required int CheckInOpenHour { get; init; }

    public required int CheckInOpenDaysBefore { get; init; }

    public required bool ParkingAvailable { get; init; }

    public required int ParkingSpaces { get; init; }

    public required bool RequireSeatSelection { get; init; }

    public required int TimeZoneId { get; init; }

    public required bool HasActiveFloorPlans { get; init; }

    public object? Capacity { get; init; }

    public object? Reservations { get; init; }

    public required bool HasCatering { get; init; }

    public required string Currency { get; init; }
}