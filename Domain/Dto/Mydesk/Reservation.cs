namespace Domain.Dto.Mydesk;

public class Reservation
{
    public required long Id { get; init; }

    public required DateTime Date { get; init; }

    public required Area? Area { get; init; }

    public required string Username { get; init; }

    public required string Email { get; init; }

    public required DateTime Created { get; init; }

    public required double? XCoord { get; init; }

    public required double? YCoord { get; init; }

    public required string Status { get; init; }

    public required bool OpenForCheckIn { get; init; }

    public required bool ParkingReserved { get; init; }

    public object? ParkingSpace { get; init; }

    public required bool ParkingAllDay { get; init; }

    public object? ParkingStartTime { get; init; }

    public object? ParkingEndTime { get; init; }

    public object? Guests { get; init; }

    public object? Lunch { get; init; }

    public required bool WorkFromHome { get; init; }

    public object? OutOfOfficeType { get; init; }

    public required Seat? Seat { get; init; }

    public string? CheckInOpen { get; init; }

    public string? CheckInClosing { get; init; }

    public required bool AllDay { get; init; }

    public object? TimeStart { get; init; }

    public object? TimeEnd { get; init; }

    public required bool IsActive { get; init; }

    public required bool IsGuest { get; init; }

    public object? HostEmail { get; init; }

    public object? HostName { get; init; }
}