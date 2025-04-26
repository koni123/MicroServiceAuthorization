using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Service1.Functions;
using Shared.Common.Auth.Settings;
using Shared.Functions.ServiceTester;

namespace UnitTests.Service1.Functions;

public class ServiceResponseGetTests
{
    private readonly IServiceTesterFunction _serviceTesterFunction = Substitute.For<IServiceTesterFunction>();
    private const string RequiredRoleName = "TestRole";
    private readonly ServiceResponseGet _sut;

    public ServiceResponseGetTests()
    {
        var settings = new ClientRoleSettings { RoleName = RequiredRoleName };
        _sut = new ServiceResponseGet(_serviceTesterFunction, settings);
    }
    
    [Fact]
    public void Run_ShouldReturnUnauthorized_WhenAuthorizationFails()
    {
        var req = Substitute.For<HttpRequest>();
        _serviceTesterFunction.AuthorizeRequest(req, RequiredRoleName).Returns(false);

        var result = _sut.Run(req);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public void Run_ShouldReturnOkObject_WhenAuthorizationSucceeds()
    {
        var req = Substitute.For<HttpRequest>();
        _serviceTesterFunction.AuthorizeRequest(req, RequiredRoleName).Returns(true);

        var result = _sut.Run(req);

        Assert.IsType<OkObjectResult>(result);
    }
}