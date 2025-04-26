using System.Net;
using Microsoft.Identity.Client;
using NSubstitute;
using Shared.Common.Auth.Services;

namespace UnitTests.Shared.Common.Auth.Services;

public class AuthHttpServiceTests
{
    private readonly ITokenAcquirer _tokenAcquirer = Substitute.For<ITokenAcquirer>();
    private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();

    public AuthHttpServiceTests()
    {
        _tokenAcquirer.AcquireTokenAsync(Arg.Any<string>())
            .Returns(GetSuccessResult());
    }

    [Fact]
    public async Task SendAsync_SendsRequestWithAuthHeader_WhenTokenAcquiredSuccessfully()
    {
        var handler = new MockHttpMessageHandler();
        _httpClientFactory.CreateClient("clientName")
            .Returns(new HttpClient(handler));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var sut = new AuthHttpService(_httpClientFactory, _tokenAcquirer, "clientName", "scope");

        var response = await sut.SendAsync(request, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("access_token", handler.HttpRequestMessage?.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task SendAsync_DoNotAcquireToken_WhenScopeEmpty()
    {
        var handler = new MockHttpMessageHandler();
        _httpClientFactory.CreateClient("clientName")
            .Returns(new HttpClient(handler));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var sut = new AuthHttpService(_httpClientFactory, _tokenAcquirer, "clientName", "");

        await sut.SendAsync(request, CancellationToken.None);

        await _tokenAcquirer.Received(0).AcquireTokenAsync(Arg.Any<string>());
    }

    private static AuthenticationResult GetSuccessResult()
    {
        return new AuthenticationResult(
            "access_token",
            false,
            Guid.NewGuid().ToString(),
            DateTimeOffset.MaxValue,
            DateTimeOffset.MaxValue,
            "tenant_id",
            null,
            "id_token",
            ["scope"],
            Guid.Empty);
    }
}