using System.Net;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shared.Common.Auth.Services;
using Shared.Functions.ServiceTester;
using UnitTests.Shared.Common.Auth;

namespace UnitTests.Shared.Functions.ServiceTester;

public class ServiceTesterFunctionTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IAuthHttpService _service1 = Substitute.For<IAuthHttpService>();
    private readonly IAuthHttpService _service2 = Substitute.For<IAuthHttpService>();
    private readonly IAuthHttpService _service3 = Substitute.For<IAuthHttpService>();
    private readonly ServiceTesterFunction _sut;

    public ServiceTesterFunctionTests()
    {
        _sut = new ServiceTesterFunction(_service1, _service2, _service3);
        _service1.SendAsync(Arg.Any<HttpRequestMessage>(), _cancellationToken)
            .Returns(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Service1 response") });
        _service2.SendAsync(Arg.Any<HttpRequestMessage>(), _cancellationToken)
            .Returns(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Service2 error") });
        _service3.SendAsync(Arg.Any<HttpRequestMessage>(), _cancellationToken)
            .Returns(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Service3 error") });
    }

    [Fact]
    public async Task CallAllServicesAsync_ReturnsResultsFromService1_WhenCalled()
    {
        var result = await _sut.CallAllServicesAsync("s1", _cancellationToken);

        Assert.True(result.Results[0].Success);
    }

    [Fact]
    public async Task CallAllServicesAsync_ReturnsResultsFromService2_WhenCalled()
    {
        var result = await _sut.CallAllServicesAsync("s1", _cancellationToken);

        Assert.False(result.Results[1].Success);
    }

    [Fact]
    public async Task CallAllServicesAsync_ReturnsResultsFromService3_WhenCalled()
    {
        var result = await _sut.CallAllServicesAsync("s1", _cancellationToken);

        Assert.False(result.Results[2].Success);
    }

    [Fact]
    public async Task CallAllServicesAsync_ReturnsExceptionResultFromService_WhenThrown()
    {
        _service1.SendAsync(Arg.Any<HttpRequestMessage>(), _cancellationToken)
            .Returns(Task.FromException<HttpResponseMessage>(new HttpRequestException("Service1 exception")));

        var result = await _sut.CallAllServicesAsync("s1", _cancellationToken);

        Assert.False(result.Results[0].Success);
        Assert.Null(result.Results[0].StatusCode);
        Assert.Equal("Service1 exception", result.Results[0].ErrorMessage);
    }
    
    [Fact]
    public void AuthenticateRequest_ReturnsFalse_WhenRoleNotFound()
    {
        var roleName = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Request.Headers["x-ms-client-principal"] = AuthTestingUtil.GetValidPrincipalClaim("does_not_exist");
        
        var result = _sut.AuthorizeRequest(context.Request, roleName);

        Assert.False(result);
    }
    
    [Fact]
    public void AuthenticateRequest_ReturnsTrue_WhenRoleFound()
    {
        var roleName = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Request.Headers["x-ms-client-principal"] = AuthTestingUtil.GetValidPrincipalClaim(roleName);
        
        var result = _sut.AuthorizeRequest(context.Request, roleName);

        Assert.True(result);
    }
}