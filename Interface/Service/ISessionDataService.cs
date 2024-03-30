using Domain.Abstractions;
using Domain.Dto.Mydesk;

namespace Interface.Service;

public interface ISessionDataService
{
    Task<Result<SessionData>> GetSessionData();
}
