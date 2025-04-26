using System.Net.Http.Headers;

namespace Shared.Common.Auth.Services;

public interface IAuthHttpService
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}

public class AuthHttpService : IAuthHttpService
{
    private readonly string _httpClientName;
    private readonly string _scope;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenAcquirer _tokenAcquirer;

    public AuthHttpService(
        IHttpClientFactory httpClientFactory,
        ITokenAcquirer tokenAcquirer,
        string httpClientName,
        string scope)
    {
        _httpClientFactory = httpClientFactory;
        _tokenAcquirer = tokenAcquirer;
        _httpClientName = httpClientName;
        _scope = scope;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient(_httpClientName);

        if (string.IsNullOrWhiteSpace(_scope))
        {
            return await client.SendAsync(request, cancellationToken);
        }

        var authResponse = await _tokenAcquirer.AcquireTokenAsync(_scope);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);
        return await client.SendAsync(request, cancellationToken);
    }
}