using System.Security.Cryptography;
using System.Text;
using Domain.Abstractions;
using Domain.Configuration;
using Domain.Dto.Mydesk;
using Implementation.Browser;
using Interface.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Implementation.Service;

public class SessionDataService : ISessionDataService
{
    private readonly ICacheService cacheService;
    private readonly IOptions<DeskReservationOptions> deskReservationOptions;

    public SessionDataService(
        ICacheService cacheService,
        IOptions<DeskReservationOptions> deskReservationOptions)
    {
        this.cacheService = cacheService;
        this.deskReservationOptions = deskReservationOptions;
    }

    public static string HashSensitiveData(string data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            string hashString = Convert.ToBase64String(hashBytes);
            return hashString;
        }
    }

    public async Task<Result<SessionData>> GetSessionData()
    {
        var cacheKey = HashSensitiveData(this.deskReservationOptions.Value.Email + this.deskReservationOptions.Value.Password);
        var data = await this.cacheService.Get(cacheKey);
        if (data is not null)
        {
            var sessionData = JsonConvert.DeserializeObject<SessionData>(data);
            if (sessionData is not null)
            {
                return sessionData;
            }
        }

        var browserOrchestrator = new BrowserOrchestrator(this.deskReservationOptions.Value);
        var result = await browserOrchestrator.GetUserSessionData();
        if (result.IsSuccess)
        {
            var jsonStr = JsonConvert.SerializeObject(result.Unwrap());
            await this.cacheService.Set(cacheKey, jsonStr, TimeSpan.FromHours(1));
        }

        return result;
    }
}
