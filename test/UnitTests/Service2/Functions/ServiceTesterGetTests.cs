using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Service2.Functions;
using Shared.Common.Auth.Settings;
using Shared.Functions.ServiceTester;
using Shared.Functions.ServiceTester.Configuration;
using Shared.Functions.ServiceTester.Models;

namespace UnitTests.Service2.Functions;

public class ServiceTesterGetTests
{
    private readonly CancellationToken _ct = CancellationToken.None;
    
    private readonly IServiceTesterFunction _serviceTesterFunction = Substitute.For<IServiceTesterFunction>();
    private const string RequiredRoleName = "TestRole";
    private readonly ServiceTesterGet _sut;

    public ServiceTesterGetTests()
    {
        var settings = new ClientRoleSettings { RoleName = RequiredRoleName };
        _sut = new ServiceTesterGet(_serviceTesterFunction, settings);
    }
    
    [Fact]
    public async Task Run_ShouldReturnUnauthorized_WhenAuthorizationFails()
    {
        var req = Substitute.For<HttpRequest>();
        _serviceTesterFunction.AuthorizeRequest(req, RequiredRoleName).Returns(false);

        var result = await _sut.Run(req, _ct);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async Task Run_ShouldReturnOkObject_WhenAuthorizationSucceeds()
    {
        var req = Substitute.For<HttpRequest>();
        _serviceTesterFunction.AuthorizeRequest(req, RequiredRoleName).Returns(true);

        var result = await _sut.Run(req, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);
    }
}