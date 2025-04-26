using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Auth.Settings;

namespace Shared.Functions.ServiceTester.Extensions;

public static class FunctionsApplicationBuilderExtensions
{
    public static void ConfigureFunctionsApplicationBuilder(this FunctionsApplicationBuilder builder, string serviceName)
    {
        var keyVault = Environment.GetEnvironmentVariable("EntraKeyVaultName");
        if (!string.IsNullOrWhiteSpace(keyVault))
        {
            TokenCredential tokenCredential = new ManagedIdentityCredential();
#if DEBUG
            tokenCredential = new DefaultAzureCredential();
#endif
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVault}.vault.azure.net/"),
                tokenCredential);
        }

        builder.ConfigureFunctionsWebApplication();
        var clientAuthSettings = builder.Configuration.GetSection(serviceName).Get<ClientAuthSettings>();
        builder.Services.AddServiceTesterFunction(builder.Configuration, clientAuthSettings);
        
        var clientRoleSettings = builder.Configuration.GetSection(serviceName).Get<ClientRoleSettings>();
        if (clientRoleSettings == null)
        {
            throw new NullReferenceException("Client role settings are not configured.");
        }
        builder.Services.AddSingleton(clientRoleSettings);
    }
}