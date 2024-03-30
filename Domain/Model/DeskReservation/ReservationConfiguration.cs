namespace Domain.Model.DeskReservation;

public class ReservationConfiguration
{
    /// <summary>
    /// Gets the first date a reservation will be attempted.
    /// Will attempt to reserve a seat as many days ahead as possible untill MaxReservations number has been reached.
    /// </summary>
    /// <value>First date that a reservation will be attempted.</value>
    public required DateOnly ReserveFromThisDateInclusive { get; init; }

    public DateOnly LatestReservationDateInclusive { get; init; }

    public bool IncludeWeekends { get; init; } = false;
}
