namespace Domain.Dto.Mydesk;

public class Person
{
    public required ulong Id { get; init; }

    public required string Username { get; init; }

    public required string Email { get; init; }

    public required bool AllDay { get; init; }

    public required long? TimeStart { get; init; }
    
    public required long? TimeEnd { get; init; }
}
