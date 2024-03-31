using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Domain.Abstractions;
using Domain.Configuration;
using Domain.Dto.Mydesk;
using Interface.Client;
using Interface.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Implementation.Client;

public class MydeskClient : IMydeskClient
{
    private const string Url = "https://rest.mydesk.dk";
    private const string ReservationsPath = "/api/Reservations/WithGuests";
    private const string LocationsPath = "/api/Locations";
    private const string AreasPath = "/api/Areas";
    private const string ValidatePath = "/api/Reservations";

    private readonly ILogger<MydeskClient> logger;
    private readonly HttpClient httpClient;
    private readonly ISessionDataService sessionDataService;
    private readonly IOptions<DeskReservationOptions> deskReservationOptions;

    public MydeskClient(
        ILogger<MydeskClient> logger,
        HttpClient httpClient,
        ISessionDataService sessionDataService,
        IOptions<DeskReservationOptions> deskReservationOptions)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.sessionDataService = sessionDataService;
        this.deskReservationOptions = deskReservationOptions;
    }

    public async Task<Result<List<Reservation>>> GetReservations(
        DateOnly from,
        DateOnly to)
    {
        this.logger.LogInformation("GetReservations\nfrom: {from}\nto: {to}", from, to);
        this.httpClient.DefaultRequestHeaders.Authorization = await this.GetAuthenticationHeaderValue();
        
        var email = this.deskReservationOptions.Value.Email;
        var url = $"{Url}{ReservationsPath}?dateFrom={from:dd-MMM-yyyy}&dateTo={to:dd-MMM-yyyy}&username={email}";
        var response = await this.httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new Error(
                "MydeskClient.GetReservationsHttpUnsuccessful",
                response.StatusCode.ToString());
        }

        var reservations = await response.Content
            .ReadFromJsonAsync<List<Reservation>>();
        
        if (reservations is null)
        {
            return new Error(
                "MydeskClient.GetReservationsFailedJsonParse");
        }

        return reservations;
    }

    public async Task<Result<List<Location>>> GetLocations(DateOnly date)
    {
        this.logger.LogInformation("GetLocations\ndate: {date}", date);
        this.httpClient.DefaultRequestHeaders.Authorization = await this.GetAuthenticationHeaderValue();

        var url = $"{Url}{LocationsPath}?date={date:dd-MMM-yyyy}";
        var response = await this.httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new Error(
                "MydeskClient.GetLocationsHttpUnsuccessful",
                response.StatusCode.ToString());
        }

        var locations = await response.Content
            .ReadFromJsonAsync<List<Location>>();
        
        if (locations is null)
        {
            return new Error(
                "MydeskClient.GetLocationsFailedJsonParse");
        }

        return locations;
    }

    public async Task<Result<List<Area>>> GetAreas(
        ulong locationId,
        DateOnly startDate,
        DateOnly endDate)
    {
        this.logger.LogInformation(
            "GetAreas\nlocationId: {locationId}\nstartDate: {startDate}\nendDate: {endDate}",
            locationId,
            startDate,
            endDate);
        
        this.httpClient.DefaultRequestHeaders.Authorization = await this.GetAuthenticationHeaderValue();

        var url = $"{Url}{AreasPath}?endDate={endDate:dd-MMM-yyyy}&locationId={locationId}&startDate={startDate:dd-MMM-yyyy}";
        var response = await this.httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new Error(
                "MydeskClient.GetAreasHttpUnsuccessful",
                response.StatusCode.ToString());
        }

        var areas = await response.Content
            .ReadFromJsonAsync<List<Area>>();
        
        if (areas is null)
        {
            return new Error(
                "MydeskClient.GetAreasFailedJsonParse");
        }

        return areas;
    }

    public async Task<Result<List<Seat>>> GetSeats(
        ulong areaId,
        DateOnly date)
    {
        this.logger.LogInformation(
            "GetSeats\nareaId: {areaId}\ndate: {date}",
            areaId,
            date);
        
        this.httpClient.DefaultRequestHeaders.Authorization = await this.GetAuthenticationHeaderValue();

        var dateStr = date.ToString("yyyy-MM-dd");
        var url = $"{Url}{AreasPath}/{areaId}/Seats?date={dateStr}";
        var response = await this.httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new Error(
                "MydeskClient.GetAreasHttpUnsuccessful",
                response.StatusCode.ToString());
        }

        var seatsResponse = await response.Content
            .ReadFromJsonAsync<SeatResponse>();
        
        if (seatsResponse is null)
        {
            return new Error(
                "MydeskClient.GetAreasFailedJsonParse");
        }

        return seatsResponse.Seats;
    }

    public async Task<Result<bool>> CreateReservation(
        CreateReservationPayload createReservationPayload)
    {
        this.logger.LogInformation(
            "CreateReservation\amount: {amount}",
            createReservationPayload.Reservations.Count);
        this.httpClient.DefaultRequestHeaders.Authorization = await this.GetAuthenticationHeaderValue();

        var url = $"{Url}{ValidatePath}";
        var json = JsonConvert.SerializeObject(createReservationPayload);
        var httpContent = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");
        var response = await this.httpClient.PostAsync(url, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            return new Error(
                "MydeskClient.CreateReservationFailed",
                response.ReasonPhrase);
        }

        return true;
    }

    private async Task<AuthenticationHeaderValue> GetAuthenticationHeaderValue()
    {
        var sessionDataResult = await this.sessionDataService.GetSessionData();
        var sessionData = sessionDataResult.Unwrap();
        return new AuthenticationHeaderValue(sessionData.TokenType, sessionData.Secret);
    }
}