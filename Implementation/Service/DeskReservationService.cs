﻿using Domain.Abstractions;
using Domain.Configuration;
using Domain.Dto.Mydesk;
using Domain.Model.DeskReservation;
using Interface.Client;
using Interface.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class DeskReservationService : IDeskReservationService
{
    private readonly ILogger<DeskReservationService> logger;
    private readonly IMydeskClient mydeskClient;
    private readonly IOptions<DeskReservationOptions> deskReservationOptions;

    public DeskReservationService(
        ILogger<DeskReservationService> logger,
        IMydeskClient mydeskClient,
        IOptions<DeskReservationOptions> deskReservationOptions)
    {
        this.logger = logger;
        this.mydeskClient = mydeskClient;
        this.deskReservationOptions = deskReservationOptions;
    }

    public async Task<Result<List<CompletedReservation>>> ReserveAvailableDesks(ReservationConfiguration config)
    {
        var from = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var to = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        var reservationResult = await this.mydeskClient.GetReservations(from, to);
        
        if (reservationResult.IsError)
        {
            return reservationResult.Error!;
        }

        var datesToAttemptReservation = this.GetRemainingReservationDates(
            reservationResult.Unwrap(),
            config);

        this.logger.LogInformation(
            "Attempting to reserve seat for the following dates {dates}",
            string.Join(", ", datesToAttemptReservation.Select(d => d.ToString("dd-MMM-yyyy"))));

        var completions = new List<CompletedReservation>();
        foreach (var date in datesToAttemptReservation)
        {
            var result = await this.AttemptReservation(date);
            if (result.IsError)
            {
                this.logger.LogInformation(
                    "Reservation attempt failed {code}\n{desc}",
                    result.Error!.Code,
                    result.Error!.Description);
                return result.Error!;
            }

            var completedReservation = result.Unwrap();
            this.logger.LogInformation(
                "Reservation completed for seat {name} -> {id}",
                completedReservation.Seat.Name,
                completedReservation.Seat.Id);
            
            completions.Add(completedReservation);
        }
        
        return completions;
    }

    private async Task<Result<CompletedReservation>> AttemptReservation(DateOnly date)
    {
        var locationsResult = await this.mydeskClient.GetLocations(date);
        if (locationsResult.IsError)
        {
            return locationsResult.Error!;
        }

        var location = locationsResult
            .Unwrap()
            .FirstOrDefault(l => l.Name == this.deskReservationOptions.Value.Location);

        if (location is null)
        {
            return new Error(
                "DeskReservationService.AttemptReservation",
                $"Failed to find location with a name that matches \"{this.deskReservationOptions.Value.Location}\"");
        }

        var areasResult = await this.mydeskClient.GetAreas(location.Id, date, date);
        if (areasResult.IsError)
        {
            return areasResult.Error!;
        }

        var area = areasResult
            .Unwrap()
            .FirstOrDefault(a => a.Name == this.deskReservationOptions.Value.Floorplan);
        
        if (area is null)
        {
            return new Error(
                "DeskReservationService.AttemptReservation",
                $"Failed to find area with a name that matches \"{this.deskReservationOptions.Value.Floorplan}\"");
        }

        var seatsResult = await this.mydeskClient.GetSeats(area.Id, date);
        if (seatsResult.IsError)
        {
            return areasResult.Error!;
        }

        var availableSeats = seatsResult
            .Unwrap()
            .Where(s => s.Active)
            .ToList();

        var seatName = this.deskReservationOptions.Value.SeatPriorityList
            .FirstOrDefault(seatName => availableSeats.Exists(s => s.Name == seatName));
        
        var seat = availableSeats.FirstOrDefault(s => s.Name == seatName);
        
        if (seat is null)
        {
            return new Error(
                "DeskReservationService.AttemptReservation",
                "Failed to find seat that is prioritized");
        }

        var createReservation = new CreateReservation
        {
            AllDay = true,
            AreaId = area.Id,
            SeatId = seat.Id,
            XCoord = seat.XCoord ?? 0,
            YCoord = seat.YCoord ?? 0,
            Date = date.ToDateTime(TimeOnly.MinValue),
            TimeStart = 0,
            TimeEnd = 0,
        };

        var reservationResult = await this.mydeskClient
            .CreateReservation(new CreateReservationPayload { Reservations = new List<CreateReservation> { createReservation } });
        if (reservationResult.IsError)
        {
            return reservationResult.Error!;
        }

        return new CompletedReservation
        {
            Date = date,
            Location = location,
            Area = area,
            Seat = seat,
        };
    }

    private List<DateOnly> GetRemainingReservationDates(
        List<Reservation> exsistingReservations,
        ReservationConfiguration config)
    {
        var datesThatShouldHaveAReservation = new List<DateOnly>();
        var exsistingDatesWithReservation = exsistingReservations
            .Select(r => DateOnly.FromDateTime(r.Date))
            .ToList();
        
        var dateSelector = config.ReserveFromThisDateInclusive;
        while (dateSelector <= config.LatestReservationDateInclusive)
        {
            var isWeekend = dateSelector.DayOfWeek == DayOfWeek.Saturday || dateSelector.DayOfWeek == DayOfWeek.Sunday;
            var isAlreadyReserved = exsistingDatesWithReservation.Exists(d => d == dateSelector);
            if (!config.IncludeWeekends && !isWeekend && !isAlreadyReserved)
            {
                datesThatShouldHaveAReservation.Add(dateSelector);
            }
            
            dateSelector = dateSelector.AddDays(1);
        }

        return datesThatShouldHaveAReservation;
    }
}