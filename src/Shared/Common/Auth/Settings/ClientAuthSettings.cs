namespace Shared.Common.Auth.Settings;

/// <summary>
/// Represents the client authentication options required for accessing a service.
/// </summary>
public class ClientAuthSettings
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string TenantId { get; set; }
}