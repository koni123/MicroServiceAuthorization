using System.Net;
using Shared.Functions.ServiceTester.Models;

namespace UnitTests.Shared.Functions.ServiceTester.Models;

public class ServiceTestResultTest
{
    private const string TestServiceName = "TestService";

    [Fact]
    public void SuccessResult_CreatesSuccessfulResult_WhenCalled()
    {
        var result = ServiceTestResultItem.SuccessResult(TestServiceName, HttpStatusCode.OK, "Response content");

        Assert.Equivalent(new
        {
            ServiceName = TestServiceName,
            Success = true,
            ResponseContent = "Response content",
            StatusCode = HttpStatusCode.OK,
            ErrorMessage = (string?)null
        }, result);
    }

    [Fact]
    public void ErrorResult_CreatesErrorResult_WhenCalled()
    {
        var result = ServiceTestResultItem.ErrorResult("TestService", HttpStatusCode.BadRequest, "Error message");

        Assert.Equivalent(new
        {
            ServiceName = TestServiceName,
            Success = false,
            ResponseContent = (string?)null,
            StatusCode = HttpStatusCode.BadRequest,
            ErrorMessage = "Error message"
        }, result);
    }

    [Fact]
    public void ExceptionResult_CreatesExceptionResult_WhenCalled()
    {
        var result = ServiceTestResultItem.ExceptionResult("TestService", "Exception message");

        Assert.Equivalent(new
        {
            ServiceName = TestServiceName,
            Success = false,
            ResponseContent = (string?)null,
            StatusCode = (HttpStatusCode?)null,
            ErrorMessage = "Exception message"
        }, result);
    }
}