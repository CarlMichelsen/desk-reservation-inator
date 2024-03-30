namespace Domain.Dto.Mydesk;

public class Seat
{
    public required ulong Id { get; init; }

    public required string? Name { get; init; }

    public required double? XCoord { get; init; }

    public required double? YCoord { get; init; }

    public required bool Active { get; init; }

    public object? Temperature { get; init; }

    public object? Humidity { get; init; }

    public object? Light { get; init; }

    public List<Person> Reservations { get; init; } = new List<Person>();

    public object? Sensor { get; init; }
}