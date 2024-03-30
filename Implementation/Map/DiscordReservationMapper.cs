using Domain.Dto.Discord;
using Domain.Model.DeskReservation;

namespace Implementation.Map;

public static class DiscordReservationMapper
{
    public static DiscordWebhookPayload Map(List<CompletedReservation> completedReservations)
    {
        return new DiscordWebhookPayload
        {
            Username = "DeskReservationLog",
            Content = completedReservations.Count > 0
                ? $"Completed {completedReservations.Count} reservations"
                : "No reservation was made",
            Embed = completedReservations.Count > 0
                ? MapToEmbed(completedReservations)
                : null,
        };
    }

    public static DiscordEmbed MapToEmbed(List<CompletedReservation> completedReservations)
    {
        return new DiscordEmbed
        {
            Title = "Reservations",
            Color = 123456,
            Fields = completedReservations.Select(MapToField).ToList(),
        };
    }

    public static DiscordEmbedField MapToField(CompletedReservation completedReservation)
    {
        var dateStr = completedReservation.Date.ToString("yyyy-mm-dd");
        var desc = $"{completedReservation.Location.Name}\n{completedReservation.Area.Name}\n{completedReservation.Seat.Name}";

        return new DiscordEmbedField
        {
            Name = dateStr,
            Value = desc,
        };
    }
}
