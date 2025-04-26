namespace Shared.Common.Auth.Settings;

/// <summary>
/// Represents the service access settings required for accessing a service.
/// </summary>
public class ServiceAccessSettings
{
    public required string BaseUrl { get; set; }
    public required string Scope { get; set; }
}