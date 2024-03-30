using System.Text.Json.Serialization;

namespace Domain.Dto.Mydesk;

public class SessionData
{
    [JsonPropertyName("homeAccountId")]
    public required string HomeAccountId { get; init; }

    [JsonPropertyName("credentialType")]
    public required string CredentialType { get; init; }

    [JsonPropertyName("secret")]
    public required string Secret { get; init; }

    [JsonPropertyName("cachedAt")]
    public required ulong CachedAt { get; init; }

    [JsonPropertyName("expiresOn")]
    public required ulong ExpiresOn { get; init; }

    [JsonPropertyName("extendedExpiresOn")]
    public required ulong ExtendedExpiresOn { get; init; }

    [JsonPropertyName("environment")]
    public required string Environment { get; init; }

    [JsonPropertyName("clientId")]
    public required Guid ClientId { get; init; }

    [JsonPropertyName("realm")]
    public required Guid Realm { get; init; }

    [JsonPropertyName("target")]
    public required string Target { get; init; }

    [JsonPropertyName("tokenType")]
    public required string TokenType { get; init; }
}
