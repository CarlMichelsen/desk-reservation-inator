using System.Text;
using Domain.Dto.Discord;
using Domain.Model.DeskReservation;

namespace Implementation.Map;

public static class DiscordReservationMapper
{
    public static DiscordWebhookPayload Map(List<CompletedReservation> completedReservations, ReservationConfiguration config)
    {
        var fromStr = config.ReserveFromThisDateInclusive.ToString("dd-MMM-yyyy");
        var toStr = config.LatestReservationDateInclusive.ToString("dd-MMM-yyyy");

        string content;
        if (completedReservations.Count == 0)
        {
            content = $"All reservations between(inclusive) {fromStr} and {toStr} have already been made";
        }
        else
        {
            var count = completedReservations.Count;
            var resPlural = count > 1 ? "reservations" : "reservation";
            var reservationList = MapCompletedReservationsToMarkupList(completedReservations);
            content = $"Made {count} {resPlural} between(inclusive) {fromStr} and {toStr}.\n\n{reservationList}";
        }

        return new DiscordWebhookPayload
        {
            Username = "DeskReservationLog",
            Content = content,
        };
    }

    private static string MapCompletedReservationsToMarkupList(List<CompletedReservation> completedReservations)
    {
        var sortedReservations = completedReservations
            .OrderBy(r => r.Location.Id)
            .ThenBy(r => r.Area.Id)
            .ThenBy(r => r.Seat.Id)
            .ThenBy(r => r.Date)
            .ToList();

        StringBuilder result = new StringBuilder();
        ulong currentLocationId = 0;
        ulong currentAreaId = 0;
        ulong currentSeatId = 0;

        foreach (var r in sortedReservations)
        {
            if (r.Location.Id != currentLocationId)
            {
                currentLocationId = r.Location.Id;
                result.AppendLine($"{r.Location.Name}");
                currentAreaId = 0; // Reset area when location changes
            }

            if (r.Area.Id != currentAreaId)
            {
                currentAreaId = r.Area.Id;
                result.AppendLine($"\t{r.Area.Name}");
                currentSeatId = 0; // Reset seat when area changes
            }

            if (r.Seat.Id != currentSeatId)
            {
                currentSeatId = r.Seat.Id;
                result.AppendLine($"\t\t{r.Seat.Name ?? "<unknown>"}");
            }

            var date = r.Date.ToString("dd-MMM-yyyy");
            var allDayString = r.Request.AllDay ? "All day" : "Partial day";

            result.AppendLine($"\t\t\t{date} -> {allDayString}");
        }

        return result.ToString();
    }
}
