using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Shared.Common.Auth.Services;
using Shared.Common.Auth.Settings;

namespace Shared.Common.Auth.Extensions;

/// <summary>
/// Extension methods for adding authenticated HTTP services to the service collection.
/// </summary>
public static class AuthHttpServiceExtensions
{
    /// <summary>
    /// Adds an authenticated HTTP service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the HTTP service to.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="clientAuthOptions">The client authentication options.</param>
    /// <param name="serviceAccessSettings">The service access settings.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clientAuthOptions"/> or <paramref name="serviceAccessSettings"/> is null.</exception>
    public static void AddAuthHttpService(
        this IServiceCollection services,
        string serviceName,
        ClientAuthSettings? clientAuthOptions,
        ServiceAccessSettings? serviceAccessSettings)
    {
        ArgumentNullException.ThrowIfNull(clientAuthOptions);
        ArgumentNullException.ThrowIfNull(serviceAccessSettings);

        var httpClientName = $"{serviceName}HttpClient";
        services.AddHttpClient(httpClientName, client => { client.BaseAddress = new Uri(serviceAccessSettings.BaseUrl); });
        services.AddKeyedSingleton<IAuthHttpService, AuthHttpService>(serviceName, (serviceProvider, _) =>
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientAuthOptions.ClientId)
                .WithClientSecret(clientAuthOptions.ClientSecret)
                .WithTenantId(clientAuthOptions.TenantId)
                .Build();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var tokenAcquirer = new TokenAcquirer(confidentialClientApplication);
            return new AuthHttpService(httpClientFactory, tokenAcquirer, httpClientName, serviceAccessSettings.Scope);
        });
    }
}