namespace Interface.Service;

public interface ICacheService
{
    Task<string?> Get(string key);

    Task Set(string key, string value, TimeSpan ttl);

    Task Remove(string key);
}
