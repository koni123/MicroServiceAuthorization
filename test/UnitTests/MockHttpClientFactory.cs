namespace UnitTests;

public class MockHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        var messageHandlerMock = new MockHttpMessageHandler();
        var httpClient = new HttpClient(messageHandlerMock);
        return httpClient;
    }
}