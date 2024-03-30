using Domain.Abstractions;
using Domain.Model.DeskReservation;

namespace Interface.Handler;

public interface IDeskReservationHandler
{
    Task<Result<List<CompletedReservation>>> ReserveAvaliableDeskSpots();
}
