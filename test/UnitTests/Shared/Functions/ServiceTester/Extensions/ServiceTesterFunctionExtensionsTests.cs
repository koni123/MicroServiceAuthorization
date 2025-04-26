using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Auth.Services;
using Shared.Common.Auth.Settings;
using Shared.Functions.ServiceTester;
using Shared.Functions.ServiceTester.Extensions;

namespace UnitTests.Shared.Functions.ServiceTester.Extensions;

public class ServiceTesterFunctionExtensionsTests
{
    private readonly string[] _serviceApps = ["Service1", "Service2", "Service3"];
    private readonly List<KeyValuePair<string, string?>> _testSettings = [];
    private readonly ClientAuthSettings _clientAuthSettings = new()
    {
        ClientId = "test-client-id",
        ClientSecret = "test-client-secret",
        TenantId = "test-tenant-id"
    };

    private readonly IConfiguration _configuration;
    private readonly ServiceCollection _services = [];
    
    public ServiceTesterFunctionExtensionsTests()
    {
        foreach (var serviceApp in _serviceApps)
        {
            _testSettings.Add(new KeyValuePair<string, string?>($"{serviceApp}:BaseUrl", "https://test-example.com"));
            _testSettings.Add(new KeyValuePair<string, string?>($"{serviceApp}:Scope", "scope"));
        }
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_testSettings)
            .Build();
    }

    [Fact]
    public void AddServiceTesterFunction_RegistersAuthHttpServicesForAllServiceApps_WhenCalled()
    {
        _services.AddServiceTesterFunction(_configuration, _clientAuthSettings);
        var serviceProvider = _services.BuildServiceProvider();
        
        foreach (var serviceApp in _serviceApps)
        {
            var service = serviceProvider.GetKeyedService<IAuthHttpService>(serviceApp);
            Assert.NotNull(service);
        }
    }

    [Fact]
    public void AddServiceTesterFunction_RegistersServiceTesterFunction_WhenCalled()
    {
        _services.AddServiceTesterFunction(_configuration, _clientAuthSettings);
        var serviceProvider = _services.BuildServiceProvider();
        
        var serviceTesterFunction = serviceProvider.GetRequiredService<IServiceTesterFunction>();
        Assert.NotNull(serviceTesterFunction);
    }

    [Fact]
    public void AddServiceTesterFunction_Throws_WhenClientAuthOptionsIsNull()
    {
        var result = Record.Exception(() => _services.AddServiceTesterFunction(_configuration, null));

        Assert.IsType<ArgumentNullException>(result);
        Assert.Contains("'clientAuthOptions'", result.Message);
    }
}