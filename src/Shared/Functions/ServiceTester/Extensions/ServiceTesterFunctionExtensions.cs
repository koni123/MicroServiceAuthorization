using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Auth.Extensions;
using Shared.Common.Auth.Settings;
using Shared.Functions.ServiceTester.Configuration;

namespace Shared.Functions.ServiceTester.Extensions;

/// <summary>
/// Extension methods for adding the Service Tester function functionality to the service collection.
/// </summary>
public static class ServiceTesterFunctionExtensions
{
    /// <summary>
    /// Adds the Service Tester function functionality to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the Service Tester functionality to.</param>
    /// <param name="configuration">The configuration used to retrieve service access settings.</param>
    /// <param name="clientAuthOptions">The client authentication options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clientAuthOptions"/> is null.</exception>
    public static void AddServiceTesterFunction(
        this IServiceCollection services,
        IConfiguration configuration,
        ClientAuthSettings? clientAuthOptions)
    {
        ArgumentNullException.ThrowIfNull(clientAuthOptions);
        var serviceApps = new[] { ServiceTesterConstants.Service1, ServiceTesterConstants.Service2, ServiceTesterConstants.Service3 };

        foreach (var serviceApp in serviceApps)
        {
            services.AddAuthHttpService(serviceApp, clientAuthOptions, configuration.GetSection(serviceApp).Get<ServiceAccessSettings>());
        }

        services.AddSingleton<IServiceTesterFunction, ServiceTesterFunction>();
    }
}