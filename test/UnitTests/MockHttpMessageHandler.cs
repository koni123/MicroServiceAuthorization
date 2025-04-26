using System.Net;

namespace UnitTests;

public class MockHttpMessageHandler : DelegatingHandler
{
    public HttpRequestMessage? HttpRequestMessage { get; set; }
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        HttpRequestMessage = request;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}