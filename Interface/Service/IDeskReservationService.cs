using Domain.Abstractions;
using Domain.Model.DeskReservation;

namespace Interface.Service;

public interface IDeskReservationService
{
    Task<Result<List<CompletedReservation>>> ReserveAvailableDesks(ReservationConfiguration config);
}
