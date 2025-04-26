using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Auth.Extensions;
using Shared.Common.Auth.Services;
using Shared.Common.Auth.Settings;

namespace UnitTests.Shared.Common.Auth.Extensions;

public class AuthHttpServiceExtensionsTests
{
    private readonly ClientAuthSettings _clientAuthSettings = new()
    {
        ClientId = "test-client-id",
        ClientSecret = "test-client-secret",
        TenantId = "test-tenant-id"
    };

    private readonly ServiceAccessSettings _serviceAccessSettings = new()
    {
        BaseUrl = "https://example.com",
        Scope = "test-scope"
    };

    private readonly ServiceCollection _services = [];

    [Fact]
    public void AddAuthHttpService_RegistersKeyedAuthHttpService_WhenCalled()
    {
        _services.AddAuthHttpService("TestService", _clientAuthSettings, _serviceAccessSettings);
        var serviceProvider = _services.BuildServiceProvider(); 
        
        var authHttpService = serviceProvider.GetRequiredKeyedService<IAuthHttpService>("TestService");
        Assert.NotNull(authHttpService);
    }

    [Fact]
    public void AddAuthHttpService_RegistersHttpClientWithCorrectBaseAddress_WhenCalled()
    {
        _services.AddAuthHttpService("TestService", _clientAuthSettings, _serviceAccessSettings);
        var serviceProvider = _services.BuildServiceProvider();
        
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("TestServiceHttpClient");
        
        Assert.Equal("https://example.com/", httpClient.BaseAddress?.ToString());
    }

    [Fact]
    public void AddAuthHttpService_Throws_WhenClientAuthOptionsIsNull()
    {
        var result = Record.Exception(() => _services.AddAuthHttpService("TestService", null, _serviceAccessSettings));

        Assert.IsType<ArgumentNullException>(result);
        Assert.Contains("'clientAuthOptions'", result.Message);
    }

    [Fact]
    public void AddAuthHttpService_Throws_WhenServiceAccessSettingsIsNull()
    {
        var result = Record.Exception(() => _services.AddAuthHttpService("TestService", _clientAuthSettings, null));

        Assert.IsType<ArgumentNullException>(result);
        Assert.Contains("'serviceAccessSettings'", result.Message);
    }
}